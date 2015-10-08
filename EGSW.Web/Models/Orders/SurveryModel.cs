using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EGSW.Web.Models.Orders
{
    public class SurveryModel
    {
        int Id { get; set; }
        public Guid SurveryGuid { get; set; }

        //[Required]
        [Display(Name="")]
        public string Question1 { get; set; }

        [Required]
        [Display(Name = "Question1")]
        public string Question2 { get; set; }

        [Required]
        [Display(Name = "Question2")]
        public string Question3 { get; set; }

        [Required]
        [Display(Name = "Question3")]
        public string Question4 { get; set; }
    }
}