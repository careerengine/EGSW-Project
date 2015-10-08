using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PagedList;

namespace EGSW.Services.Orders
{
    public class OrderService :IOrderService
    {
        private readonly IRepository<GutterCleanOrder> _gutterCleanOrderRepository;

        private readonly IRepository<Survery> _surveryRepository;

        public OrderService(IRepository<GutterCleanOrder> gutterCleanOrderRepository,
            IRepository<Survery> surveryRepository)
        {
            this._gutterCleanOrderRepository = gutterCleanOrderRepository;
            this._surveryRepository = surveryRepository; 
        }


        public PagedList.IPagedList<GutterCleanOrder> SearchOrders(int customerId = 0, string customerEmail = null, int agentId = 0, DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            OrderStatus? os = null, PaymentStatus? ps = null, string orderGuid = null,
            string sortOrder = null, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            int? orderStatusId = null;
            if (os.HasValue)
                orderStatusId = (int)os.Value;

            int? paymentStatusId = null;
            if (ps.HasValue)
                paymentStatusId = (int)ps.Value;

           
            var query = _gutterCleanOrderRepository.Table;
            
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);

            if (!String.IsNullOrWhiteSpace(customerEmail))
                query = query.Where(o => o.Customer.Email.Contains(customerEmail));

            if (agentId>0)
                query = query.Where(o => o.AgentId == agentId);
            
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (orderStatusId.HasValue)
                query = query.Where(o => orderStatusId.Value == o.OrderStatusId);
            if (paymentStatusId.HasValue)
                query = query.Where(o => paymentStatusId.Value == o.PaymentStatusId);
           
           
            query = query.Where(o => !o.Deleted);

            switch (sortOrder)
            {
                case "Id":
                    query = query.OrderBy(s => s.Id);
                    break;
                case "Id_desc":
                    query = query.OrderByDescending(s => s.Id);
                    break;

                case "IsAgentAssign":
                    query = query.OrderBy(s => s.IsAgentAssign);
                    break;
                case "IsAgentAssign_desc":
                    query = query.OrderByDescending(s => s.IsAgentAssign);
                    break;

                case "IsWorkedComplated":
                    query = query.OrderBy(s => s.IsWorkedComplated);
                    break;
                case "IsWorkedComplated_desc":
                    query = query.OrderByDescending(s => s.IsWorkedComplated);
                    break;

                case "IsCustomerQa":
                    query = query.OrderBy(s => s.IsCustomerQa);
                    break;
                case "IsCustomerQa_desc":
                    query = query.OrderByDescending(s => s.IsCustomerQa);
                    break;

                case "IsPayAgentWorker":
                    query = query.OrderBy(s => s.IsPayAgentWorker);
                    break;
                case "IsPayAgentWorker_desc":
                    query = query.OrderByDescending(s => s.IsPayAgentWorker);
                    break;

                case "LastUpdatedDateUtc":
                    query = query.OrderBy(s => s.LastUpdatedDateUtc);
                    break;
                case "LastUpdatedDateUtc_desc":
                    query = query.OrderByDescending(s => s.LastUpdatedDateUtc);
                    break;

                default:
                    query = query.OrderByDescending(s => s.LastUpdatedDateUtc);
                    break;
            }


            //query = query.OrderByDescending(o => o.CreatedOnUtc);



            if (!String.IsNullOrEmpty(orderGuid))
            {
                //filter by GUID. Filter in BLL because EF doesn't support casting of GUID to string
                var orders = query.ToList();
                orders = orders.FindAll(o => o.OrderGuid.ToString().ToLowerInvariant().Contains(orderGuid.ToLowerInvariant()));
                return new PagedList.PagedList<GutterCleanOrder>(orders, pageIndex, pageSize);
            }

            //database layer paging
            return new PagedList.PagedList<GutterCleanOrder>(query, pageIndex, pageSize);
        }

        
        
        
        public void InsertOrder(GutterCleanOrder entity)
        {
            if (entity == null)
                throw new ArgumentNullException("GutterCleanOrder");

            _gutterCleanOrderRepository.Insert(entity);
        }


        public GutterCleanOrder GetOrderById(int orderId)
        {
            if (orderId == 0)
                return null;

            return _gutterCleanOrderRepository.GetById(orderId);
        }

        public GutterCleanOrder GetOrderByGuid(Guid orderGuid)
        {
            var query = _gutterCleanOrderRepository.Table;

            var result = query.Where(o => o.OrderGuid.Value.ToString() == orderGuid.ToString()).SingleOrDefault();

            return result;
        }


        public void UpdateOrder(GutterCleanOrder entity)
        {
            if (entity == null)
                throw new ArgumentNullException("GutterCleanOrder");



            _gutterCleanOrderRepository.Update(entity);
        }




        public void InsertSurvey(Survery entity)
        {

            if (entity == null)
                throw new ArgumentNullException("Survery");

            _surveryRepository.Insert(entity);
        }

        public Survery GetOrderSurveryByOrderId(int orderId)
        {
            var query = _surveryRepository.Table;

            var result = query.Where(o => o.OrderId == orderId).SingleOrDefault();

            return result;
        }

        public Survery GetOrderSurveryByOrderGuid(Guid orderGuid)
        {
            var query = _surveryRepository.Table;

            var result = query.Where(o => o.SurveryGuid.Value.ToString() == orderGuid.ToString()).SingleOrDefault();

            return result;
        }


        
    }
}
