using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EGSW.Web.Models
{

    public class GutterCleanPaymentRequestModel
    {

        public GutterCleanPaymentRequestModel()
        {
            CreditCardTypes = new List<SelectListItem>();
            ExpireMonths = new List<SelectListItem>();
            ExpireYears = new List<SelectListItem>();
            AvailableAddress = new List<SelectListItem>();
            Warnings = new List<string>();
        }


         [DisplayName("First Name")]
        public string FirstName { get; set; }

         [DisplayName("Last Name")]
        public string LastName{get;set;}

         [DisplayName("Email")]
        public string Email {get;set;}

         [DisplayName("Phone Number")]
        public string ContactNumber { get; set; }

         [DisplayName("Billing Address")]
         [Required]
        public string Address { get; set; }

         [DisplayName("Billing Zipcode")]
         [Required]
        public string Zipcode { get; set; }

         [DisplayName("Order Total")]
        public decimal OrderTotal { get; set; }

        public bool AccetTermandCondition { get; set; }


         [DisplayName("Card Type")]
         [Required]
        public string CreditCardType { get; set; }

         [DisplayName("Name On Card")]
         [Required]
        public string NameOnCard { get; set; }

        [DisplayName("Card Number")]
        [Required]
        public string CardNumber { get; set; }

        [DisplayName("Expiry Month")]
        [Required]
        public int CardExpiryMonth { get; set; }

        [DisplayName("Expiry Year")]
        [Required]
        public int CardExpiryYear { get; set; }

        [DisplayName("Card Security Code")]
        [Required]
        public string CardSecurityCode { get; set; }
        public int SelectedAddressId { get; set; }

        public IList<SelectListItem> ExpireMonths { get; set; }
        public IList<SelectListItem> ExpireYears { get; set; }
        public IList<SelectListItem> CreditCardTypes { get; set; }
        public IList<SelectListItem> AvailableAddress { get; set; }

        public IList<string> Warnings { get; set; }

        #region review properties

        [Display(Name = "Square Footage")]
        public int QuestionSquareFootage { get; set; }

        [Display(Name = "Year Built")]
        public string QuestionYearBuiltStr { get; set; }

        [Display(Name = "Style Of Gutter")]
        public string QuestionStyleOfGutterStr { get; set; }

        [Display(Name = "Roof Material")]
        public string RoofMaterial { get; set; }

        public string QuestionDeliveryTimeStr { get; set; }

        public string AddressService { get; set; }

        public string zipcodeService { get; set; }
        #endregion
    }


    public class GutterCleanInfoModel
    {


        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }


        [Required]
        [Display(Name = "ZipCode")]
        public string ZipCode { get; set; }
    }
}