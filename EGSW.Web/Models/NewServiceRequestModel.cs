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
        public string ServiceEmailAdddress { get; set; }

        [Required]
        public string ServiceZipCode { get; set; }

        
        public string Message { get; set; }


        public bool Result { get; set; }
    }
}