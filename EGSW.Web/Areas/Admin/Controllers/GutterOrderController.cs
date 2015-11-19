using EGSW.Data;
using EGSW.Data.Customers;
using EGSW.Framework;
using EGSW.Services;
using EGSW.Services.Directory;
using EGSW.Services.Orders;
using EGSW.Web.Areas.Admin.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Areas.Admin.Controllers
{
    public class GutterOrderController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IQuestionAnswerEntityData _questionAnswerEntityDataService;
        private readonly IZipCodeService _zipCodeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDbContext _dbContext;


        public GutterOrderController(IWorkContext workContext, ICustomerService customerService, IOrderService orderService,
            IQuestionAnswerEntityData questionAnswerEntityDataService, IZipCodeService zipCodeService,
            IWorkflowMessageService workflowMessageService,
            IDateTimeHelper dateTimeHelper,
            IDbContext dbContext)
        {
            this._customerService = customerService;
            this._workContext = workContext;
            this._orderService = orderService;
            this._questionAnswerEntityDataService = questionAnswerEntityDataService;
            this._zipCodeService = zipCodeService;

            this._workflowMessageService = workflowMessageService;
            this._dateTimeHelper = dateTimeHelper;
            this._dbContext = dbContext;


        }


        protected virtual GutterCleanOrderModel PrepareOrderDetailsModel(GutterCleanOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            var model = new GutterCleanOrderModel();
            model.Id = order.Id;
            model.OrderGuid =(Guid) order.OrderGuid;
            model.QuestionSquareFootage = order.QuestionSquareFootage;
            model.QuestionStyleOfGutter = order.QuestionStyleOfGutter;
            model.CustomerId = order.CustomerId;
            model.CustomerName = order.Customer.FirstName + " " + order.Customer.LastName;

            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == order.QuestionStyleOfGutter).SingleOrDefault();
            model.QuestionStyleOfGutterStr = r.Text;

            model.QuestionYearBuilt = order.QuestionYearBuilt;

            var year = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == order.QuestionYearBuilt.ToString()).SingleOrDefault();
            model.QuestionYearBuiltStr = year.Text;

            model.RoofMaterial = order.RoofMaterial;

            model.QuestionDeliveryTimeStr = "5 business days";
            if (order.QuestionDeliveryTime == 1)
                model.QuestionDeliveryTimeStr = "5 business days";

            if (order.QuestionDeliveryTime == 2)
                model.QuestionDeliveryTimeStr = "8 hours";

            if (order.QuestionDeliveryTime == 3)
                model.QuestionDeliveryTimeStr = "4 hours";


            model.OrderTotal = order.OrderTotal;

            model.Address = order.Address;

            model.City = order.City;
            model.State = order.State;

            model.Zipcode = order.Zipcode;
            model.AgentId = order.AgentId.HasValue ? (int)order.AgentId.Value : 0;

            model.PaymentStatusId = order.PaymentStatusId;
            model.OrderStatusId = order.OrderStatusId;
            model.CaptureTransactionId = order.CaptureTransactionId;
            model.CaptureTransactionResult = order.CaptureTransactionResult;
            //model.CreatedOnUtc = order.CreatedOnUtc;
            model.CreatedOnUtc = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            if (order.CompletionDateUtc.HasValue)
                model.CompletionDateUtc = _dateTimeHelper.ConvertToUserTime(order.CompletionDateUtc.Value, DateTimeKind.Utc); 
           

            // This button only show 'Admin User'
            model.HideSetAgentButton = _workContext.CurrentCustomer.IsAgent();
            
            // agent Mark Paid Button
            model.HideMarkPaidButton = true;

            model.IsAgentPaid = order.IsPayAgentWorker.HasValue ? order.IsPayAgentWorker.Value : false;

            if (_workContext.CurrentCustomer.IsAdmin())
                model.orderViewByAdmin = true;

            
            if (order.AgentId <= 0)
                model.HideSetWorkedCompletedButton = true;
            else
            {
                if (((OrderStatus)model.OrderStatusId).ToString() == OrderStatus.Complete.ToString())
                {
                    model.HideSetWorkedCompletedButton = true;
                    model.HideSetAgentButton = true;

                    if (!model.IsAgentPaid && !_workContext.CurrentCustomer.IsAgent())
                    {
                        model.HideMarkPaidButton = false;
                    }
                }
            }

            if (order.IsCustomerQa.HasValue)
            {
                if (order.IsCustomerQa.Value)
                {
                    model.ShowRating = true;

                    var orderSurvey=order.Surveries.FirstOrDefault();
                    if(orderSurvey!=null)
                    {
                        //model.Question1 =Convert.ToString( orderSurvey.Question1);
                        model.Question2 = Convert.ToString(orderSurvey.Question2);
                        model.Question3 = Convert.ToString(orderSurvey.Question3);
                        model.Question4 = Convert.ToString(orderSurvey.Question4);
                    }

                    // Rating detail will not show to agent.
                    if (_workContext.CurrentCustomer.IsAgent())
                    {
                        model.ShowRating = false;
                    }
                }

            }

            

            return model;
        }


        // GET: Admin/Order
        public ActionResult Index()
        {
            return View();
        }

        // GET: Agent Order List
        public ActionResult ListAllOrdersOfAgent(int Id)
        {
            var orderList = _orderService.GetOrderByAgentId(Id);

            ViewBag.orderList = orderList;
            return View();
        }


        // GET: Order
        
        [AdminRoleAuthorizeAttribute]
        public ActionResult GutterOrderStepList(string sortOrder, int? page)
        {

            int pageSize = 50;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;


            ViewBag.LastUpdatedDateUtcSortParm = String.IsNullOrEmpty(sortOrder) ? "LastUpdatedDateUtc_desc" : "";
            ViewBag.IdSortParm = sortOrder == "Id" ? "Id_desc" : "Id";
            ViewBag.IsAgentAssignSortParm = sortOrder == "IsAgentAssign" ? "IsAgentAssign_desc" : "IsAgentAssign";
            ViewBag.IsWorkedComplatedSortParm = sortOrder == "IsWorkedComplated" ? "IsWorkedComplated_desc" : "IsWorkedComplated";
            ViewBag.IsCustomerQaSortParm = sortOrder == "IsCustomerQa" ? "IsCustomerQa_desc" : "IsCustomerQa";
            ViewBag.IsPayAgentWorkerSortParm = sortOrder == "IsPayAgentWorker" ? "IsPayAgentWorker_desc" : "IsPayAgentWorker";            



            var products = _orderService.SearchOrders(sortOrder:sortOrder, pageIndex: pageIndex,pageSize: pageSize);

            return View(products);
        }


        // GET: Order
        [AdminRoleAuthorizeAttribute]
        public ActionResult List( string CustomerEmail, int? OrderStatusId, int? page)
        {
            ViewBag.CustomerEmailData = CustomerEmail;
            ViewBag.OrderStatusIdData = OrderStatusId;


            OrderStatus? orderStatus = OrderStatusId > 0 ? (OrderStatus?)(OrderStatusId) : null;

            int pageSize = 50;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            string sortOrder = "Id";
            //ViewBag.IdSortParm = sortOrder == "Id" ? "Id_desc" : "Id";
            var products = _orderService.SearchOrders(os:orderStatus, customerEmail:CustomerEmail, pageIndex: pageIndex,
            pageSize: pageSize, sortOrder: "Id_desc");

            return View(products);
        }


        // GET: Agent Order List
        public ActionResult AgentOrderList(int? page)
        {

            int pageSize = 50;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var products = _orderService.SearchOrders(agentId:_workContext.CurrentCustomer.Id, pageIndex: pageIndex,
            pageSize: pageSize);

            return View(products);
        }

        

        public ActionResult OrderDetail(int orderId)
        {

            var order = _orderService.GetOrderById(orderId);

            if (order == null || order.Deleted)
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentCustomer.IsAdmin())
            {

            }
            else if (_workContext.CurrentCustomer.IsAgent() && order.AgentId!=_workContext.CurrentCustomer.Id)
            {
                return RedirectToAction("OrderDetail");
            }

            var model = PrepareOrderDetailsModel(order);

            var customerList = _customerService.GetAllCustomers(new int[] { 4, 1 });
            if (customerList.Count > 0)
            {
                foreach (var customer in customerList)
                    model.AvailableAgents.Add(new SelectListItem() { Text = customer.FirstName + " " + customer.LastName, Value = customer.Id.ToString() });
            }
            return View(model);
        }

       [HttpPost]
       [FormValueRequired("Edit")]
        public ActionResult OrderDetail(GutterCleanOrderModel model)
        {
            var order = _orderService.GetOrderById(model.Id);

            if (order == null || order.Deleted)
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentCustomer.IsAdmin())
            {

            }
            else if (_workContext.CurrentCustomer.IsAgent() && order.AgentId != _workContext.CurrentCustomer.Id)
            {
                return RedirectToAction("OrderDetail");
            }


            var customerList = _customerService.GetAllCustomers(new int[] { 4,1 });
            if (customerList.Count > 0)
            {
                foreach (var customer in customerList)
                    model.AvailableAgents.Add(new SelectListItem() { Text = customer.FirstName + " " + customer.LastName, Value = customer.Id.ToString() });
            }
            return View(model);
        }

        [HttpPost, ActionName("OrderDetail")]
        [FormValueRequired("setagent")]
        public ActionResult OrderDetailAsginAgent(GutterCleanOrderModel model)
        {
            var order = _orderService.GetOrderById(model.Id);

            if (order == null || order.Deleted)
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentCustomer.IsAdmin())
            {

            }
            else if (_workContext.CurrentCustomer.IsAgent() && order.AgentId != _workContext.CurrentCustomer.Id)
            {
                return RedirectToAction("OrderDetail");
            }


            if (model.AgentId > 0)
            {
                order.AgentId = model.AgentId;
                order.IsAgentAssign = true;
                order.LastUpdatedDateUtc = DateTime.UtcNow;
                _orderService.UpdateOrder(order);

                _workflowMessageService.SendOrderAssigneAgentNotification(order, model.AgentId);
            }

            

            return RedirectToAction("OrderDetail", new { orderId = model.Id });
        }


        [HttpPost, ActionName("OrderDetail")]
        [FormValueRequired("setworkcompleted")]
        public ActionResult SetOrderCompleted(GutterCleanOrderModel model)
        {
            var order = _orderService.GetOrderById(model.Id);

            if (order == null || order.Deleted)
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentCustomer.IsAdmin())
            {

            }
            else if (_workContext.CurrentCustomer.IsAgent() && order.AgentId != _workContext.CurrentCustomer.Id)
            {
                return RedirectToAction("OrderDetail");
            }

            if (order.AgentId > 0)
            {
                order.OrderStatusId = (int)OrderStatus.Complete;
                order.CompletionDateUtc = DateTime.UtcNow;
                order.IsWorkedComplated = true;
                order.LastUpdatedDateUtc = DateTime.UtcNow;
                _orderService.UpdateOrder(order);

                _workflowMessageService.SendOrderPlacedWorkCompletedCustomerNotification(order);
            }



            return RedirectToAction("OrderDetail", new { orderId = model.Id });
        }



        [HttpPost, ActionName("OrderDetail")]
        [FormValueRequired("setmarkpaidagent")]
        public ActionResult SetOrderMarkPaidAgent(GutterCleanOrderModel model)
        {
            var order = _orderService.GetOrderById(model.Id);

            if (order == null || order.Deleted)
                return new HttpUnauthorizedResult();

            if (_workContext.CurrentCustomer.IsAdmin())
            {

            }
            else if (_workContext.CurrentCustomer.IsAgent() && order.AgentId != _workContext.CurrentCustomer.Id)
            {
                return RedirectToAction("OrderDetail");
            }

            if (model.AgentId > 0)
            {
              
                order.IsPayAgentWorker = true;
                order.LastUpdatedDateUtc = DateTime.UtcNow;
                _orderService.UpdateOrder(order);

                //_workflowMessageService.SendOrderPlacedWorkCompletedCustomerNotification(order);
            }



            return RedirectToAction("OrderDetail", new { orderId = model.Id });
        }


       


    }
}