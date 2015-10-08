using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGSW.Web.Models.Orders
{
    public class GutterCleanOrderList
    {
        public GutterCleanOrderList()
        {
            Orders = new List<GutterCleanOrderModel>();
        }

        public IList<GutterCleanOrderModel> Orders { get; set; }
    }
}