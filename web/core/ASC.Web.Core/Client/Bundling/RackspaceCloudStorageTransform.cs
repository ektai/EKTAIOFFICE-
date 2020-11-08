/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@EKTAIOFFICE.com
 *
 * The interactive user interfaces in modified source and object code versions of EKTAIOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original EKTAIOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by EKTAIOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using System.Web.Optimization;

using ASC.Common.Web;
using ASC.Common.Logging;
using ASC.Data.Storage.Configuration;
using ASC.Data.Storage.RackspaceCloud;
using net.openstack.Core.Domain;
using net.openstack.Providers.Rackspace;

namespace ASC.Web.Core.Client.Bundling
{
    class RackspaceCloudStorageTransform : IBundleTransform
    {
        private static readonly ILog log = LogManager.GetLogger("ASC.Web.Bundle.RackspaceCloudStorageTransform");
        private static bool successInitialized = false;
        private static readonly Dictionary<string, string> appenders = new Dictionary<string, string>();
        private static int work = 0;
        private static readonly ConcurrentQueue<CdnItem> queue = new ConcurrentQueue<CdnItem>();

        private static String _username = String.Empty;
        private static String _apiKey = String.Empty;
        private static String _container = String.Empty;
        private static String _subDir = String.Empty;

        static RackspaceCloudStorageTransform()
        {
            try
            {
                var section = (StorageConfigurationSection)WebConfigurationManager.GetSection("storage");

                if (section == null)
                {
                    throw new Exception("Storage section not found.");
                }

                if (section.Appenders.Count == 0)
                {
                    throw new Exception("Appenders not found.");
                }

                foreach (AppenderConfigurationElement a in section.Appenders)
                {
                    var url = string.IsNullOrEmpty(a.AppendSecure) ? a.Append : a.AppendSecure;
                    if (url.StartsWith("~"))
                    {
                        throw new Exception("Only absolute cdn path supported. Can not use " + url);
                    }

                    appenders[a.Extensions + "|"] = url.TrimEnd('/') + "/";
                }

                foreach (HandlerConfigurationElement h in section.Handlers)
                {
                    if (h.Type.Equals(typeof(RackspaceCloudStorage)) && h.Name == "cdn")
                    {
                        _username = h.HandlerProperties["username"].Value;
                        _apiKey = h.HandlerProperties["apiKey"].Value;
                        _container = h.HandlerProperties["container"].Value;
                        _subDir = h.HandlerProperties["subdir"].Value;

                        successInitialized = true;

                        break;
                    }
                }


            }
            catch (Exception fatal)
            {
                log.Fatal(fatal);
            }
        }

        public void Process(BundleContext context, BundleResponse response)
        {
            if (successInitialized && BundleTable.Bundles.UseCdn)
            {
                try
                {
                    var bundle = context.BundleCollection.GetBundleFor(context.BundleVirtualPath);
                    if (bundle != null)
                    {
                        queue.Enqueue(new CdnItem { Bundle = bundle, Response = response });
                        Action upload = UploadToCdn;
                        upload.BeginInvoke(null, null);
                    }
                }
                catch (Exception fatal)
                {
                    log.Fatal(fatal);
                    throw;
                }
            }
        }

        private CloudFilesProvider GetClient()
        {
            CloudIdentity cloudIdentity = new CloudIdentity()
            {
                Username = _username,
                APIKey = _apiKey
            };

            return new CloudFilesProvider(cloudIdentity);
        }

        
        private void UploadToCdn()
        {
            try
            {
                // one thread only
                if (Interlocked.CompareExchange(ref work, 1, 0) == 0)
                {
                    var @continue = false;
                    try
                    {
                        CdnItem item;
                        if (queue.TryDequeue(out item))
                        {
                            @continue = true;

                            var cdnpath = GetCdnPath(item.Bundle.Path);
                            var key = new Uri(cdnpath).PathAndQuery.TrimStart('/');
                            var path = key.Remove(0, _container.Length + 1);

                            var content = Encoding.UTF8.GetBytes(item.Response.Content);
                            var inputStream = new MemoryStream();

                            if (ClientSettings.GZipEnabled)
                            {
                                using (var zip = new GZipStream(inputStream, CompressionMode.Compress, true))
                                {
                                    zip.Write(content, 0, content.Length);
                                    zip.Flush();
                                }
                            }
                            else
                            {
                                inputStream.Write(content, 0, content.Length);
                            }

                            inputStream.Position = 0;

                            bool upload = true;

                            var client = GetClient();                        

                            var etag = SelectelSharp.Common.Helpers.CalculateSHA1(item.Response.Content);

                            var fileInfo = client.ListObjects(_container, 1, null, null,  path);

                            if (fileInfo != null && fileInfo.Any())
                            {
                                upload = fileInfo.Single().Hash != etag;
                            }

                            if (upload)
                            {
                                var contentType = String.Empty;

                                var mime = string.IsNullOrEmpty(contentType) ? MimeMapping.GetMimeMapping(Path.GetFileName(key))
                             : contentType;

                                var customHeaders = new Dictionary<string, string>();

                                if (ClientSettings.GZipEnabled)
                                {
                                    customHeaders.Add("Content-Encoding", "gzip");
                                }

                                var cache = TimeSpan.FromDays(365);

                                customHeaders.Add("Cache-Control", String.Format("public, maxage={0}", (int)cache.TotalSeconds));
                                customHeaders.Add("Expires", DateTime.UtcNow.Add(cache).ToString());

                                client.CreateObject(_container, inputStream, path, mime, 4096, customHeaders);
                            }
                            else
                            {
                                inputStream.Close();
                            }

                            item.Bundle.CdnPath = cdnpath;
                        }
                    }
                    catch (Exception err)
                    {
                        log.Error(err);
                    }
                    finally
                    {
                        work = 0;
                        if (@continue)
                        {
                            Action upload = () => UploadToCdn();
                            upload.BeginInvoke(null, null);
                        }
                    }
                }
            }
            catch (Exception fatal)
            {
                log.Fatal(fatal);
            }
        }

        private static string GetCdnPath(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            var abspath = string.Empty;
            foreach (var a in appenders)
            {
                abspath = a.Value;
                if (a.Key.ToLowerInvariant().Contains(ext + "|"))
                {
                    break;
                }
            }

            return abspath + path.TrimStart('~', '/');
        }

    }
}
