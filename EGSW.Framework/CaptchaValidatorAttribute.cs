using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;


namespace EGSW.Framework
{
    public class CaptchaValidatorAttribute : ActionFilterAttribute
    {
        private const string CHALLENGE_FIELD_KEY = "recaptcha_challenge_field";
        private const string RESPONSE_FIELD_KEY = "recaptcha_response_field";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool valid = false;
            var captchaChallengeValue = filterContext.HttpContext.Request.Form[CHALLENGE_FIELD_KEY];
            var captchaResponseValue = filterContext.HttpContext.Request.Form[RESPONSE_FIELD_KEY];
            if (!string.IsNullOrEmpty(captchaChallengeValue) && !string.IsNullOrEmpty(captchaResponseValue))
            {
                               
                    //validate captcha
                    var captchaValidtor = new Recaptcha.RecaptchaValidator
                    {
                        PrivateKey = "6LdIPB4TAAAAAJ3UVZFyyY9mI3DBELNFjw-ODTp7",//captchaSettings.ReCaptchaPrivateKey,
                        RemoteIP = filterContext.HttpContext.Request.UserHostAddress,
                        Challenge = captchaChallengeValue,
                        Response = captchaResponseValue
                    };

                    var recaptchaResponse = captchaValidtor.Validate();
                    valid = recaptchaResponse.IsValid;
                
            }

            //this will push the result value into a parameter in our Action  
            filterContext.ActionParameters["captchaValid"] = valid;

            base.OnActionExecuting(filterContext);
        }
    }
}
