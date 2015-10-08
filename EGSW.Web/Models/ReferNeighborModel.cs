using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Models
{
    public partial class ReferNeighborModel
    {
        [Required]
        [Display(Name = "Friend’s Name")]
        public string FriendName { get; set; }

        [Required]
        [Display(Name = "Friend’s email address")]
        public string FriendEmail { get; set; }

        [Required]
        [Display(Name = "Your Name")]
        public string YourName { get; set; }    

        
        
        public bool Result { get; set; }
    }
}