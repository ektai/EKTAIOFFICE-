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
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Tenants;
using Newtonsoft.Json;

namespace ASC.Web.Core.WhiteLabel
{
    [Serializable]
    [DataContract]
    public class CompanyWhiteLabelSettings : BaseSettings<CompanyWhiteLabelSettings>
    {
        [DataMember(Name = "CompanyName")]
        public string CompanyName { get; set; }

        [DataMember(Name = "Site")]
        public string Site { get; set; }

        [DataMember(Name = "Email")]
        public string Email { get; set; }

        [DataMember(Name = "Address")]
        public string Address { get; set; }

        [DataMember(Name = "Phone")]
        public string Phone { get; set; }

        [DataMember(Name = "IsLicensor")]
        public bool IsLicensor { get; set; }

        public bool IsDefault
        {
            get
            {
                var defaultSettings = GetDefault() as CompanyWhiteLabelSettings;

                if (defaultSettings == null) return false;

                return CompanyName == defaultSettings.CompanyName &&
                       Site == defaultSettings.Site &&
                       Email == defaultSettings.Email &&
                       Address == defaultSettings.Address &&
                       Phone == defaultSettings.Phone &&
                       IsLicensor == defaultSettings.IsLicensor;
            }
        }

        #region ISettings Members

        public override Guid ID
        {
            get { return new Guid("{C3C5A846-01A3-476D-A962-1CFD78C04ADB}"); }
        }

        private static CompanyWhiteLabelSettings _default;

        public override ISettings GetDefault()
        {
            if (_default != null) return _default;

            var settings = CoreContext.Configuration.GetSetting("CompanyWhiteLabelSettings");

            _default = string.IsNullOrEmpty(settings) ? new CompanyWhiteLabelSettings() : JsonConvert.DeserializeObject<CompanyWhiteLabelSettings>(settings);

            return _default;
        }

        #endregion

        public static CompanyWhiteLabelSettings Instance
        {
            get
            {
                return LoadForDefaultTenant();
            }
        }
    }
}
