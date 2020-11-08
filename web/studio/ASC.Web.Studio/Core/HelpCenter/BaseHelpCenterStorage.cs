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
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Configuration;
using ASC.Common.Caching;
using ASC.Common.Logging;
using ASC.Common.Threading;
using ASC.Web.Core.Client;

namespace ASC.Web.Studio.Core.HelpCenter
{
    [Serializable]
    [DataContract(Name = "BaseHelpCenterData", Namespace = "")]
    public abstract class BaseHelpCenterData
    {
        [DataMember(Name = "ResetCacheKey")]
        public string ResetCacheKey { get; set; }

        public abstract void Init(string html, string helpLinkBlock, string url);
    }

    class HelpDownloader
    {
        private static readonly DistributedTaskQueue Tasks;
        private static readonly object LockObj;
        private static readonly bool DownloadEnabled;

        static HelpDownloader()
        {
            Tasks = new DistributedTaskQueue("HelpDownloader", 1);
            LockObj = new object();
            if (!bool.TryParse(WebConfigurationManager.AppSettings["web.help-center.download"] ?? "false", out DownloadEnabled))
            {
                DownloadEnabled = false;
            }
        }

        public static void Make(HelpCenterRequest request)
        {
            if (!DownloadEnabled) return;

            lock (LockObj)
            {
                if (Tasks.GetTasks().Any(r => r.GetProperty<string>("Url") == request.Url)) return;

                Tasks.QueueTask(request.SendRequest, request.GetDistributedTask());
            }
        }

        public static void Complete(string url)
        {
            lock (LockObj)
            {
                var task = Tasks.GetTasks().FirstOrDefault(r => r.GetProperty<string>("Url") == url);
                if (task != null)
                {
                    Tasks.RemoveTask(task.Id);
                }
            }
        }
    }

    public class BaseHelpCenterStorage<T> where T : BaseHelpCenterData, new()
    {
        private static Dictionary<string, T> data = new Dictionary<string, T>();
        const string BaseStoragePath = "/App_Data/static/helpcenter/";

        private string FilePath { get; set; }
        private string CacheKey { get; set; }

        private static readonly ICache cache;
        private static readonly TimeSpan expirationTimeout;
        private static readonly ILog Log;

        static BaseHelpCenterStorage()
        {
            cache = AscCache.Memory;
            expirationTimeout = TimeSpan.FromDays(1);
            Log = LogManager.GetLogger("ASC.Web.HelpCenter");
        }

        public BaseHelpCenterStorage(string basePath, string fileName, string cacheKey)
        {
            FilePath = Path.GetFullPath(Path.Combine(basePath + BaseStoragePath, fileName.TrimStart('/')));
            CacheKey = cacheKey;
        }

        public T GetData(string baseUrl, string url, string helpLinkBlock)
        {
            var helpCenterData = GetFromCacheOrFile(url);
            if (helpCenterData != null) return helpCenterData;

            helpCenterData = new T { ResetCacheKey = ClientSettings.ResetCacheKey };
            var request = new HelpCenterRequest
            {
                Url = url,
                BaseUrl = baseUrl,
                HelpLinkBlock = helpLinkBlock,
                Starter = (r, html) =>
                {
                    InitAndCacheData(r, html, helpCenterData);
                    HelpDownloader.Complete(url);
                }
            };

            HelpDownloader.Make(request);

            return null;
        }

        public async Task<T> GetDataAsync(string baseUrl, string url, string helpLinkBlock, string resetCacheKey)
        {
            var helpCenterData = GetFromCacheOrFile(url);

            if (helpCenterData == null)
            {
                helpCenterData = new T {ResetCacheKey = resetCacheKey};
                var request = new HelpCenterRequest
                {
                    Url = url,
                    BaseUrl = baseUrl,
                    HelpLinkBlock = helpLinkBlock,
                    Starter = (r, html) => InitAndCacheData(r, html, helpCenterData)
                };
                await request.SendRequestAsync();
            }

            return helpCenterData;
        }

