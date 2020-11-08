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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using ASC.Core;

using HtmlAgilityPack;
using Newtonsoft.Json;

namespace ASC.Web.Core.Client.HttpHandlers
{
    public abstract class ClientScript
    {
        protected virtual bool CheckAuth { get { return true; } }

        protected virtual string BaseNamespace
        {
            get { return "ASC.Resources"; }
        }


        protected abstract IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context);

        public async Task GetDataAsync(HttpContext context, StringBuilder builder)
        {
            var store = GetClientVariables(context);
            if (store == null) return;

            if (!string.IsNullOrEmpty(BaseNamespace))
            {
                var namespaces = BaseNamespace.Split('.');
                for (var index = 1; index <= namespaces.Length; index++)
                {
                    var ns = string.Join(".", namespaces, 0, index);
                    builder.AppendFormat("if (typeof({0})==='undefined'){{{0} = {{}};}} ", ns);
                }
            }

            foreach (var clientObject in store)
            {
                var resourceSet = clientObject.Value as ClinetResourceSet;
                if (resourceSet != null)
                {
                    builder.AppendFormat("{0}.{1}={2};", BaseNamespace, clientObject.Key, JsonConvert.SerializeObject(resourceSet.GetResources()));
                    continue;
                }

                var templateSet = clientObject.Value as ClientTemplateSet;
                if (templateSet != null)
                {
                    builder.Append(Environment.NewLine);
                    await templateSet.GetClientTemplatesAsync(builder);
                    continue;
                }

                if (CheckAuth && !SecurityContext.CurrentAccount.IsAuthenticated)
                {
                    continue;
                }

                builder.AppendFormat("jq.extend({0},{1});", BaseNamespace, JsonConvert.SerializeObject(clientObject.Value));
            }
        }

        protected internal virtual string GetCacheHash()
        {
            return string.Empty;
        }


        protected KeyValuePair<string, object> RegisterObject(object value)
        {
            return new KeyValuePair<string, object>(Guid.NewGuid().ToString(), value);
        }

        protected KeyValuePair<string, object> RegisterResourceSet(string key, ResourceManager resourceManager)
        {
            return new KeyValuePair<string, object>(key, new ClinetResourceSet(resourceManager));
        }

        protected IEnumerable<KeyValuePair<string, object>> RegisterClientTemplatesPath(HttpContext context, params string[] virtualPath)
        {
            return new List<KeyValuePair<string, object>>(1)
            {
                new KeyValuePair<string, object>(Guid.NewGuid().ToString(), new ClientTemplateSet(virtualPath.Select(path => context.Server.MapPath(path)).ToArray()))
            };
        }

        public static void GetTemplateData(string input, StringBuilder builder)
        {
            input = Regex.Replace(input, @"\s*(<[^>]+>)\s*", "$1", RegexOptions.Singleline);
            var doc = new HtmlDocument();
            doc.LoadHtml(input);
            var nodes = doc.DocumentNode.SelectNodes("/script[@type='text/x-jquery-tmpl']");
            var templates = nodes.ToDictionary(x => x.Attributes["id"].Value, y => y.InnerHtml);

            foreach (var template in templates)
            {
                // only for jqTmpl for now
                builder.Append(string.Format("jQuery.template('{0}', '{1}');{2}",
                    template.Key,
                    template.Value
                        .Replace("\r\n", "")
                        .Replace("\r", "")
                        .Replace("\n", "")
                        .Replace(Environment.NewLine, "")
                        .Replace("'", "\\'"), Environment.NewLine));
            }
        }

        public static async Task GetTemplateDataAsync(string[] paths, StringBuilder builder)
        {
            var dataBuilder = new StringBuilder();
            foreach (var path in paths)
            {
                using (var reader = File.OpenText(path))
                {
                    dataBuilder.Append(await reader.ReadToEndAsync());
                }
            }
            GetTemplateData(dataBuilder.ToString(), builder);
        }

        class ClientTemplateSet
        {
            private readonly string[] paths;

            public ClientTemplateSet(params string[] paths)
            {
                this.paths = paths;
            }

            public async Task GetClientTemplatesAsync(StringBuilder builder)
            {
                await GetTemplateDataAsync(paths, builder);
            }
        }

        class ClinetResourceSet
        {
            private readonly ResourceManager manager;

            public ClinetResourceSet(ResourceManager manager)
            {
                this.manager = manager;
            }

            public IDictionary<string, string> GetResources()
            {
                return manager.GetResourceSet(CultureInfo.InvariantCulture, true, true)
                    .Cast<DictionaryEntry>()
                    .Select(e => (string)e.Key)
                    .ToDictionary(k => k, k => manager.GetString(k));
            }
        }
    }
}