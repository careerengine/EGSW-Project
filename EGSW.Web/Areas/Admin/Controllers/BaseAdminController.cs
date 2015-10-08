using EGSW.Data;
using EGSW.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Areas.Admin.Controllers
{
    [AdminAuthorizeAttribute]
    public class BaseAdminController : Controller
    {
        /// <summary>
        /// Initialize controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            //set work context to admin mode
            DependencyResolver.Current.GetService<IWorkContext>().IsAdmin = true;

            base.Initialize(requestContext);
        }
    }
}