        private T GetFromCacheOrFile(string url)
        {
            try
            {
                data = FromCache();
                if (data == null)
                {
                    data = FromFile();
                    if (data != null)
                    {
                        ToCache(data);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error GetVideoGuide", e);
            }

            if (data == null)
            {
                data = new Dictionary<string, T>();
            }

            T helpCenterData = null;
            if (data.ContainsKey(url))
            {
                helpCenterData = data[url];
            }
            if (helpCenterData != null &&
                string.CompareOrdinal(helpCenterData.ResetCacheKey, ClientSettings.ResetCacheKey) != 0)
            {
                return null;
            }
            return helpCenterData;
        }

        private void InitAndCacheData(HelpCenterRequest request, string html, T helpCenterData)
        {
            helpCenterData.Init(html, request.HelpLinkBlock, request.BaseUrl);

            lock (data)
            {
                data[request.Url] = helpCenterData;

                ToFile(data);
                ToCache(data);
            }
        }

        private Dictionary<string, T> FromFile()
        {
            if (File.Exists(FilePath))
            {
                using (var stream = File.OpenRead(FilePath))
                {
                    return FromStream(stream);
                }
            }

            return null;
        }

        private void ToFile(Dictionary<string, T> obj)
        {
            try
            {
                using (var filesStream = File.Open(FilePath, FileMode.Create))
                using (var stream = ToStream(obj))
                {
                    if (stream.CanSeek)
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    var buffer = new byte[4096];
                    int readed;
                    while ((readed = stream.Read(buffer, 0, 4096)) != 0)
                    {
                        filesStream.Write(buffer, 0, readed);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error UpdateVideoGuide", e);
            }
        }

        private static MemoryStream ToStream(Dictionary<string, T> obj)
        {
            var stream = new MemoryStream();
            using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(gzip, obj);
            }
            return stream;
        }

        private static Dictionary<string, T> FromStream(Stream stream)
        {
            var formatter = new BinaryFormatter();
            using (var gzip = new GZipStream(stream, CompressionMode.Decompress, true))
            {
                return (Dictionary<string, T>) formatter.Deserialize(gzip);
            }
        }

        private void ToCache(Dictionary<string, T> obj)
        {
            cache.Insert(CacheKey, obj, DateTime.UtcNow.Add(expirationTimeout));
        }

        private Dictionary<string, T> FromCache()
        {
            return cache.Get<Dictionary<string, T>>(CacheKey);
        }
    }

    public class HelpCenterRequest
    {
        public string Url { get; set; }
        public string HelpLinkBlock { get; set; }
        public string BaseUrl { get; set; }
        public Action<HelpCenterRequest, string> Starter { get; set; }
        private static bool stopRequesting;
        private static readonly ILog Log;

        protected DistributedTask TaskInfo { get; private set; }

        static HelpCenterRequest()
        {
            Log = LogManager.GetLogger("ASC.Web.HelpCenter");
        }

        public HelpCenterRequest()
        {
            TaskInfo = new DistributedTask();
        }

        public virtual DistributedTask GetDistributedTask()
        {
            FillDistributedTask();
            return TaskInfo;
        }

        protected virtual void FillDistributedTask()
        {
            TaskInfo.SetProperty("Url", Url);
        }

        internal void SendRequest(DistributedTask task, CancellationToken token)
        {
            var result = string.Empty;
            if (stopRequesting)
            {
                Starter(this, result);
                return;
            }
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);

                httpWebRequest.AllowAutoRedirect = false;
                httpWebRequest.Timeout = 15000;
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers["Accept-Language"] = "en"; // get correct en lang

                var countTry = 0;
                const int maxTry = 3;
                while (countTry < maxTry)
                {
                    try
                    {
                        countTry++;
                        using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                        using (var stream = httpWebResponse.GetResponseStream())
                        using (var reader = new StreamReader(stream, Encoding.GetEncoding(httpWebResponse.CharacterSet)))
                        {
                            result = reader.ReadToEnd();
                            break;
                        }
                    }
                    catch (WebException ex)
                    {
                        if (ex.Status != WebExceptionStatus.Timeout)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                stopRequesting = true;
                Log.Error(string.Format("HelpCenter is not avaliable by url {0}", Url), e);
            }

            Starter(this, result);
        }

        internal async Task SendRequestAsync()
        {
            var httpHandler = new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseProxy = false
            };
            using (var httpClient = new HttpClient(httpHandler) {Timeout = TimeSpan.FromMinutes(1)})
            {
                var dataAsync = await httpClient.GetStringAsync(Url);
                Starter(this, dataAsync);
            }
        }
    }
}