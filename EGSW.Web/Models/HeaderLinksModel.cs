using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGSW.Web.Models
{
    public partial class HeaderLinksModel 
    {
        public bool IsAuthenticated { get; set; }
        public string CustomerEmailUsername { get; set; }
        public bool DisplayAdminLink { get; set; }
        public string Text { get; set; }
    }
}