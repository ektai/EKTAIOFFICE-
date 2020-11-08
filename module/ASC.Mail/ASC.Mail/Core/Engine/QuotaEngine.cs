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
using ASC.Common.Logging;
using ASC.Data.Storage;

namespace ASC.Mail.Core.Engine
{
    public class QuotaEngine
    {
        public int Tenant { get; private set; }
        public ILog Log { get; private set; }

        public QuotaEngine(int tenant, ILog log = null)
        {
            Tenant = tenant;
            Log = log ?? LogManager.GetLogger("ASC.Mail.QuotaEngine");
        }

        public void QuotaUsedAdd(long usedQuota)
        {
            try
            {
                var quotaController = new TenantQuotaController(Tenant);
                quotaController.QuotaUsedAdd(Defines.MODULE_NAME, string.Empty, Defines.MAIL_QUOTA_TAG, usedQuota);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("QuotaUsedAdd with params: tenant={0}, used_quota={1}", Tenant, usedQuota), ex);

                throw;
            }
        }

        public void QuotaUsedDelete(long usedQuota)
        {
            try
            {
                var quotaController = new TenantQuotaController(Tenant);
                quotaController.QuotaUsedDelete(Defines.MODULE_NAME, string.Empty, Defines.MAIL_QUOTA_TAG, usedQuota);
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("QuotaUsedDelete with params: tenant={0}, used_quota={1}", Tenant, usedQuota), ex);

                throw;
            }
        }
    }
}
