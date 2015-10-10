using EGSW.Web.ActionFilters;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequreSecureConnectionFilter());
        }
    }
}
