using EGSW.Data;
using EGSW.Data.Customers;
using EGSW.Framework;
using EGSW.Services;
using EGSW.Services.Authentication;
using EGSW.Web.ActionFilters;
using EGSW.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.IO;

namespace EGSW.Web.Controllers
{
    [RequreSecureConnectionFilter]
    public class CommonController : Controller
    {

         private readonly IAuthenticationService _authenticationService;  
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        public CommonController(ICustomerService customerService, IWorkContext workContext, 
            IAuthenticationService authenticationService,
            IWorkflowMessageService workflowMessageService)
        {
            this._customerService = customerService;
            this._workContext = workContext;
            this._authenticationService = authenticationService;
            this._workflowMessageService = workflowMessageService;

        }

        public ActionResult Index(int Id)
        {
            //get here from Database;
            var _seoUrlService = DependencyResolver.Current.GetService<EGSW.Services.SeoUrls.ISeoUrlService>();
            var Result = _seoUrlService.GetSeoUrlById(Id);
            
            var randomList = _seoUrlService.GetCountySeoUrl(Result.CountyName).OrderBy(n => Guid.NewGuid()).Take(5);
            //var randomList = _seoUrlService.GetAllSeoUrl().OrderBy(n => Guid.NewGuid()).Take(5);
            ViewBag.SeoRandomList = randomList;

            return View(Result);
        }

        public ActionResult Update()
        {
            var _seoUrlService = DependencyResolver.Current.GetService<EGSW.Services.SeoUrls.ISeoUrlService>();

            var seoUrlList = _seoUrlService.GetAllSeoUrl();
            seoUrlList = seoUrlList.Where(i => i.Id > 66).ToList();
            foreach (SeoUrl entity in seoUrlList)
            {
                entity.SeoName = entity.CityName.Replace(" ", "-") + "-" + entity.CountyName.Replace("-county", "") + "-" + entity.StateAbbr.Replace(" ", "-");
                entity.SeoName = entity.SeoName.ToLower();
                _seoUrlService.UpdateSeoUrl(entity);

            }

            return Content("All Url Update");
        }


        public ActionResult SeoSitemapXml()
        {
            List<ISitemapItem> _items;
            _items = new List<ISitemapItem>();


            string siteUrl = ConfigurationManager.AppSettings["SiteUrl"];
            siteUrl = siteUrl.Remove(siteUrl.LastIndexOf("/"));
            //_items.Add(new SitemapItem(siteUrl+Url.RouteUrl("HomePage")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
            //_items.Add(new SitemapItem(siteUrl + Url.RouteUrl("AboutUs")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
            //_items.Add(new SitemapItem(siteUrl + Url.RouteUrl("ContactUs")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
            //_items.Add(new SitemapItem(siteUrl + Url.RouteUrl("FAQ")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
            //_items.Add(new SitemapItem(siteUrl + Url.RouteUrl("ReferANeighbor")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
            //_items.Add(new SitemapItem(siteUrl + Url.RouteUrl("Register")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
            //_items.Add(new SitemapItem(siteUrl + Url.RouteUrl("Login")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
            //_items.Add(new SitemapItem(siteUrl + Url.RouteUrl("PasswordRecovery")) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });

            string line;
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\URL.txt");

            try
            {
                System.IO.StreamReader file = new System.IO.StreamReader(path);
                //_items.Add(new SitemapItem(siteUrl) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
                while ((line = file.ReadLine()) != null)
                {
                    _items.Add(new SitemapItem(siteUrl + Url.RouteUrl(line)) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });
                }

                file.Close();
            }
            catch (Exception e) { }

            
            
            var _seoUrlService = DependencyResolver.Current.GetService<EGSW.Services.SeoUrls.ISeoUrlService>();

            var seoUrlList = _seoUrlService.GetAllSeoUrl();

            
            foreach (var entity in seoUrlList)
            {

                _items.Add(new SitemapItem(ConfigurationManager.AppSettings["SiteUrl"] +entity.SeoName) { ChangeFrequency = ChangeFrequency.Monthly, LastModified = DateTime.Now, Priority = 1 });

            }

            




            return new XmlSitemapResult(_items);
        }


        //header links
        [ChildActionOnly]
        public ActionResult HeaderLinks()
        {
            var customer = _workContext.CurrentCustomer;



            var model = new HeaderLinksModel
            {
                IsAuthenticated = (customer.IsRegistered() || customer.IsAdmin() || customer.IsAgent())?true:false,
                CustomerEmailUsername = (customer.IsRegistered() || customer.IsAdmin() || customer.IsAgent()) ? customer.Email : "",
                DisplayAdminLink = (customer.IsAdmin() || customer.IsAgent()),

            };

            if (customer.IsAdmin())
            {
                model.Text = "Admin";
            }
            else if (customer.IsAgent())
            {
                model.Text = "Agent";
            }

            //var model = new HeaderLinksModel
            //{
            //    IsAuthenticated = customer.IsRegistered(),
            //    CustomerEmailUsername = customer.IsRegistered() ? customer.Email : "",
            //    DisplayAdminLink = customer.IsAdmin(),

            //};

           
            
            return PartialView(model);
        }

        public ActionResult CityNames()
        {
            var val = Request.QueryString["name"];
            //get here from Database;
            var city = val;
            var _seoUrlService = DependencyResolver.Current.GetService<EGSW.Services.SeoUrls.ISeoUrlService>();
           // var Result = _seoUrlService.GetCountySeoUrl(city);

            var randomList = _seoUrlService.GetCountySeoUrl(city).OrderBy(n => n.CityName);
            ViewBag.SeoRandomList = randomList;
            ViewBag.countyName = city;
            return View();
            //return View();
        }
        
        public ActionResult ReferANeighbor()
        {
            ReferNeighborModel model = new ReferNeighborModel();

            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        public ActionResult ReferANeighbor(ReferNeighborModel model, bool captchaValid)
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("", "Wrong Captcha code.");
            }

            if (ModelState.IsValid)
            {
                model.Result = true;



                _workflowMessageService.SendReferANeighborMessage(model.FriendName, model.FriendEmail, model.YourName);
            }

            return View(model);
        }
        
    }
}