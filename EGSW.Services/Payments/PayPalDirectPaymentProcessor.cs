using EGSW.Data;
using PayPal.PayPalAPIInterfaceService;
using PayPal.PayPalAPIInterfaceService.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EGSW.Services.Payments
{
    public class PayPalDirectPaymentProcessor : IPaymentMethod
    {
        private readonly PayPalDirectPaymentSettings _paypalDirectPaymentSettings;
        private readonly ICustomerService _customerService;
        private readonly IWebHelper _webHelper;

        public PayPalDirectPaymentProcessor(ICustomerService customerService, IWebHelper webHelper)
        {
            _paypalDirectPaymentSettings = new PayPalDirectPaymentSettings();
            _customerService = customerService;
            _webHelper = webHelper;
        }

        #region Utilities

        /// <summary>
        /// Gets Paypal URL
        /// </summary>
        /// <returns></returns>
        private string GetPaypalUrl()
        {
            return _paypalDirectPaymentSettings.UseSandbox ? "https://www.sandbox.paypal.com/us/cgi-bin/webscr" :
                "https://www.paypal.com/us/cgi-bin/webscr";
        }

        protected PayPalAPIInterfaceServiceService GetService()
        {
            var config = new Dictionary<string, string>();
            var url = _paypalDirectPaymentSettings.UseSandbox ? "https://api-3t.sandbox.paypal.com/2.0" : "https://api-3t.paypal.com/2.0";
            var mode = _paypalDirectPaymentSettings.UseSandbox ? "sandbox" : "live";

            config.Add("PayPalAPI", url);
            config.Add("mode", mode);
            config.Add("account0.apiUsername", _paypalDirectPaymentSettings.ApiAccountName);
            config.Add("account0.apiPassword", _paypalDirectPaymentSettings.ApiAccountPassword);
            config.Add("account0.apiSignature", _paypalDirectPaymentSettings.Signature);

            var service = new PayPalAPIInterfaceServiceService(config);
            return service;
        }

        /// <summary>
        /// Get Paypal country code
        /// </summary>
        /// <param name="country">Country</param>
        /// <returns>Paypal country code</returns>
        protected CountryCodeType GetPaypalCountryCodeType(string country)
        {
            var payerCountry = CountryCodeType.US;
            try
            {
                payerCountry = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), country.ToUpperInvariant());
            }
            catch
            {
            }
            return payerCountry;
        }

        /// <summary>
        /// Get Paypal credit card type
        /// </summary>
        /// <param name="creditCardType">Credit card type</param>
        /// <returns>Paypal credit card type</returns>
        protected CreditCardTypeType GetPaypalCreditCardType(string creditCardType)
        {
            if (String.IsNullOrEmpty(creditCardType))
                return CreditCardTypeType.VISA;

            if (creditCardType.Equals("VISA", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.VISA;
            if (creditCardType.Equals("MASTERCARD", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.MASTERCARD;
            if (creditCardType.Equals("DISCOVER", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.DISCOVER;
            if (creditCardType.Equals("AMEX", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.AMEX;
            if (creditCardType.Equals("MAESTRO", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.MAESTRO;
            if (creditCardType.Equals("SOLO", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.SOLO;
            if (creditCardType.Equals("SWITCH", StringComparison.InvariantCultureIgnoreCase))
                return CreditCardTypeType.SWITCH;

            return (CreditCardTypeType)Enum.Parse(typeof(CreditCardTypeType), creditCardType);
        }

        protected string GetApiVersion()
        {
            return "117";
        }

        protected ProcessPaymentResult AuthorizeOrSale(ProcessPaymentRequest processPaymentRequest, bool authorizeOnly)
        {
            var result = new ProcessPaymentResult();
            

            var customer = _customerService.GetCustomerById(processPaymentRequest.CustomerId);
            if (customer == null)
                throw new Exception("Customer cannot be loaded");

            var req = new DoDirectPaymentReq();
            req.DoDirectPaymentRequest = new DoDirectPaymentRequestType();
            req.DoDirectPaymentRequest.Version = GetApiVersion();
            var details = new DoDirectPaymentRequestDetailsType();
            req.DoDirectPaymentRequest.DoDirectPaymentRequestDetails = details;
            details.IPAddress = _webHelper.GetCurrentIpAddress() ?? "";
            if (authorizeOnly)
                details.PaymentAction = PaymentActionCodeType.AUTHORIZATION;
            else
                details.PaymentAction = PaymentActionCodeType.SALE;
            //credit card
            details.CreditCard = new CreditCardDetailsType();
            details.CreditCard.CreditCardNumber = processPaymentRequest.CreditCardNumber;
            details.CreditCard.CreditCardType = GetPaypalCreditCardType(processPaymentRequest.CreditCardType);
            details.CreditCard.ExpMonth = processPaymentRequest.CreditCardExpireMonth;
            details.CreditCard.ExpYear = processPaymentRequest.CreditCardExpireYear;
            details.CreditCard.CVV2 = processPaymentRequest.CreditCardCvv2;
            
            //details.CreditCard.CardOwner = new PayerInfoType();
            //details.CreditCard.CardOwner.PayerCountry = GetPaypalCountryCodeType(customer.BillingAddress.Country);
            ////billing address
            //details.CreditCard.CardOwner.Address = new AddressType();
            //details.CreditCard.CardOwner.Address.Street1 = customer.BillingAddress.Address1;
            //details.CreditCard.CardOwner.Address.Street2 = customer.BillingAddress.Address2;
            //details.CreditCard.CardOwner.Address.CityName = customer.BillingAddress.City;
            //if (customer.BillingAddress.StateProvince != null)
            //    details.CreditCard.CardOwner.Address.StateOrProvince = customer.BillingAddress.StateProvince.Abbreviation;
            //else
            //    details.CreditCard.CardOwner.Address.StateOrProvince = "CA";
            //details.CreditCard.CardOwner.Address.Country = GetPaypalCountryCodeType(customer.BillingAddress.Country);
            //details.CreditCard.CardOwner.Address.PostalCode = customer.BillingAddress.ZipPostalCode;
            //details.CreditCard.CardOwner.Payer = customer.BillingAddress.Email;
            //details.CreditCard.CardOwner.PayerName = new PersonNameType();
            //details.CreditCard.CardOwner.PayerName.FirstName = customer.BillingAddress.FirstName;
            //details.CreditCard.CardOwner.PayerName.LastName = customer.BillingAddress.LastName;

            //order totals
            var payPalCurrency = PaypalHelper.GetPaypalCurrency("USD");
            details.PaymentDetails = new PaymentDetailsType();
            details.PaymentDetails.OrderTotal = new BasicAmountType();
            details.PaymentDetails.OrderTotal.value = Math.Round(processPaymentRequest.OrderTotal, 2).ToString("N", new CultureInfo("en-us"));
            details.PaymentDetails.OrderTotal.currencyID = payPalCurrency;
            details.PaymentDetails.Custom = processPaymentRequest.OrderGuid.ToString();
            details.PaymentDetails.ButtonSource = "EGSWPaymentButton";

            ////shipping
            //if (customer.ShippingAddress != null)
            //{
            //    if (customer.ShippingAddress.StateProvince != null && customer.ShippingAddress.Country != null)
            //    {
            //        var shippingAddress = new AddressType();
            //        shippingAddress.Name = customer.ShippingAddress.FirstName + " " + customer.ShippingAddress.LastName;
            //        shippingAddress.Street1 = customer.ShippingAddress.Address1;
            //        shippingAddress.Street2 = customer.ShippingAddress.Address2;
            //        shippingAddress.CityName = customer.ShippingAddress.City;
            //        shippingAddress.StateOrProvince = customer.ShippingAddress.StateProvince.Abbreviation;
            //        shippingAddress.PostalCode = customer.ShippingAddress.ZipPostalCode;
            //        shippingAddress.Country = (CountryCodeType)Enum.Parse(typeof(CountryCodeType), customer.ShippingAddress.Country.TwoLetterIsoCode, true);
            //        details.PaymentDetails.ShipToAddress = shippingAddress;
            //    }
            //}

            //send request
            var service = GetService();
            DoDirectPaymentResponseType response = service.DoDirectPayment(req);

            string error;
            bool success = PaypalHelper.CheckSuccess(response, out error);
            if (success)
            {
                result.AvsResult = response.AVSCode;
                result.AuthorizationTransactionCode = response.CVV2Code;
                if (authorizeOnly)
                {
                    result.AuthorizationTransactionId = response.TransactionID;
                    result.AuthorizationTransactionResult = response.Ack.ToString();

                    result.NewPaymentStatus = PaymentStatus.Authorized;
                }
                else
                {
                    result.CaptureTransactionId = response.TransactionID;
                    result.CaptureTransactionResult = response.Ack.ToString();

                    result.NewPaymentStatus = PaymentStatus.Paid;
                }
            }
            else
            {
                result.AddError(error);
            }
            return result;
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIpn(string formString, out Dictionary<string, string> values)
        {
            var req = (HttpWebRequest)WebRequest.Create(GetPaypalUrl());
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            //now PayPal requires user-agent. otherwise, we can get 403 error
            req.UserAgent = HttpContext.Current.Request.UserAgent;

            string formContent = string.Format("{0}&cmd=_notify-validate", formString);
            req.ContentLength = formContent.Length;

            using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
            {
                sw.Write(formContent);
            }

            string response;
            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = HttpUtility.UrlDecode(sr.ReadToEnd());
            }
            bool success = response.Trim().Equals("VERIFIED", StringComparison.OrdinalIgnoreCase);

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (string l in formString.Split('&'))
            {
                string line = l.Trim();
                int equalPox = line.IndexOf('=');
                if (equalPox >= 0)
                    values.Add(line.Substring(0, equalPox), line.Substring(equalPox + 1));
            }

            return success;
        }

        #endregion


        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return AuthorizeOrSale(processPaymentRequest, false);
        }

        public bool SupportCapture
        {
            get
            {
                return true;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }
    }
}
