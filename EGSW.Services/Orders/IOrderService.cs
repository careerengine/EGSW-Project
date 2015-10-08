using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PagedList;

namespace EGSW.Services.Orders
{
    public interface IOrderService
    {

        PagedList.IPagedList<GutterCleanOrder> SearchOrders( int customerId = 0, string customerEmail=null, int agentId = 0,            
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            OrderStatus? os = null, PaymentStatus? ps = null,  string orderGuid = null,string sortOrder=null,
            int pageIndex = 0, int pageSize = int.MaxValue);


        

        /// <summary>
        /// Insert a guest customer
        /// </summary>
        /// <returns>Customer</returns>
        void InsertOrder(GutterCleanOrder entity);

        void UpdateOrder(GutterCleanOrder entity);

        GutterCleanOrder GetOrderById(int orderId);

        GutterCleanOrder GetOrderByGuid(Guid orderGuid);




        #region Survey

        void InsertSurvey(Survery entity);
        
        Survery GetOrderSurveryByOrderId(int orderId);

        Survery GetOrderSurveryByOrderGuid(Guid orderGuid);
        #endregion





    }
}
