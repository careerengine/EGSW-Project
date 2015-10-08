using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Areas.Admin.Models.Orders
{
    public class GutterCleanOrderModel
    {

        public GutterCleanOrderModel()
        {
            //this.OrderNotes = new HashSet<OrderNote>();
            this.AvailableAgents = new List<SelectListItem>();           
        }


        public IList<SelectListItem> AvailableAgents { get; set; }

        public int Id { get; set; }

        [Display(Name = "Order Guid")]        
        public Guid OrderGuid { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }


        [Display(Name = "Customer")]
        public string CustomerName { get; set; }

        [Display(Name = "Square Footage")]
        public int QuestionSquareFootage { get; set; }

        public int QuestionYearBuilt { get; set; }

        [Display(Name = "Year Built")]
        public string QuestionYearBuiltStr { get; set; }

        public string QuestionStyleOfGutter { get; set; }

        [Display(Name = "Style Of Gutter")]
        public string QuestionStyleOfGutterStr { get; set; }

        [Display(Name = "Delivery Time")]
        public string QuestionDeliveryTimeStr { get; set; }

        [Display(Name = "Roof Material")]
        public string RoofMaterial { get; set; }

       


        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        [Display(Name = "Order Status")]
        public int OrderStatusId { get; set; }

        [Display(Name = "Payment Status")]
        public int PaymentStatusId { get; set; }

        [Display(Name = "Order Total")]
        public decimal OrderTotal { get; set; }

        [Display(Name = "Agent")] 
        public int AgentId { get; set; }


        public bool IsAgentPaid { get; set; }

        public bool HideSetAgentButton { get; set; }
        public bool HideSetWorkedCompletedButton { get; set; }
        public bool HideMarkPaidButton { get; set; }
        public bool ShowRating { get; set; }

        public int? WorkerId { get; set; }
        public int AgentStatusId { get; set; }
        public int WorkerStatusId { get; set; }

        [Display(Name = "Created On")]
        public DateTime CreatedOnUtc { get; set; }
        public bool Deleted { get; set; }
        public string CustomerIp { get; set; }

        [Display(Name = "Completion Date")] 
        public DateTime? CompletionDateUtc { get; set; }

        [Display(Name = "Transaction Id")]
        public string CaptureTransactionId { get; set; }
        public string CaptureTransactionResult { get; set; }
        public string CapturePaymentGatwayResponse { get; set; }


        #region Rating Field

        public string Question1 { get; set; }

        [Display(Name = "Question1")]
        public string Question2 { get; set; }

        [Display(Name = "Question2")]
        public string Question3 { get; set; }

        [Display(Name = "Question3")]
        public string Question4 { get; set; }

        #endregion

        public bool orderViewByAdmin { get; set; }
    }
}