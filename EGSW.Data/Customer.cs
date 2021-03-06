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
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public Customer()
        {
            this.CustomerRoles = new HashSet<CustomerRole>();
            this.GutterCleanOrders = new HashSet<GutterCleanOrder>();
            this.Addresses = new HashSet<Address>();
        }
    
        public int Id { get; set; }
        public System.Guid CustomerGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ZipPostalCode { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public bool IsSystemAccount { get; set; }
        public string LastIpAddress { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public Nullable<System.DateTime> LastLoginDateUtc { get; set; }
        public Nullable<System.DateTime> LastActivityDateUtc { get; set; }
        public string PasswordRecoveryToken { get; set; }
    
        public virtual ICollection<CustomerRole> CustomerRoles { get; set; }
        public virtual ICollection<GutterCleanOrder> GutterCleanOrders { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}
