using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Models
{

    public class GutterCleanOrderModel
    {
        public GutterCleanOrderModel()
        {
            //this.OrderNotes = new HashSet<OrderNote>();

            this.AvailableSquareFootage = new List<SelectListItem>();
            this.AvailableYearBuilt = new List<SelectListItem>();
            this.AvailableStyleOfGutter = new List<SelectListItem>();
            this.AvailableRoofMaterial = new List<SelectListItem>();
        }
    

        public int Id { get; set; }
        public Guid OrderGuid { get; set; }
        public int CustomerId { get; set; }
        public int QuestionSquareFootage { get; set; }
        public int QuestionYearBuilt { get; set; }
        public int QuestionDeliveryTime { get; set; }
        public string QuestionYearBuiltStr { get; set; }
        public string QuestionStyleOfGutter { get; set; }
        public string QuestionStyleOfGutterStr { get; set; }
        public string QuestionDeliveryTimeStr { get; set; }
        public string RoofMaterial { get; set; }
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
    
        //public virtual Customer Customer { get; set; }
        //public virtual ICollection<OrderNote> OrderNotes { get; set; }
    }
}