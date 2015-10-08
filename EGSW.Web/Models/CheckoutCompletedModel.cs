using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGSW.Web.Models
{
    public class CheckoutCompletedModel
    {
        public int OrderId { get; set; }
        public string QuestionDeliveryTimeStr { get; set; }
    }
}