using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.ActionFilters
{
    public class RequreSecureConnectionFilter : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Request.IsLocal)
            {
                // when connection to the application is local, don't do any HTTPS stuff
                return;
            }
            else if (filterContext.HttpContext.Request.Url.Host.ToLower().Contains("test-gutter.classifiedsolutionsgroup") || filterContext.HttpContext.Request.Url.Host.ToLower().Contains("ericsguttercleaning"))
            {
                // when connection to the application is local, don't do any HTTPS stuff
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}