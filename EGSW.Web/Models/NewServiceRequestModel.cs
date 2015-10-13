using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EGSW.Web.Models
{
    public class NewServiceRequestModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string ServiceEmailAdddress { get; set; }

        [Required]
        [RegularExpression(@"^\d{5}?$", ErrorMessage = "Invalid Zipcode")]
        public string ServiceZipCode { get; set; }

        
        public string Message { get; set; }


        public bool Result { get; set; }
    }
}