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


using ASC.Common.Logging;
using ASC.Core;
using ASC.Mail.Core;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.MailServer
{
    public partial class MailServerApi : Interfaces.IApiEntryPoint
    {
        private EngineFactory _engineFactory;

        private EngineFactory MailEngineFactory
        {
            get
            {
                return _engineFactory ??
                       (_engineFactory =
                           new EngineFactory(
                               CoreContext.TenantManager.GetCurrentTenant().TenantId,
                               SecurityContext.CurrentAccount.ID.ToString()));
            }
        }

        ///<summary>
        /// Api name entry
        ///</summary>
        public string Name
        {
            get { return "mailserver"; }
        }

        private ILog _log;

        private ILog Logger
        {
            get { return _log ?? (_log = LogManager.GetLogger("ASC.Api")); }
        }

        private static int TenantId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().TenantId; }
        }

        private static string Username
        {
            get { return SecurityContext.CurrentAccount.ID.ToString(); }
        }
    }
}
