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


var GreetingSettingsManager = new function () {

    var _bindAjaxProOnLoading = function () {
        AjaxPro.onLoading = function (b) {
            if (b) {
                LoadingBanner.showLoaderBtn("#studio_greetingSettings");
            } else {
                LoadingBanner.hideLoaderBtn("#studio_greetingSettings");
            }
        };
    };

    var _saveGreetingOptions = function () {
        _bindAjaxProOnLoading();
        GreetingSettingsController.SaveGreetingSettings(jq('#studio_greetingHeader').val(),
                                                function (result) {
                                                    LoadingBanner.showMesInfoBtn("#studio_greetingSettings", result.value.Message, result.value.Status == 1 ? "success" : "error");
                                                });
      };

    var _restoreGreetingOptions = function () {
        _bindAjaxProOnLoading();

        GreetingSettingsController.RestoreGreetingSettings(function (result) {
            if (result.value.Status == 1) {
                jq('#studio_greetingHeader').val(result.value.CompanyName);
            }
            LoadingBanner.showMesInfoBtn("#studio_greetingSettings", result.value.Message, result.value.Status == 1 ? "success" : "error");

        });
    };

    var _uploadCompleteGreetingLogo = function (file, response) {
        LoadingBanner.hideLoaderBtn("#studio_greetingSettings");

        var result = eval("(" + response + ")");

        if (!result.Success) {
            LoadingBanner.showMesInfoBtn("#studio_greetingSettings", result.Message, "error");
        }
    };

    this.Init = function () {
        jq('#saveGreetSettingsBtn').on("click", _saveGreetingOptions);
        jq('#restoreGreetSettingsBtn').on("click", _restoreGreetingOptions);
    }
};


jq(function () {
    GreetingSettingsManager.Init();
});