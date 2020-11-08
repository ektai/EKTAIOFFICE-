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
using System;
using System.Text;
using System.Web;

namespace ASC.Web.Core.Helpers
{
    public class AuthorizationHelper
    {
        public static bool ProcessBasicAuthorization(HttpContext context)
        {
            string authCookie;
            return ProcessBasicAuthorization(context, out authCookie);
        }

        public static bool ProcessBasicAuthorization(HttpContext context, out string authCookie)
        {
            authCookie = null;
            try
            {
                //Try basic
                var authorization = context.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    return false;
                }

                authorization = authorization.Trim();
                if (0 <= authorization.IndexOf("Basic", 0))
                {
                    var arr = Encoding.ASCII.GetString(Convert.FromBase64String(authorization.Substring(6))).Split(new[] { ':' });
                    var username = arr[0];
                    var password = arr[1];
                    var u = CoreContext.UserManager.GetUserByEmail(username);
                    if (u != null && u.ID != ASC.Core.Users.Constants.LostUser.ID)
                    {
                        authCookie = SecurityContext.AuthenticateMe(u.Email, password);
                    }
                }
                else if (0 <= authorization.IndexOf("Bearer", 0))
                {
                    authorization = authorization.Substring("Bearer ".Length);
                    if (SecurityContext.AuthenticateMe(authorization))
                    {
                        authCookie = authorization;
                    }
                }
                else
                {
                    if (SecurityContext.AuthenticateMe(authorization))
                    {
                        authCookie = authorization;
                    }
                }
            }
            catch (Exception) { }
            return SecurityContext.IsAuthenticated;
        }
    }
}