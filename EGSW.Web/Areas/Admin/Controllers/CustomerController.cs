using EGSW.Data;
using EGSW.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGSW.Data.Customers;
using EGSW.Web.Areas.Admin.Models.Customers;
using EGSW.Framework;
using EGSW.Services.Orders;

namespace EGSW.Web.Areas.Admin.Controllers
{
    [AdminRoleAuthorizeAttribute]
    public class CustomerController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;

        public CustomerController(ICustomerService customerService, IWorkContext workContext, IOrderService orderService)
        {
            this._customerService = customerService;
            this._workContext = workContext;
            this._orderService = orderService;
        }

        [NonAction]
        protected virtual void PrepareCustomerModel(CustomerModel model, Customer customer, bool excludeProperties)
        {
            ViewBag.DisplayText = "Customer";
            string CustomerRoleName = "";

            if (customer != null)
            {
                model.Id = customer.Id;
                if (!excludeProperties)
                {
                    model.FirstName = customer.FirstName;
                    model.LastName = customer.LastName;
                    model.PhoneNumber = customer.PhoneNumber;
                    model.Email = customer.Email;
                    model.Address1 = customer.Address1;
                    model.Address2 = customer.Address2;
                    model.City = customer.City;
                    model.ZipPostalCode = customer.ZipPostalCode;
                    model.CreatedOnUtc = customer.CreatedOnUtc;
                    model.Active = customer.Active;
                    model.SelectedCustomerRoleId = customer.CustomerRoles.Select(cr => cr.Id).SingleOrDefault();
                    CustomerRoleName = customer.CustomerRoles.Select(cr=>cr.Name).SingleOrDefault();


                    if (CustomerRoleName == "Administrators")
                    {
                        ViewBag.DisplayText = "Administrator";
                    }
                    else if (CustomerRoleName == "Agent")
                    {
                        ViewBag.DisplayText = "Agent";
                    }
                }
            }

            //customer roles
            foreach (var c in _customerService.GetAllCustomerRoles())
            {
                model.AvailableCustomerRoles.Add(new CustomerRoleModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    SystemName = c.SystemName
                });
            }

