using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace EGSW.Services
{
    public  class SiteSetting
    {
        public string SiteUrl { get; set; }
        public string SiteSubject { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string ReturnEmail { get; set; }
        public string BCCEmail { get; set; }
        public string AdminEmail { get; set; }

        
        //public string SiteUrl { get; set; }

        public SiteSetting()
        {
            SiteUrl = ConfigurationManager.AppSettings["SiteUrl"].ToString();
            SiteSubject = ConfigurationManager.AppSettings["SiteSubject"].ToString();
            SenderEmail = ConfigurationManager.AppSettings["SenderEmail"].ToString();
            SenderName = ConfigurationManager.AppSettings["SenderName"].ToString();
            ReturnEmail = ConfigurationManager.AppSettings["ReturnEmail"].ToString();
            BCCEmail = ConfigurationManager.AppSettings["BCCEmail"].ToString();
            AdminEmail = ConfigurationManager.AppSettings["AdminEmail"].ToString();
        }
    }
}
