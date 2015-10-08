using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EGSW.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
           // routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //home page
            routes.MapRoute("HomePage",
                            "",
                            new { controller = "Home", action = "GutterCleanRequest" },
                             new[] { "EGSW.Web.Controllers" });

            //GutterCleanRequest 
            routes.MapRoute("GutterCleanRequest",
                            "gutter-clean-request/",
                            new { controller = "Home", action = "GutterCleanRequest" },
                            new[] { "EGSW.Web.Controllers" });

            //GutterCleanRequest 
            routes.MapRoute("GutterCleanInfoRegister",
                            "gutter-clean-info/",
                            new { controller = "Home", action = "GutterCleanInfoRegister" },
                            new[] { "EGSW.Web.Controllers" });

            //ContactUs
            routes.MapRoute("ContactUs",
                            "contact-us/",
                            new { controller = "Home", action = "Contact" },
                            new[] { "EGSW.Web.Controllers" });

            //ReferANeighbor
            routes.MapRoute("ReferANeighbor",
                            "refer-a-neighbor/",
                            new { controller = "Common", action = "ReferANeighbor" },
                            new[] { "EGSW.Web.Controllers" });

            //GenerateQuote
            routes.MapRoute("GenerateQuote",
                            "generate-quote/",
                            new { controller = "Home", action = "GenerateQuote" },
                            new[] { "EGSW.Web.Controllers" });


            //Process Payment  Completed
            routes.MapRoute("ProcessPayment",
                            "process-payment/",
                            new { controller = "Home", action = "ProcessPayment" },
                            new[] { "EGSW.Web.Controllers" });

            // Completed
            routes.MapRoute("Completed",
                            "completed/{orderId}",
                            new { controller = "Home", action = "Completed" },
                            new { orderId = @"\d+" },
                            new[] { "EGSW.Web.Controllers" });

            //login
            routes.MapRoute("Login",
                            "login/",
                            new { controller = "Account", action = "Login" });

            //login
            routes.MapRoute("LogOff",
                            "log-off/",
                            new { controller = "Account", action = "LogOff" });

            //Register 
            routes.MapRoute("Register",
                            "register/",
                            new { controller = "Account", action = "Register" });

            //CustomerInfo
            routes.MapRoute("CustomerInfo",
                            "profile/",
                            new { controller = "Account", action = "Info" });

            //CustomerAddresses
            routes.MapRoute("CustomerAddresses",
                            "addresses/",
                            new { controller = "Account", action = "Addresses" });

            routes.MapRoute("CustomerAddressDelete",
                            "customer/addressdelete/{addressId}",
                            new { controller = "Account", action = "AddressDelete" },
                            new { addressId = @"\d+" });

            routes.MapRoute("CustomerAddressEdit",
                            "customer/addressedit/{addressId}",
                            new { controller = "Account", action = "AddressEdit" },
                            new { addressId = @"\d+" });

            routes.MapRoute("CustomerAddressAdd",
                            "customer/addressadd",
                            new { controller = "Account", action = "AddressAdd" });


            //CustomerOrders
            routes.MapRoute("CustomerOrders",
                            "order/",
                            new { controller = "Order", action = "Index" });

            // Completed
            routes.MapRoute("OrderDetails",
                            "order-details/{orderId}",
                            new { controller = "Order", action = "OrderDetail" },
                            new { orderId = @"\d+" },
                            new[] { "EGSW.Web.Controllers" });

            //CustomerChangePassword
            routes.MapRoute("CustomerChangePassword",
                            "changepassword/",
                            new { controller = "Account", action = "ChangePassword" });



            //passwordrecovery
            routes.MapRoute("PasswordRecovery",
                            "passwordrecovery/",
                            new { controller = "Account", action = "ForgotPassword" },
                            new[] { "EGSW.Web.Controllers" });

            //Reset Password
            routes.MapRoute("ResetPassword",
                            "resetpassword/",
                            new { controller = "Account", action = "ResetPassword" },
                            new[] { "EGSW.Web.Controllers" });

            //GenerateQuote
            routes.MapRoute("Survey",
                            "survey/{surveykey}",
                            new { controller = "Order", action = "Survey" },
                            new[] { "EGSW.Web.Controllers" });


            routes.MapRoute("SurveyConfirmation",
                            "survey-confirmation",
                            new { controller = "Order", action = "SurveyConfirmation" },
                            new[] { "EGSW.Web.Controllers" });



            //Comming Soon
            routes.MapRoute("AboutUs",
                            "about-us/",
                            new { controller = "Static", action = "AboutUs" });

            //Comming Soon
            routes.MapRoute("FAQ",
                            "faq/",
                            new { controller = "Static", action = "Faq" });

            //Comming Soon
            routes.MapRoute("CommingSoon",
                            "comming-soon/",
                            new { controller = "Static", action = "Index" });

            //sitemap (XML)
            routes.MapRoute("sitemap.xml",
                            "sitemap.xml",
                            new { controller = "Common", action = "SeoSitemapXml" });


            routes.MapLocalizedRoute("SeoFriendlyUrl",
                                                 "{SeoFriendlyName}",
                                                 new { controller = "Common", action = "Index" },
                                                 new[] { "EGSW.Web.Controllers" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                , namespaces: new[] { "EGSW.Web.Controllers" }
            );
        }
    }


    public static class LocalizedRouteExtensionMethod
    {
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults, string[] namespaces)
        {
            return MapLocalizedRoute(routes, name, url, defaults, null /* constraints */, namespaces);
        }
        public static Route MapLocalizedRoute(this RouteCollection routes, string name, string url, object defaults, object constraints, string[] namespaces)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            var route = new clsRouteData(url, new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(defaults),
                Constraints = new RouteValueDictionary(constraints),
                DataTokens = new RouteValueDictionary()
            };

            if ((namespaces != null) && (namespaces.Length > 0))
            {
                route.DataTokens["Namespaces"] = namespaces;
            }

            routes.Add(name, route);

            return route;
        }
    }


    public class clsRouteData : Route
    {
        public clsRouteData(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            RouteData data = base.GetRouteData(httpContext);
            if (data != null)
            {
                var SeoFriendliyName = data.Values["SeoFriendlyName"] as string;
                //get here from Database;
                var _seoUrlService = DependencyResolver.Current.GetService<EGSW.Services.SeoUrls.ISeoUrlService>();
                var Resutls = _seoUrlService.GetSeoUrlBySeoName(SeoFriendliyName);
                if (Resutls != null && Resutls.Id > 0)
                {
                    data.Values["controller"] = "Common";
                    data.Values["action"] = "Index";
                    data.Values["Id"] = Resutls.Id;
                }
                else
                {
                    // Add Error page here.
                }
            }
            return data;
        }

    }

}
