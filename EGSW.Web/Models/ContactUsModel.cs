using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Models
{
    public class ContactUsModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone No.")]
        public string PhoneNo { get; set; }

        [Required]
        [AllowHtml]
        [Display(Name = "Message")]
        public string Message { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public bool Result { get; set; }
    }
}