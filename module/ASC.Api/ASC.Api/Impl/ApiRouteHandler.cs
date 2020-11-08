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
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using ASC.Api.Interfaces;
using ASC.Api.Utils;
using ASC.Common.Logging;
using Autofac;

namespace ASC.Api.Impl
{
    public class ApiRouteHandler : IApiRouteHandler
    {
        public ILifetimeScope Container { get; set; }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var authorizations = Container.Resolve<IEnumerable<IApiAuthorization>>().ToList();
            var log = Container.Resolve<ILog>();

            //Authorize request first
            log.DebugFormat("Authorizing {0}", requestContext.HttpContext.Request.Url);


            if (requestContext.RouteData.DataTokens.ContainsKey(DataTokenConstants.RequiresAuthorization)
                && !(bool)requestContext.RouteData.DataTokens[DataTokenConstants.RequiresAuthorization])
            {
                //Authorization is not required for method
                log.Debug("Authorization is not required");
                return GetHandler(requestContext);
            }

            foreach (var apiAuthorization in authorizations)
            {
                log.DebugFormat("Authorizing with:{0}", apiAuthorization.GetType().ToString());
                if (apiAuthorization.Authorize(requestContext.HttpContext))
                {
                    return GetHandler(requestContext);
                }
            }

            if (authorizations.Any(apiAuthorization => apiAuthorization.OnAuthorizationFailed(requestContext.HttpContext)))
            {
                log.Debug("Unauthorized");
                return new ErrorHttpHandler(HttpStatusCode.Unauthorized, HttpStatusCode.Unauthorized.ToString());
            }
            log.Debug("Forbidden");

            return new ErrorHttpHandler(HttpStatusCode.Unauthorized, HttpStatusCode.Unauthorized.ToString());
        }

        public virtual IHttpHandler GetHandler(RequestContext requestContext)
        {
            return Container.BeginLifetimeScope().Resolve<IApiHttpHandler>(new TypedParameter(typeof(RouteData), requestContext.RouteData));
        }
    }

    class ApiAsyncRouteHandler : ApiRouteHandler
    {
        public override IHttpHandler GetHandler(RequestContext requestContext)
        {
            throw new NotImplementedException("This handler is not yet implemented");
        }
    }
}