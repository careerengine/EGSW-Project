using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EGSW.Web.Areas.Admin.Models.Customers
{
    public class CustomerModel
    {
        public CustomerModel()
        {
            this.AvailableCustomerRoles = new List<CustomerRoleModel>();
        }

        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string ZipPostalCode { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
       
        public string LastIpAddress { get; set; }
        public System.DateTime CreatedOnUtc { get; set; }
        public Nullable<System.DateTime> LastLoginDateUtc { get; set; }

        public List<CustomerRoleModel> AvailableCustomerRoles { get; set; }

        [Required]
        public int SelectedCustomerRoleId{ get; set; }


        #region Average Rating Field

        public bool ShowRating { get; set; }

        public string Question1 { get; set; }


        [Display(Name = "Question1")]
        public string Question2 { get; set; }

        [Display(Name = "Question2")]
        public string Question3 { get; set; }

        [Display(Name = "Question3")]
        public string Question4 { get; set; }

        #endregion
        
    }
}