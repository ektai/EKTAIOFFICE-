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


using System.Web;
using System.Web.Routing;
using ASC.Api.Impl.Constraints;
using ASC.Api.Interfaces;

namespace ASC.Api.Impl.Routing
{
    public class ApiAccessControlRouteRegistrator : IApiRouteRegistrator
    {
        public IApiConfiguration Config { get; set; }

        public void RegisterRoutes(RouteCollection routes)
        {
            //Register 1 route
            var basePath = Config.GetBasePath();
            var constrasints = new RouteValueDictionary { { "method", ApiHttpMethodConstraint.GetInstance("OPTIONS") } };
            routes.Add(new Route(basePath + "{*path}", null, constrasints, new ApiAccessRouteHandler()));
        }

        public class ApiAccessRouteHandler : IRouteHandler
        {
            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                return new ApiAccessHttpHandler();
            }

            public class ApiAccessHttpHandler : IHttpHandler
            {
                public void ProcessRequest(HttpContext context)
                {
                    //Set access headers
                    //Access-Control-Allow-Origin: http://foo.example  
                    //Access-Control-Allow-Methods: POST, GET, OPTIONS  
                    //Access-Control-Allow-Headers: X-PINGOTHER  
                    //Access-Control-Max-Age: 1728000  
                    //context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                    context.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE";
                    context.Response.Headers["Access-Control-Allow-Headers"] = "origin, authorization, accept, content-type";
                    context.Response.Headers["Access-Control-Max-Age"] = "1728000";

                }

                public bool IsReusable
                {
                    get { return false; }
                }
            }
        }
    }
}