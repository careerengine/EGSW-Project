using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using EGSW.Web.Models;
using EGSW.Services;
using EGSW.Data;
using EGSW.Data.Customers;
using EGSW.Services.Authentication;
using EGSW.Web.Models.Customers;
using EGSW.Services.Directory;
using EGSW.Web.Models.Orders;
using System.IO;
using EGSW.Web.ActionFilters;

namespace EGSW.Web.Controllers
{
    //[Authorize]
    [RequreSecureConnectionFilter]
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authenticationService;  
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly HttpContextBase _httpContext;
        private readonly IAddressService _addressService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IZipCodeService _zipCodeService;

        public AccountController(ICustomerService customerService, IWorkContext workContext, HttpContextBase httpContext, IAuthenticationService authenticationService, IWebHelper webHelper
            , IAddressService addressService
            , IWorkflowMessageService workflowMessageService
            , IZipCodeService zipCodeService)
        {
            this._customerService = customerService;
            this._workContext = workContext;
            this._authenticationService = authenticationService;
            this._webHelper = webHelper;
            this._httpContext = httpContext;
            this._addressService = addressService;
            this._workflowMessageService = workflowMessageService;
            this._zipCodeService = zipCodeService;
        }



        protected void PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer)
        {
            model.Id = customer.Id;
            model.FirstName = customer.FirstName;
            model.LastName = customer.LastName;
            model.Email = customer.Email;
            model.ZipPostalCode = customer.ZipPostalCode;
            model.PhoneNumber = customer.PhoneNumber;
            model.City = customer.City;
            model.Address1 = customer.Address1;
            model.Address2 = customer.Address2;
            
        }

        protected void PrepareModel(AddressModel model, Address address)
        {
            if (address != null)
            {
                model.Id = address.Id;
                model.Address1 = address.Address1;
                model.Address2 = address.Address2;
                model.City = address.City;
                model.Email = _workContext.CurrentCustomer.Email; // address.Email;
                model.PhoneNumber = address.PhoneNumber;
                model.StateProvinceName = address.State;
                model.ZipPostalCode = address.ZipPostalCode;
            }
        }
        

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var loginResult = _customerService.ValidateCustomer( model.Email, model.Password);
            switch (loginResult)
            {
                case CustomerLoginResults.Successful:
                    {
                        var customer =  _customerService.GetCustomerByEmail(model.Email);

                        //migrate shopping cart
                        _workContext.CurrentCustomer=customer;

                        //sign in new customer
                        _authenticationService.SignIn(customer, model.RememberMe);

                        customer.LastActivityDateUtc = DateTime.UtcNow;
                        customer.LastLoginDateUtc = DateTime.UtcNow;

                        _customerService.UpdateCustomer(customer);

                        if (String.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                            return RedirectToRoute("GutterCleanRequest");

                        return Redirect(returnUrl);
                    }
                case CustomerLoginResults.CustomerNotExist:
                    ModelState.AddModelError("", "Account Not Exist");
                    break;
                
                case CustomerLoginResults.NotActive:
                    ModelState.AddModelError("", "Account Not Active");
                    break;
                case CustomerLoginResults.NotRegistered:
                    ModelState.AddModelError("", "Not Registered");
                    break;
                case CustomerLoginResults.WrongPassword:
                default:
                    ModelState.AddModelError("", "Wrong Credentials");
                    break;
            }

            //If we got this far, something failed, redisplay form            
            return View(model);

        }


        //
        // GET: /Account/Login
        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult AjaxLogin(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public virtual string RenderPartialViewToString(string viewName, object model)
        {
            //Original source code: http://craftycodeblog.com/2010/05/15/asp-net-mvc-render-partial-view-to-string/
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.RouteData.GetRequiredString("action");

            this.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }

        }

        [HttpPost]
        [ActionName("AjaxLogin")]
        [AllowAnonymous]        
        public ActionResult AjaxLogin(AjaxLoginViewModel model, string returnUrl)
        {

            try
            {
                if (!ModelState.IsValid)
                {

                    if(!Request.IsAjaxRequest())
                        return View();

                    return Json(new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "login-form",
                            html = this.RenderPartialViewToString("AjaxLogin", model)
                        }
                       // goto_section = "shipping_method"
                    });
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, change to shouldLockout: true
                var loginResult = _customerService.ValidateCustomer(model.AjaxEmail, model.AjaxPassword);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerService.GetCustomerByEmail(model.AjaxEmail);

                            //migrate shopping cart
                            _workContext.CurrentCustomer = customer;

                            customer.LastActivityDateUtc = DateTime.UtcNow;
                            customer.LastLoginDateUtc = DateTime.UtcNow;

                            _customerService.UpdateCustomer(customer);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.AjaxRememberMe);

                            GutterCleanRequestModel Requestmodel = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
                            if (Requestmodel == null)
                            {
                                return Json(new { redirect = Url.RouteUrl("GutterCleanRequest") });                                
                            }
                            else
                            {
                                return Json(new { redirect = Url.RouteUrl("ProcessPayment") });
                            }

                           
                            
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", "Customer Not Exist");
                        break;

                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", "Customer Not Active");
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", "Not Registered");
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", "Wrong Credentials");
                        break;
                }

                //If we got this far, something failed, redisplay form            
                return Json(new { error = 1, message ="" });
            }
            catch (Exception exc)
            {               
                return Json(new { error = 1, message = exc.Message });
            }

            return View();

        }



        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            //GutterCleanRequestModel model = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
            //if (model == null)
            //{
            //    return RedirectToRoute("GutterCleanRequest");
            //}

            RegisterViewModel modelRegister = new RegisterViewModel();
            return View(modelRegister);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ActionName("Register")]        
        public ActionResult Register(RegisterViewModel model)
        {

            string returnUrl = string.Empty;

            if (ModelState.IsValid)
            {

                if (_workContext.CurrentCustomer.IsRegistered())
                {
                    //Already registered customer. 
                    _authenticationService.SignOut();

                    //Save a new record
                    _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
                }
                var customer = _workContext.CurrentCustomer;
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                
                

                bool isApproved = true;
                var registrationRequest = new CustomerRegistrationRequest(customer, model.Email, model.Password,isApproved);
                
                var registrationResult = _customerService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //form fields
                    


                    //notifications
                    //if (_customerSettings.NotifyNewCustomerRegistration)
                    //    _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer, _localizationSettings.DefaultAdminLanguageId);

                    switch (UserRegistrationType.Standard)
                    {
                        
                        case UserRegistrationType.Standard:
                            {
                                ////send customer welcome message
                                //_workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                //var redirectUrl = Url.RouteUrl("RegisterResult", new { resultId = (int)UserRegistrationType.Standard });
                                //if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                //    redirectUrl = _webHelper.ModifyQueryString(redirectUrl, "returnurl=" + HttpUtility.UrlEncode(returnUrl), null);
                                //return Redirect(redirectUrl);

                                //send email
                                _workflowMessageService.SendCustomerWelcomeMessage(customer);

                                GutterCleanRequestModel Requestmodel = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
                                if (Requestmodel == null)
                                {
                                    return RedirectToRoute("GutterCleanRequest");
                                }
                                else
                                {

                                    return RedirectToRoute("ProcessPayment");
                                }
                            }
                        default:
                            {
                                return RedirectToRoute("GutterCleanRequest");
                                
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }



        [ChildActionOnly]
        public ActionResult CustomerNavigation(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();          

            model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return PartialView(model);
        }

        public ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerInfoModel();
            PrepareCustomerInfoModel(model, customer);

            return View(model);
        }

        [HttpPost]        
        [ValidateInput(false)]
        public ActionResult Info(CustomerInfoModel model, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            
            try
            {
                if (ModelState.IsValid)
                {
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;
                    customer.PhoneNumber = model.PhoneNumber;
                    customer.Address1 = model.Address1;
                    customer.Address2 = model.Address2;
                    customer.City = model.City;
                    customer.ZipPostalCode = model.ZipPostalCode;

                    _customerService.UpdateCustomer(customer);
                   
                    return RedirectToRoute("CustomerInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }          


            return View(model);
        }

        #region My account / Addresses

       
        public ActionResult Addresses()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAddressListModel();
            var addresses = customer.Addresses                
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                PrepareModel(
                    model:addressModel,
                    address: address);
                model.Addresses.Add(addressModel);
            }
            return View(model);
        }

        
        public ActionResult AddressDelete(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address != null)
            {
                customer.RemoveAddress(address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            return RedirectToRoute("CustomerAddresses");
        }

        
        public ActionResult AddressAdd()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var model = new AddressModel();
            PrepareModel(model:model,
                address: null
                );

            model.Email = _workContext.CurrentCustomer.Email;
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddressAdd(AddressModel model, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {

                
                var zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(model.ZipPostalCode);
                

                var address = new Address();
               
                address.Address1 = model.Address1;
                address.Address2 = model.Address2;
                
                address.Email = model.Email;

                if (zipcodeResult != null)
                {
                    address.City = model.City;
                    address.State = model.StateProvinceName;
                }

                
                address.PhoneNumber = model.PhoneNumber;
                address.ZipPostalCode = model.ZipPostalCode;
                address.CreatedOnUtc = DateTime.UtcNow;

                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form         

            return View(model);
        }

        
        public ActionResult AddressEdit(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");


            var model = new AddressModel();
            PrepareModel(model: model,
                address: address
                );

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddressEdit(AddressModel model, int addressId, FormCollection form)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            

            if (ModelState.IsValid)
            {
                var zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(model.ZipPostalCode);

                address.Address1 = model.Address1;
                address.Address2 = model.Address2;

                if (zipcodeResult != null)
                {
                    address.City = model.City;
                    address.State = model.StateProvinceName;
                }

                
                address.Email = model.Email;                
                address.PhoneNumber = model.PhoneNumber;
                address.ZipPostalCode = model.ZipPostalCode;
                _addressService.UpdateAddress(address);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form            
            return View(model);
        }

        #endregion

        public ActionResult ChangePassword()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            ChangePasswordViewModel model = new ChangePasswordViewModel();
            return View(model);
        }

        [HttpPost]       
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
                var customer = _workContext.CurrentCustomer;
                customer.Password = model.Password;

                _customerService.UpdateCustomer(customer);

                return RedirectToRoute("CustomerInfo");
            }
            
            return View(model);
        }


        ////
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel();
            return View(model);
        }


        ////
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var customer = _customerService.GetCustomerByEmail(model.Email);
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid().ToString();
                    customer.PasswordRecoveryToken = passwordRecoveryToken;
                    _customerService.UpdateCustomer(customer);

                    //send email
                    _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer);

                    model.Result = "Email with instructions has been sent to you. ";
                }
                else
                {
                    model.Result = "Email not found. ";
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var model = new ResetPasswordViewModel();

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.Result = "Wrong Token.";
            }

           

            return View(model);
        }

        
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(string token, string email, ResetPasswordViewModel model)
        {

            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            //validate token
            if (!customer.IsPasswordRecoveryTokenValid(token))
            {
                model.DisablePasswordChanging = true;
                model.Result = "Wrong Token.";
            }


            if (ModelState.IsValid)
            {
               
                
                    customer.Password = model.ConfirmPassword;
                    customer.PasswordRecoveryToken = "";
                    _customerService.UpdateCustomer(customer);

                    model.DisablePasswordChanging = true;
                    model.Result = "Password Has Been Changed.";

                    return RedirectToAction("ResetPasswordConfirmation", "Account");
               

                //return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation


        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        

       

        

        
         //POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
           
            _authenticationService.SignOut();
            return RedirectToRoute("GutterCleanRequest");
        }

        

        
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}