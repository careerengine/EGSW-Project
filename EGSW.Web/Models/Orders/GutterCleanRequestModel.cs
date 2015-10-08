using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace EGSW.Web.Models.Orders
{
    public class GutterCleanRequestModel
    {
        public GutterCleanRequestModel()
        {
            //this.OrderNotes = new HashSet<OrderNote>();

            this.AvailableSquareFootage = new List<SelectListItem>();
            this.AvailableYearBuilt = new List<SelectListItem>();
            this.AvailableStyleOfGutter = new List<SelectListItem>();
            this.AvailableRoofMaterial = new List<SelectListItem>();
            this.AvailableDeliveryTime = new List<SelectListItem>();
        }
    
        public int Id { get; set; }
        public Guid OrderGuid { get; set; }
        public int CustomerId { get; set; }

        [Display(Name = "Square Footage")]
        [Required]
        public int QuestionSquareFootage { get; set; }

        [Display(Name = "Year Built")]        
        [Required]
        public int QuestionYearBuilt { get; set; }

        [Display(Name = "Style Of Gutter")]
        [Required]
        public string QuestionStyleOfGutter { get; set; }


        [Display(Name = "Roof Material")]
        [Required]
        public string RoofMaterial { get; set; }

        [Display(Name = "Delivery Time")]
        public int QuestionDeliveryTime { get; set; }

        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
        public decimal OrderTotal { get; set; }
        public int? AgentId { get; set; }
        public int? WorkerId { get; set; }
        public int AgentStatusId { get; set; }
        public int WorkerStatusId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool Deleted { get; set; }
        public string CustomerIp { get; set; }
        public DateTime? CompletionDateUtc { get; set; }
        public string CaptureTransactionId { get; set; }
        public string CaptureTransactionResult { get; set; }
        public string CapturePaymentGatwayResponse { get; set; }



        public IList<SelectListItem> AvailableSquareFootage { get; set; }
        public IList<SelectListItem> AvailableYearBuilt { get; set; }
        public IList<SelectListItem> AvailableStyleOfGutter { get; set; }
        public IList<SelectListItem> AvailableRoofMaterial { get; set; }
        public IList<SelectListItem> AvailableDeliveryTime { get; set; }
    }
}