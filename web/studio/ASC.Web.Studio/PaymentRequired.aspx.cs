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


using ASC.Core;
using ASC.Core.Billing;
using ASC.Web.Core.Files;
using ASC.Web.Core.WhiteLabel;
using ASC.Web.Studio.Utility;
using Resources;
using System;
using System.Web;

namespace ASC.Web.Studio
{
    public partial class PaymentRequired : MainPage
    {
        public static string Location
        {
            get { return "~/PaymentRequired.aspx"; }
        }

        protected AdditionalWhiteLabelSettings Settings;

        protected override bool MayNotAuth
        {
            get { return true; }
        }

        protected override bool MayNotPaid
        {
            get { return true; }
            set { }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (CoreContext.Configuration.Personal)
                Context.Response.Redirect(FilesLinkUtility.FilesBaseAbsolutePath);

            if (TenantExtra.GetCurrentTariff().State < TariffState.NotPaid)
                Response.Redirect(CommonLinkUtility.GetDefault(), true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Master.DisabledSidePanel = true;
            Master.TopStudioPanel.DisableUserInfo = true;
            Master.TopStudioPanel.DisableProductNavigation = true;
            Master.TopStudioPanel.DisableSearch = true;
            Master.TopStudioPanel.DisableSettings = true;
            Master.TopStudioPanel.DisableTariff = true;
            Master.TopStudioPanel.DisableLoginPersonal = true;

            Title = HeaderStringHelper.GetPageTitle(Resource.PaymentRequired);

            Page.RegisterStyle("~/UserControls/Management/TariffSettings/css/tariff.less");
            Page.RegisterStyle("~/UserControls/Management/TariffSettings/css/tariffstandalone.less");

            Settings = AdditionalWhiteLabelSettings.Instance;
        }
    }
}