            // Prepare Rating Properties.
            if (_workContext.CurrentCustomer.IsAdmin() || _workContext.CurrentCustomer.IsAgent())
            {
                PrepareAverageRatingForUser(model, CustomerRoleName);
            }
           

        }


        protected virtual void PrepareAverageRatingForUser(CustomerModel model, string role)
        {
            if (role == "Administrators" || role == "Agent")
            {
                int pageSize = 10;
                int pageIndex = 1;
                //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

                var OrderList = _orderService.SearchOrders(agentId: model.Id, pageIndex: pageIndex, pageSize: pageSize);

                if (OrderList.Count > 0)
                {
                    var OrderRatingList = OrderList.Where(o => o.IsCustomerQa.HasValue && o.IsCustomerQa.Value == true);

                    if (OrderRatingList.Count() > 0)
                    {
                        //model.Question1 = (OrderRatingList.Select(o => o.Surveries.FirstOrDefault()).Sum(s => s.Question1) / OrderRatingList.Select(o => o.Surveries.FirstOrDefault()).Count()).ToString();
                        model.Question2 = (OrderRatingList.Select(o => o.Surveries.FirstOrDefault()).Sum(s => s.Question2) / OrderRatingList.Select(o => o.Surveries.FirstOrDefault()).Count()).ToString();
                        model.Question3 = (OrderRatingList.Select(o => o.Surveries.FirstOrDefault()).Sum(s => s.Question3) / OrderRatingList.Select(o => o.Surveries.FirstOrDefault()).Count()).ToString();

                        model.ShowRating = true;
                    }
                    else
                    {
                        model.ShowRating = false;
                    }
                }
            }
            else
            {
                model.ShowRating = false;
            }
        }

        // GET: Admin/Customer
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Admin/CustomerList
        public ActionResult List(string FirstName,string LastName, string CustomerEmail, string ZipCode, int? customerId, int? page)
        {
            //if (!_workContext.CurrentCustomer.IsRegistered())
            //    return new HttpUnauthorizedResult();
           

            // prepare search parameter
            ViewBag.customerIdData = customerId;
            ViewBag.FirstNameData = FirstName;
            ViewBag.LastNameData = LastName;
            ViewBag.CustomerEmailData = CustomerEmail;
            ViewBag.ZipCodeData = ZipCode;

            int pageSize = 50;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var customer = _customerService.GetAllCustomers(customerId:customerId, firstName:FirstName,lastName:LastName, customerEmail: CustomerEmail, zipCode:ZipCode, pageIndex: pageIndex,
            pageSize: pageSize,customerRoleIds:new int[]{2});

            return View(customer);
        }


        // GET: Admin/Agent List
        public ActionResult AgentList(int? page)
        {
            //if (!_workContext.CurrentCustomer.IsRegistered())
            //    return new HttpUnauthorizedResult();

            int pageSize = 50;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var customer = _customerService.GetAllCustomers(pageIndex: pageIndex,
                pageSize: pageSize,customerRoleIds:new int[]{4,1});

            return View(customer);
        }



        // GET: Admin/Customer
        public ActionResult CreateCustomer()
        {           

            var model = new CustomerModel();
            PrepareCustomerModel(model, null, false);
            //default value
            model.Active = true;
            return View(model);

          
        }

        [HttpPost]
        public ActionResult CreateCustomer(CustomerModel model)
        {

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var cust2 = _customerService.GetCustomerByEmail(model.Email);
                if (cust2 != null)                    
                    ModelState.AddModelError("", "Email Address already exists.");
            }


            if (model.SelectedCustomerRoleId<=0)
            {               
               ModelState.AddModelError("", "Add the customer to 'Administrators' or 'Registered' customer role");
            }

            if (ModelState.IsValid)
            {
               
                   var entity = new Customer();
                    entity.CustomerGuid = Guid.NewGuid();
                    entity.FirstName = model.FirstName;
                    entity.LastName = model.LastName;
                    entity.Email = model.Email;
                    entity.PhoneNumber = model.PhoneNumber;
                    entity.Address1 = model.Address1;
                    entity.Address2 = model.Address2;
                    entity.City = model.City;
                    entity.Password = model.Password;
                    entity.ZipPostalCode = model.ZipPostalCode;
                    entity.Active = model.Active;
                    entity.CreatedOnUtc = DateTime.UtcNow;

                    _customerService.InsertCustomer(entity);

                    //validate customer roles
                    var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
                    var newCustomerRole = allCustomerRoles.Where(r => r.Id == model.SelectedCustomerRoleId).SingleOrDefault();

                    if (newCustomerRole != null)
                    {
                        entity.CustomerRoles.Add(newCustomerRole);
                        _customerService.UpdateCustomer(entity);
                    }
                    

                    return RedirectToAction("List");
                

                
            }

            //If we got this far, something failed, redisplay form
            PrepareCustomerModel(model, null, true);
            return View(model);
        }


        // GET: Admin/Customer
        public ActionResult EditCustomer(int Id)
        {
            var customer = _customerService.GetCustomerById(Id);
            if (customer == null || customer.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            var model = new CustomerModel();
            PrepareCustomerModel(model, customer, false);
            //ViewBag.DisplayText = "Customer";
            //if (customer.IsAdmin())
            //{
            //    ViewBag.DisplayText = "Administrator";
            //}
            //else if (customer.IsAgent())
            //{
            //    ViewBag.DisplayText = "Agent";
            //}
            return View(model);
        }


        [HttpPost]
        public ActionResult EditCustomer(CustomerModel model)
        {

            var entity = _customerService.GetCustomerById(model.Id);
            if (entity == null || entity.Deleted)
                //No customer found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {               
                entity.FirstName = model.FirstName;
                entity.LastName = model.LastName;


                if (!String.IsNullOrWhiteSpace(model.Password))
                {
                    entity.Password = model.Password;
                }

                entity.PhoneNumber = model.PhoneNumber;
                entity.Address1 = model.Address1;
                entity.Address2 = model.Address2;
                entity.City = model.City;                
                entity.ZipPostalCode = model.ZipPostalCode;
                entity.Active = model.Active;
                _customerService.UpdateCustomer(entity);

                // validate previous Role
                var previousRole = entity.CustomerRoles.SingleOrDefault();
                if(previousRole!=null)
                {
                    entity.CustomerRoles.Remove(previousRole);
                }

                //validate customer roles
                var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
                var newCustomerRole = allCustomerRoles.Where(r => r.Id == model.SelectedCustomerRoleId).SingleOrDefault();

                if (newCustomerRole != null)
                {
                    entity.CustomerRoles.Add(newCustomerRole);
                    _customerService.UpdateCustomer(entity);
                }
                    

                return RedirectToAction("List");
            }

            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("changepassword")]
        public ActionResult ChangePassword(CustomerModel model)
        {
            //if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
            //    return AccessDeniedView();

            var customer = _customerService.GetCustomerById(model.Id);
            if (customer == null)
                //No customer found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
               
            }

            return RedirectToAction("Edit", new { id = customer.Id });
        }
    }
}