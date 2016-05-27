using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace EGSW.Framework
{
    public static class HtmlExtensions
    {
        public static string GenerateCaptcha(this HtmlHelper helper)
        {
            
            var theme =  "white";
            var captchaControl = new Recaptcha.RecaptchaControl
            {
                ID = "recaptcha",
                Theme = theme,
                //PublicKey = "6LdIPB4TAAAAAAPFVf_js9wSQ5mBVQ-n0RqdmUc7",
                //PrivateKey = "6LdIPB4TAAAAAJ3UVZFyyY9mI3DBELNFjw-ODTp7"

                PublicKey = "6LekyyATAAAAAHneVo8dcKkx4D7fff865X3bHNTg",
                PrivateKey = "6LekyyATAAAAAABSNFkB4B0gp0FE9d4JA2MtyQU7"
            };

            var htmlWriter = new HtmlTextWriter(new StringWriter());

            captchaControl.RenderControl(htmlWriter);

            return htmlWriter.InnerWriter.ToString();
        }
    }
}
