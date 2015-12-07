using EGSW.Data;
using EGSW.Data.Customers;
using EGSW.Services;
using EGSW.Services.Payments;
using EGSW.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//using EGSW.Web.PayPalPaymentService;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using EGSW.Services.Orders;
using EGSW.Web.Models.Orders;
using EGSW.Services.Directory;
using EGSW.Services.Authentication;
using EGSW.Framework;
using EGSW.Web.ActionFilters;

namespace EGSW.Web.Controllers
{
    [RequreSecureConnectionFilter]
    public class HomeController : Controller
    {
        private readonly IDbContext _context;
        private readonly IAuthenticationService _authenticationService;  
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IQuestionAnswerEntityData _questionAnswerEntityDataService;
        private readonly HttpContextBase _httpContext;
        private readonly IPaymentMethod  _paymentMethod;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IZipCodeService _zipCodeService;
        private readonly IDbContext _dbContext;

        public HomeController(IDbContext context, IAuthenticationService authenticationService, IWorkContext workContext, ICustomerService customerService,
            IQuestionAnswerEntityData questionAnswerEntityDataService, HttpContextBase httpContext,
            IPaymentMethod paymentMethod, IOrderService orderService, IWebHelper webHelper,
            IWorkflowMessageService workflowMessageService, IZipCodeService zipCodeService,
            IDbContext dbContext)
        {
            this._context = context;
            this._authenticationService = authenticationService;
            this._customerService = customerService;
            this._workContext = workContext;
            this._questionAnswerEntityDataService = questionAnswerEntityDataService;
            this._httpContext = httpContext;
            this._paymentMethod = paymentMethod;
            this._orderService = orderService;
            this._webHelper = webHelper;
            this._workflowMessageService = workflowMessageService;
            this._zipCodeService = zipCodeService;
            this._dbContext = dbContext;
        }

        protected void PrepareGutterCleanOrderModel(GutterCleanRequestModel model)
        {
            //model.AvailableRoofMaterial=_questionAnswerEntityDataService.

            model.AvailableRoofMaterial=_questionAnswerEntityDataService.RoofMaterialAnswer();
            model.AvailableSquareFootage = _questionAnswerEntityDataService.SquareFootageAnswer();
            model.AvailableStyleOfGutter = _questionAnswerEntityDataService.StyleOfGuttersAnswer();
            model.AvailableYearBuilt = _questionAnswerEntityDataService.YearBuiltAnswer();
            model.AvailableDeliveryTime = _questionAnswerEntityDataService.DeliveryTimeAnswer();
        }


        public ActionResult Index()
        {
            

            var customer = _workContext.CurrentCustomer;

            ////_customerService.InsertGuestCustomer();

            ////Customer customer = new Customer();
            ////customer.FirstName = "Nitendra";
            ////customer.LastName = "Jain";
            ////customer.IsSystemAccount = false;
            ////customer.LastLoginDateUtc = DateTime.UtcNow;
            //// customer.CustomerGuid = Guid.NewGuid();
            ////    customer.Active = true;
            ////    customer.CreatedOnUtc = DateTime.UtcNow;
            ////    customer.LastActivityDateUtc = DateTime.UtcNow;

            ////    _customerService.InsertCustomer(customer);

          //var listResult=  _customerService.GetAllCustomers();
        
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ContactUsModel model = new ContactUsModel();

            return View(model);
        }

        [HttpPost]
        [CaptchaValidator]
        public ActionResult Contact(ContactUsModel model,bool captchaValid)
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("", "Wrong Captcha code.");
            }

            if (ModelState.IsValid)
            {
                model.Result = true;
                ContactU entity = new ContactU();
                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;
                entity.Email = model.Email;
                entity.PhoneNo = model.PhoneNo;
                entity.Message = model.Message;
                entity.CreatedOnUtc = DateTime.UtcNow;
                _customerService.InsertContactUs(entity);

                _workflowMessageService.SendContactUsMessage(entity);
            }

