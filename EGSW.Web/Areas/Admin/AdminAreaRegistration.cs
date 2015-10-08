using System.Web.Mvc;

namespace EGSW.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {

            ////home page
            //routes.MapRoute("GutterCleanRequest",
            //                "gutter-clean-request/",
            //                new { controller = "Home", action = "GutterCleanRequest" },
            //                new[] { "EGSW.Web.Controllers" });

            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }                
            );
        }
    }
}