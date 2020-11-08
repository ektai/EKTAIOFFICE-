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
using System.Threading;
using System.Web;
using ASC.Common.Logging;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Web.Core.Client.Bundling;
using ASC.Web.Core.WhiteLabel;

namespace ASC.Web.Core.Client.HttpHandlers
{
    public abstract class ClientScriptLocalization : ClientScript
    {
        protected override bool CheckAuth { get { return false; } }

        protected internal sealed override string GetCacheHash()
        {
            var result = ClientSettings.ResetCacheKey + Thread.CurrentThread.CurrentCulture.Name;

            try
            {
                var tenantId = CoreContext.TenantManager.GetCurrentTenant().TenantId;
                var whiteLabelSettings = TenantWhiteLabelSettings.Load();

                if (!string.IsNullOrEmpty(whiteLabelSettings.LogoText))
                {
                    result += tenantId.ToString() + whiteLabelSettings.LogoText;
                }
            }
            catch (Exception e)
            {
                LogManager.GetLogger("ASC").Error(e);
            }

            return result;
        }
    }

    public abstract class ClientScriptTemplate : ClientScript
    {
        protected override bool CheckAuth { get { return false; } }

        protected abstract string[] Links { get; }

        protected sealed override string BaseNamespace { get { return ""; } }

        protected sealed override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            return RegisterClientTemplatesPath(context, Links);
        }

        protected internal sealed override string GetCacheHash()
        {
            return ClientSettings.ResetCacheKey;
        }

        public string[] GetLinks()
        {
            if (ClientSettings.BundlingEnabled)
            {
                return Links;
            }

            return new[] { new ClientScriptReference().AddScript(this).GetLink() };
        }
    }
}