            return View(model);
        }


        public ActionResult GutterCleanRequest()
        {
            //if ((_workContext.CurrentCustomer.IsGuest()))
            //    return new HttpUnauthorizedResult();

            _dbContext.ExecuteSqlCommand("EXEC dbo.[DeleteGuests]");

            GutterCleanRequestModel model = new GutterCleanRequestModel();
            PrepareGutterCleanOrderModel(model);
            return View(model);
        }

        [HttpPost]
        public ActionResult GutterCleanRequest(GutterCleanRequestModel model)
        {

             if (ModelState.IsValid)
            {
                decimal totalAmount = 0;
                totalAmount = _questionAnswerEntityDataService.CalculateCost(model.QuestionSquareFootage, model.QuestionYearBuilt, Convert.ToInt32(model.QuestionStyleOfGutter), model.RoofMaterial, model.QuestionDeliveryTime);
                model.OrderTotal = Math.Round(totalAmount);
                _httpContext.Session["GutterCleanRequestModel"] = model;

                return RedirectToRoute("ProcessPayment");
            }

            PrepareGutterCleanOrderModel(model);
            return View(model);
        }

        public ActionResult GenerateQuote(GutterCleanRequestModel model)
        {

            if (ModelState.IsValid)
            {
                decimal totalAmount = 0;
                totalAmount = _questionAnswerEntityDataService.CalculateCost(model.QuestionSquareFootage, model.QuestionYearBuilt, Convert.ToInt32(model.QuestionStyleOfGutter), model.RoofMaterial,model.QuestionDeliveryTime);
                return Json(new { TotalAmount = Math.Round(totalAmount), Success = true, message = string.Empty });
            }

            string message = string.Empty;
            var query = from state in ModelState.Values
                        from error in state.Errors
                        select error.ErrorMessage;

            return Json(new { TotalAmount = 0, Success = false, message = query.ToArray() });
            
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult GutterCleanInfoRegister()
        {
            GutterCleanRequestModel requestModel = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
            if (requestModel == null)
            {
                return RedirectToRoute("GutterCleanRequest");
            }

            GutterCleanInfoModel model = new GutterCleanInfoModel();
            return View(model);
        }

        public bool addAddress(GutterCleanOrder entity)
        {
             var customer = _workContext.CurrentCustomer;

           // var zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(entity.Zipcode);


            var address = new Address();

            address.Address1 = entity.Address;
            //address.Address2 = entity.Address2;

            //address.Email = model.Email;
/*
            if (zipcodeResult != null)
            {
                address.City = model.City;
                address.State = model.StateProvinceName;
            }


            address.PhoneNumber = model.PhoneNumber; */
            address.ZipPostalCode = entity.Zipcode;
            address.CreatedOnUtc = entity.CreatedOnUtc;//DateTime.UtcNow;

            customer.Addresses.Add(address);
            _customerService.UpdateCustomer(customer);

            return true;

        }

        //
        // POST: /Account/GutterCleanInfoRegister
        [HttpPost]
        [CaptchaValidator]
        [ActionName("GutterCleanInfoRegister")]
        public ActionResult GutterCleanInfoRegister(GutterCleanInfoModel model, bool captchaValid)
        {
            GutterCleanRequestModel requestModel = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
            if (requestModel == null)
            {
                return RedirectToRoute("GutterCleanRequest");
            }


            string returnUrl = string.Empty;

            if (!captchaValid)
            {
                ModelState.AddModelError("", "Wrong Captcha code.");
            }

            var zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(model.ZipCode);
            if (zipcodeResult != null)
            {
                
            }
            else
            {
                ModelState.AddModelError("", "Zipcode is not valid.");
               
            }


            if (ModelState.IsValid)
            {

                if (!_workContext.CurrentCustomer.IsRegistered())
                {
                    //Already registered customer. 
                    _authenticationService.SignOut();

                    //Save a new record
                    _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();

                    var customer = _workContext.CurrentCustomer;
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;
                    
                    customer.Address1 = model.Address;
                    customer.ZipPostalCode = model.ZipCode;


                    bool isApproved = true;
                    string password = Guid.NewGuid().ToString();
                    var registrationRequest = new CustomerRegistrationRequest(customer, model.Email, password.Substring(0, 8), isApproved);

                    var registrationResult = _customerService.RegisterCustomer(registrationRequest);
                    if (registrationResult.Success)
                    {

                        //login customer now
                        if (isApproved)
                            _authenticationService.SignIn(customer, true);

                        
                        switch (UserRegistrationType.Standard)
                        {

                            case UserRegistrationType.Standard:
                                {

                                    var address = new Address();                                    
                                    address.Address1 = model.Address;
                                    address.Address2 = "";
                                    address.City = zipcodeResult.CityName;
                                    address.Email = model.Email;
                                    address.State = zipcodeResult.StateName;
                                    address.PhoneNumber = "";
                                    address.ZipPostalCode = model.ZipCode;
                                    address.CreatedOnUtc = DateTime.UtcNow;

                                    customer.Addresses.Add(address);
                                    _customerService.UpdateCustomer(customer);

                                    
                                    
                                    //send email
                                    //_workflowMessageService.SendCustomerWelcomeMessage(customer);

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
                else
                {
                    /// user is already registered
                    return RedirectToRoute("ProcessPayment");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        
        public ActionResult ProcessPayment()
        {
            var customer = _workContext.CurrentCustomer;
            if ((_workContext.CurrentCustomer.IsGuest()))
                return RedirectToRoute("GutterCleanInfoRegister");               
                //return new HttpUnauthorizedResult();

            GutterCleanRequestModel model = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
            if (model == null)
            {
                return RedirectToRoute("GutterCleanRequest");
                
            }

            GutterCleanPaymentRequestModel paymentRequestmodel = new GutterCleanPaymentRequestModel();

            //paymentRequestmodel.Address = customer.Address1;
            //paymentRequestmodel.Zipcode = customer.ZipPostalCode;
            paymentRequestmodel.OrderTotal = model.OrderTotal;

//            select top (1) *  from GutterCleanOrder where CustomerId='998' 
//order by CreatedOnUtc desc

            var AddressToShow = customer.GutterCleanOrders.OrderByDescending(n => n.CreatedOnUtc).First();
            //take(1)

           // var AddressToShow = customer.GutterCleanOrders.ToList();

            String AddressShow = string.Empty;

            //foreach (var address in AddressToShow)
            {
                AddressShow = string.Empty;
                AddressShow = AddressToShow.Address + ", " + AddressToShow.City + "," + AddressToShow.State + "," + AddressToShow.Zipcode;
                paymentRequestmodel.AvailableAddress.Add(new SelectListItem
                {
                    Text = AddressShow,
                    Value = AddressToShow.Id.ToString(),
                });
            }

            var AddressList = customer.Addresses.ToList();

            string first_add = string.Empty; ;

            string AddressText = string.Empty;
            foreach (var address in AddressList)
            {
                 AddressText = string.Empty;
                
                 AddressText = address.Address1 + ", " + address.City + "," + address.State + "," + address.ZipPostalCode;

                 if (address.Address1.Equals(AddressToShow.Address) && (address.ZipPostalCode == AddressToShow.Zipcode))
                 {
                     paymentRequestmodel.AvailableAddress[0].Value = address.Id.ToString();
                     first_add = "true"; continue; 
                 }
                 paymentRequestmodel.AvailableAddress.Add(new SelectListItem
                     {
                         Text = AddressText,
                         Value = address.Id.ToString(),
                     });
            }

            if (first_add == "" || first_add != "true")
            {
                paymentRequestmodel.AvailableAddress.Remove(paymentRequestmodel.AvailableAddress[0]);
            }

            paymentRequestmodel.AvailableAddress.Add(new SelectListItem
            {
                Text = "New Address",
                Value = "-1",
            });


            //CC types
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Visa",
                Value = "Visa",
            });
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Master card",
                Value = "MasterCard",
            });
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Discover",
                Value = "Discover",
            });
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Amex",
                Value = "Amex",
            });

            //years            
            paymentRequestmodel.ExpireYears.Add(new SelectListItem
            {
                Text = "Year",
                Value = "",
            });
            for (int i = 0; i < 25; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                paymentRequestmodel.ExpireYears.Add(new SelectListItem
                {
                    Text = year,
                    Value = year,
                });
            }

            //months
            //months
            paymentRequestmodel.ExpireMonths.Add(new SelectListItem
            {
                Text = "Month",
                Value = "",
            });
            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i : i.ToString();
                paymentRequestmodel.ExpireMonths.Add(new SelectListItem
                {
                    Text = text,
                    Value = i.ToString(),
                });
            }

            //prepare review Parameter
            paymentRequestmodel.QuestionSquareFootage = model.QuestionSquareFootage;

            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == model.QuestionStyleOfGutter).SingleOrDefault();
            paymentRequestmodel.QuestionStyleOfGutterStr = r.Text;

           

            var yearRecord = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == model.QuestionYearBuilt.ToString()).SingleOrDefault();
            paymentRequestmodel.QuestionYearBuiltStr = yearRecord.Text;

            paymentRequestmodel.RoofMaterial = model.RoofMaterial;

            paymentRequestmodel.QuestionDeliveryTimeStr = "5 business days";
            if (model.QuestionDeliveryTime == 1)
                paymentRequestmodel.QuestionDeliveryTimeStr = "5 business days";

            if (model.QuestionDeliveryTime == 2)
                paymentRequestmodel.QuestionDeliveryTimeStr = "8 hours";

            if (model.QuestionDeliveryTime == 3)
                paymentRequestmodel.QuestionDeliveryTimeStr = "4 hours";

            var zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(customer.ZipPostalCode);
            //paymentRequestmodel.Address = customer.Address1;

            paymentRequestmodel.AddressService = customer.Address1;
            if (zipcodeResult != null)
            {
                paymentRequestmodel.AddressService = customer.Address1+", "  + zipcodeResult.CityName+", " + zipcodeResult.StateName  ;
            }
            paymentRequestmodel.zipcodeService = customer.ZipPostalCode;

            return View(paymentRequestmodel);
        }

        [HttpPost]
        public ActionResult ProcessPayment(GutterCleanPaymentRequestModel paymentRequestmodel)
        {
            var customer = _workContext.CurrentCustomer;
            if ((_workContext.CurrentCustomer.IsGuest()))
                return RedirectToRoute("GutterCleanInfoRegister");
            //return new HttpUnauthorizedResult();

            //GutterCleanRequestModel requestModel = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
            GutterCleanRequestModel model = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
            if (model == null)
            {
                return RedirectToRoute("GutterCleanRequest");
            }

            ZipCode zipcodeResult;

           // int addAddress_flag = 0;

            if (paymentRequestmodel.SelectedAddressId > 0)
            {
                var selectedAddress = customer.Addresses.Where(a => a.Id == paymentRequestmodel.SelectedAddressId).SingleOrDefault();
                zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(selectedAddress.ZipPostalCode);
                if (zipcodeResult != null)
                {
                }
                else
                {
                    ModelState.AddModelError("Zipcode", "Selected Address Zipcode is not valid. Please select Another Addresss Or Enter New Address.");
                }
                
            }
            else
            {
                zipcodeResult = _zipCodeService.GetZipCodeDetailByZipcode(paymentRequestmodel.Zipcode);
                if (zipcodeResult != null)
                {
                }
                else
                {
                    ModelState.AddModelError("Zipcode", "Zipcode is not valid.");
                }
            }



            if (ModelState.IsValid)
            {
                //GutterCleanRequestModel model = _httpContext.Session["GutterCleanRequestModel"] as GutterCleanRequestModel;
                if (model == null)
                {
                    return RedirectToRoute("GutterCleanRequest");
                }
                paymentRequestmodel.OrderTotal = model.OrderTotal;
                ProcessPaymentRequest processPaymentRequest = new ProcessPaymentRequest();
                processPaymentRequest.OrderTotal = model.OrderTotal;

                processPaymentRequest.CreditCardName = paymentRequestmodel.NameOnCard;
                processPaymentRequest.CreditCardNumber = paymentRequestmodel.CardNumber;
                processPaymentRequest.CreditCardCvv2 = paymentRequestmodel.CardSecurityCode;
                processPaymentRequest.CreditCardExpireMonth = paymentRequestmodel.CardExpiryMonth;
                processPaymentRequest.CreditCardExpireYear = paymentRequestmodel.CardExpiryYear;
                
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                if (processPaymentRequest.OrderGuid == Guid.Empty)
                    processPaymentRequest.OrderGuid = Guid.NewGuid();

                try
                {
                    ProcessPaymentResult processPaymentResult = null;
                    processPaymentResult = _paymentMethod.ProcessPayment(processPaymentRequest);


                    if (processPaymentResult.Success)
                    {
                        var zipcodeDetail = _zipCodeService.GetZipCodeDetailByZipcode(paymentRequestmodel.Zipcode);


                        GutterCleanOrder entity = new GutterCleanOrder();
                        entity.CustomerId = _workContext.CurrentCustomer.Id;
                        entity.CustomerIp = _webHelper.GetCurrentIpAddress();
                        entity.OrderStatusId = (int)OrderStatus.Processing;
                        entity.PaymentStatusId = (int)PaymentStatus.Paid;
                        entity.CaptureTransactionId = processPaymentResult.CaptureTransactionId;
                        entity.CaptureTransactionResult = processPaymentResult.CaptureTransactionResult;

                        entity.QuestionSquareFootage = model.QuestionSquareFootage;
                        entity.QuestionStyleOfGutter = model.QuestionStyleOfGutter;
                        entity.QuestionYearBuilt = model.QuestionYearBuilt;
                        entity.RoofMaterial = model.RoofMaterial;
                        entity.QuestionDeliveryTime = model.QuestionDeliveryTime;

                        entity.OrderTotal = model.OrderTotal;
                        entity.OrderGuid = processPaymentRequest.OrderGuid;

                        if (paymentRequestmodel.SelectedAddressId > 0)
                        {
                            var selectedAddress = customer.Addresses.Where(a => a.Id == paymentRequestmodel.SelectedAddressId).SingleOrDefault();

                            entity.Address = selectedAddress.Address1;
                            entity.Zipcode = selectedAddress.ZipPostalCode;

                            zipcodeDetail = _zipCodeService.GetZipCodeDetailByZipcode(entity.Zipcode);

                            if (zipcodeDetail != null)
                            {
                                entity.City = zipcodeDetail.CityName; ;
                                entity.State = zipcodeDetail.StateName;
                            }
                        }
                        else
                        {
                            entity.Address = paymentRequestmodel.Address;
                            entity.Zipcode = paymentRequestmodel.Zipcode;

                            if (zipcodeDetail != null)
                            {
                                entity.City = zipcodeDetail.CityName; ;
                                entity.State = zipcodeDetail.StateName;
                            }
                        }

                        

                        entity.AgentId = 0;
                        entity.WorkerId = 0;
                        entity.AgentStatusId = 0;
                        entity.WorkerStatusId = 0;
                        entity.CreatedOnUtc = DateTime.UtcNow;
                        //entity.LastUpdatedDateUtc = DateTime.UtcNow;
                        _orderService.InsertOrder(entity);

                        // add new address if user enter new address which is not in its address list
                        if (paymentRequestmodel.SelectedAddressId <= 0)
                        {
                            addAddress(entity);
                        }

                        _httpContext.Session["GutterCleanRequestModel"] = null;

                        _workflowMessageService.SendOrderPlacedCustomerNotification(entity);
                        _workflowMessageService.SendOrderPlacedSiteOwnerNotification(entity);

                        return RedirectToRoute("Completed", new { orderId = entity.Id });

                    }

                    foreach (var error in processPaymentResult.Errors)
                        paymentRequestmodel.Warnings.Add(error);
                }
                catch (Exception exc)
                {
                   
                    paymentRequestmodel.Warnings.Add(exc.Message);
                }

                // return View(paymentRequestmodel);
            }


            // Model is not valid then redisplay model

            var AddressList = customer.Addresses.ToList();
            string AddressText = string.Empty;
            foreach (var address in AddressList)
            {
                AddressText = string.Empty;
                AddressText = address.Address1 + ", " + address.City + "," + address.State + "," + address.ZipPostalCode;
                paymentRequestmodel.AvailableAddress.Add(new SelectListItem
                {
                    Text = AddressText,
                    Value = address.Id.ToString(),
                });
            }
            paymentRequestmodel.AvailableAddress.Add(new SelectListItem
            {
                Text = "New Address",
                Value = "0",
            });


            //CC types
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Visa",
                Value = "Visa",
            });
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Master card",
                Value = "MasterCard",
            });
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Discover",
                Value = "Discover",
            });
            paymentRequestmodel.CreditCardTypes.Add(new SelectListItem
            {
                Text = "Amex",
                Value = "Amex",
            });

            //years
            paymentRequestmodel.ExpireYears.Add(new SelectListItem
            {
                Text = "Year",
                Value = "",
            });
            for (int i = 0; i < 25; i++)
            {
                string year = Convert.ToString(DateTime.Now.Year + i);
                paymentRequestmodel.ExpireYears.Add(new SelectListItem
                {
                    Text = year,
                    Value = year,
                });
            }

            //months
            paymentRequestmodel.ExpireMonths.Add(new SelectListItem
            {
                Text = "Month",
                Value = "",
            });
            for (int i = 1; i <= 12; i++)
            {
                string text = (i < 10) ? "0" + i : i.ToString();
                paymentRequestmodel.ExpireMonths.Add(new SelectListItem
                {
                    Text = text,
                    Value = i.ToString(),
                });
            }

            //prepare review Parameter
            paymentRequestmodel.QuestionSquareFootage = model.QuestionSquareFootage;

            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == model.QuestionStyleOfGutter).SingleOrDefault();
            paymentRequestmodel.QuestionStyleOfGutterStr = r.Text;



            var yearRecord = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == model.QuestionYearBuilt.ToString()).SingleOrDefault();
            paymentRequestmodel.QuestionYearBuiltStr = yearRecord.Text;

            paymentRequestmodel.RoofMaterial = model.RoofMaterial;

            paymentRequestmodel.QuestionDeliveryTimeStr = "5 business days";
            if (model.QuestionDeliveryTime == 1)
                paymentRequestmodel.QuestionDeliveryTimeStr = "5 business days";

            if (model.QuestionDeliveryTime == 2)
                paymentRequestmodel.QuestionDeliveryTimeStr = "8 hours";

            if (model.QuestionDeliveryTime == 3)
                paymentRequestmodel.QuestionDeliveryTimeStr = "4 hours";

            var zipcodeResultCustomer = _zipCodeService.GetZipCodeDetailByZipcode(customer.ZipPostalCode);
            paymentRequestmodel.Address = customer.Address1;

            paymentRequestmodel.AddressService = customer.Address1;
            if (zipcodeResult != null)
            {
                paymentRequestmodel.AddressService = customer.Address1 + ", " + zipcodeResultCustomer.CityName + ", " + zipcodeResultCustomer.StateName;
            }
            paymentRequestmodel.zipcodeService = customer.ZipPostalCode;


            return View(paymentRequestmodel);
        }

        public ActionResult Completed(int? orderId)
        {
            ////validation
            //if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
            //    return new HttpUnauthorizedResult();

            GutterCleanOrder order = null;
            if (orderId.HasValue)
            {
                //load order by identifier (if provided)
                order = _orderService.GetOrderById(orderId.Value);
            }
            
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
            {
                return RedirectToRoute("HomePage");
            }

            

            //model
            var model = new CheckoutCompletedModel
            {
                OrderId = order.Id,
                
            };

            model.QuestionDeliveryTimeStr = "5 business days";

            if (order.QuestionDeliveryTime == 1)
                model.QuestionDeliveryTimeStr = "5 business days";

            if(order.QuestionDeliveryTime==2)
                model.QuestionDeliveryTimeStr = "8 hours";

            if (order.QuestionDeliveryTime == 3)
                model.QuestionDeliveryTimeStr = "4 hours";


            return View(model);
        }


        
    }
}