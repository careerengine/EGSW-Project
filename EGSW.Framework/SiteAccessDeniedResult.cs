using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EGSW.Framework
{
    public class SiteAccessDeniedResult : ViewResult
    {
        public SiteAccessDeniedResult()
        {
            ViewName = "~/Areas/Admin/Views/Shared/AccessDenied.cshtml";
        }
    }
}
