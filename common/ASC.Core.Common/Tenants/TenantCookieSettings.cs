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
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core.Common.Settings;

namespace ASC.Core.Tenants
{
    [Serializable]
    [DataContract]
    public class TenantCookieSettings : BaseSettings<TenantCookieSettings>
    {
        [DataMember(Name = "Index")]
        public int Index { get; set; }

        [DataMember(Name = "LifeTime")]
        public int LifeTime { get; set; }

        private static readonly bool IsVisibleSettings;

        static TenantCookieSettings()
        {
            IsVisibleSettings = !(ConfigurationManager.AppSettings["web.hide-settings"] ?? string.Empty)
                        .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                        .Contains("CookieSettings", StringComparer.CurrentCultureIgnoreCase);
        }

        public override ISettings GetDefault()
        {
            return GetInstance();
        }

        public static TenantCookieSettings GetInstance()
        {
            return new TenantCookieSettings();
        }

        public override Guid ID
        {
            get { return new Guid("{16FB8E67-E96D-4B22-B217-C80F25C5DE1B}"); }
        }


        public bool IsDefault()
        {
            var defaultSettings = GetInstance();

            return LifeTime == defaultSettings.LifeTime;
        }


        public static TenantCookieSettings GetForTenant(int tenantId)
        {
            return IsVisibleSettings
                       ? LoadForTenant(tenantId)
                       : GetInstance();
        }

        public static void SetForTenant(int tenantId, TenantCookieSettings settings = null)
        {
            if (!IsVisibleSettings) return;
            (settings ?? GetInstance()).SaveForTenant(tenantId);
        }

        public static TenantCookieSettings GetForUser(Guid userId)
        {
            return IsVisibleSettings
                       ? LoadForUser(userId)
                       : GetInstance();
        }

        public static TenantCookieSettings GetForUser(int tenantId, Guid userId)
        {
            return IsVisibleSettings
                       ? SettingsManager.Instance.LoadSettingsFor<TenantCookieSettings>(tenantId, userId)
                       : GetInstance();
        }

        public static void SetForUser(Guid userId, TenantCookieSettings settings = null)
        {
            if (!IsVisibleSettings) return;
            (settings ?? GetInstance()).SaveForUser(userId);
        }

        public static DateTime GetExpiresTime(int tenantId)
        {
            var settingsTenant = GetForTenant(tenantId);
            var expires = settingsTenant.IsDefault() ? DateTime.UtcNow.AddYears(1) : DateTime.UtcNow.AddMinutes(settingsTenant.LifeTime);
            return expires;
        }
    }
}