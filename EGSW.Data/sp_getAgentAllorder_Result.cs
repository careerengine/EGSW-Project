//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EGSW.Data
{
    using System;
    
    public partial class sp_getAgentAllorder_Result
    {
        public int Id { get; set; }
        public Nullable<System.Guid> OrderGuid { get; set; }
        public int CustomerId { get; set; }
        public int QuestionSquareFootage { get; set; }
        public int QuestionYearBuilt { get; set; }
        public string QuestionStyleOfGutter { get; set; }
        public string RoofMaterial { get; set; }
        public int QuestionDeliveryTime { get; set; }
        public string Address { get; set; }
        public string Zipcode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int OrderStatusId { get; set; }
        public int PaymentStatusId { get; set; }
        public decimal OrderTotal { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<int> WorkerId { get; set; }
        public int AgentStatusId { get; set; }
        public int WorkerStatusId { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public bool Deleted { get; set; }
        public string CustomerIp { get; set; }
        public Nullable<System.DateTime> CompletionDateUtc { get; set; }
        public string CaptureTransactionId { get; set; }
        public string CaptureTransactionResult { get; set; }
        public string CapturePaymentGatwayResponse { get; set; }
        public Nullable<bool> IsAgentAssign { get; set; }
        public Nullable<bool> IsWorkedComplated { get; set; }
        public Nullable<bool> IsPayAgentWorker { get; set; }
        public Nullable<bool> IsCustomerQa { get; set; }
        public Nullable<System.DateTime> LastUpdatedDateUtc { get; set; }
    }
}
