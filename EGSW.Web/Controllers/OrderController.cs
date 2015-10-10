using EGSW.Data;
using EGSW.Data.Customers;
using EGSW.Services;
using EGSW.Services.Directory;
using EGSW.Services.Orders;
using EGSW.Web.ActionFilters;
using EGSW.Web.Models;
using EGSW.Web.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Controllers
{
    [RequreSecureConnectionFilter]
    public class OrderController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IQuestionAnswerEntityData _questionAnswerEntityDataService;
        private readonly IZipCodeService _zipCodeService;

        public OrderController(IWorkContext workContext, ICustomerService customerService,IOrderService orderService,
            IQuestionAnswerEntityData questionAnswerEntityDataService, IZipCodeService zipCodeService)
        {           
            this._customerService = customerService;
            this._workContext = workContext;            
            this._orderService = orderService;
            this._questionAnswerEntityDataService = questionAnswerEntityDataService;
            this._zipCodeService = zipCodeService;

           
        }

        protected virtual GutterCleanOrderModel PrepareOrderDetailsModel(GutterCleanOrder order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            var model = new GutterCleanOrderModel();
            model.QuestionSquareFootage = order.QuestionSquareFootage;
            model.QuestionStyleOfGutter = order.QuestionStyleOfGutter;
            model.QuestionDeliveryTime = order.QuestionDeliveryTime;
            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == order.QuestionStyleOfGutter).SingleOrDefault();
            model.QuestionStyleOfGutterStr = r.Text;

            model.QuestionYearBuilt = order.QuestionYearBuilt;

            var year = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == order.QuestionYearBuilt.ToString()).SingleOrDefault();
            model.QuestionYearBuiltStr = year.Text;

            model.RoofMaterial = order.RoofMaterial;

            model.QuestionDeliveryTimeStr = "5 business days";
            if (model.QuestionDeliveryTime == 1)
                model.QuestionDeliveryTimeStr = "5 business days";

            if (model.QuestionDeliveryTime == 2)
                model.QuestionDeliveryTimeStr = "8 hours";

            if (model.QuestionDeliveryTime == 3)
                model.QuestionDeliveryTimeStr = "4 hours";


            model.OrderTotal = order.OrderTotal;

            model.Address = order.Address;

            model.City = order.City;
            model.State = order.State;          
            
            model.Zipcode = order.Zipcode;

            model.PaymentStatusId = order.PaymentStatusId;
            model.OrderStatusId = order.OrderStatusId;
            model.CaptureTransactionId = order.CaptureTransactionId;
            model.CaptureTransactionResult = order.CaptureTransactionResult;
            model.CreatedOnUtc = order.CreatedOnUtc;

            
            
            return model;
        }
        // GET: Order
        public ActionResult Index(int? page)
        {

            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

             int pageSize = 5;
             int pageIndex = 1;
             pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
           
                var products = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id,
                pageIndex: pageIndex,
                pageSize: pageSize);

            return View(products);
        }


        public ActionResult OrderDetail(int orderId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            var order = _orderService.GetOrderById(orderId);

            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return new HttpUnauthorizedResult();

            var model = PrepareOrderDetailsModel(order);

            return View(model);
        }


        public ActionResult Survey(string surveykey)
        {
            SurveryModel model = new SurveryModel();
            model.SurveryGuid = Guid.Parse(surveykey);
            Guid suryKey;

            if(Guid.TryParse(surveykey, out suryKey))
            {
                var suery = _orderService.GetOrderSurveryByOrderGuid(suryKey);

                if (suery != null)
                    return RedirectToRoute("HomePage");
            }

            

            return View(model);
        }

        [HttpPost]
        public ActionResult Survey(SurveryModel model)
        {
            Survery entity = new Survery();

            var order = _orderService.GetOrderByGuid(model.SurveryGuid);

            if (ModelState.IsValid)
            {
                entity.OrderId = order.Id;
                entity.SurveryGuid = order.OrderGuid;
                entity.Question1 = 0;// Convert.ToInt32(model.Question1);
                entity.Question2 = Convert.ToInt32(model.Question2);
                entity.Question3 = Convert.ToInt32(model.Question3);
                entity.Question4 = Convert.ToInt32(model.Question4);
                entity.CreatedOnUtc = DateTime.UtcNow;

                _orderService.InsertSurvey(entity);

                order.IsCustomerQa = true;
                order.LastUpdatedDateUtc = DateTime.UtcNow;
                _orderService.UpdateOrder(order);

                return RedirectToRoute("SurveyConfirmation");
            }
            return View(model);
        }

        public ActionResult SurveyConfirmation()
        {
            return View();
        }
    }
}