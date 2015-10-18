using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Models.Customers
{
    public partial class AddressModel 
    {
        public int Id { get; set; }

        //[DisplayName("First Name")]
        //[Required]
        //[AllowHtml]
        //public string FirstName { get; set; }

        //[DisplayName("Last Name")]
        //[Required]
        //[AllowHtml]
        //public string LastName { get; set; }

        [DisplayName("Email")]
        //[Required]
        [EmailAddress]
        [AllowHtml]
        public string Email { get; set; }

        [DisplayName("State")]
        //[Required]
        [AllowHtml]
        public string StateProvinceName { get; set; }


        [DisplayName("City")]
        //[Required]
        [AllowHtml]
        public string City { get; set; }


        [DisplayName("Address1")]
        [Required]
        [AllowHtml]
        public string Address1 { get; set; }


        [DisplayName("Address2")]
        //[Required]
        [AllowHtml]
        public string Address2 { get; set; }


        [DisplayName("Zip Code")]
        [Required]
        [AllowHtml]
        public string ZipPostalCode { get; set; }



        [DisplayName("Phone Number")]
        //[Required]
        [AllowHtml]
        public string PhoneNumber { get; set; }

       

        
        
    }
}