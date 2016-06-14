using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web;
using System.Net.Mail;
using System.Collections;
using EGSW.Services.Directory;

namespace EGSW.Services
{
    public partial class WorkflowMessageService : IWorkflowMessageService
    {
        private readonly IWebHelper _webHelper;
        private readonly IZipCodeService _zipCodeService;
        private readonly IQuestionAnswerEntityData _questionAnswerEntityDataService;
        private readonly IDateTimeHelper _dateTimeHelper;
        SiteSetting _siteSetting;
        private readonly ICustomerService _customerService;
        public WorkflowMessageService(IWebHelper webHelper, IZipCodeService zipCodeService,
            IQuestionAnswerEntityData questionAnswerEntityDataService, IDateTimeHelper dateTimeHelper
            , ICustomerService customerService)
        {
            this._webHelper = webHelper;
            this._zipCodeService = zipCodeService;
            _siteSetting = new SiteSetting();
            this._questionAnswerEntityDataService = questionAnswerEntityDataService;
            this._dateTimeHelper =dateTimeHelper;
            this._customerService = customerService;
        }

        protected virtual string GetSiteUrl()
        {

            return ConfigurationManager.AppSettings["SiteUrl"];
        }


        public int SendCustomerRegisteredNotificationMessage(Customer customer)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerWelcomeMessage(Customer customer)
        {
            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/CustomerWelcomeMessage.htm"));

            string passwordRecoveryUrl = string.Format("{0}resetpassword?token={1}&email={2}", GetSiteUrl(), customer.PasswordRecoveryToken, HttpUtility.UrlEncode(customer.Email));
            mailBody = mailBody.Replace("%customerfullname%", customer.FirstName+" "+customer.LastName);
            mailBody = mailBody.Replace("%customeremail%", customer.Email);
            mailBody = mailBody.Replace("%customerpassword%", customer.Password);
            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);

            var BCC = _siteSetting.BCCEmail + "," + "info@ericsguttercleaning.com";

            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, customer.Email, "", BCC, mailBody, "Eric's Gutter Cleaning Registration Confirmation");

            return 1;
        }


        public int SendContactUsMessage(ContactU contactUs)
        {
            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/CustomerContactUs.htm"));

            
            mailBody = mailBody.Replace("%contactname%", contactUs.FirstName + " " + contactUs.LastName);
            mailBody = mailBody.Replace("%phoneno%", contactUs.PhoneNo);
            mailBody = mailBody.Replace("%email%", contactUs.Email);
            mailBody = mailBody.Replace("%message%", contactUs.Message);
            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);

            var BCC = _siteSetting.BCCEmail;
            //if (!_siteSetting.SenderName.Contains("Test"))
            //{
              //  BCC = BCC + ",info@ericsguttercleaning.com";
            //}
            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, contactUs.Email, "info@ericsguttercleaning.com", _siteSetting.BCCEmail, mailBody, " Eric's Gutter Cleaning - Thank you for contacting us!");

            return 1;
        }

        public int SendReferANeighborMessage(string FriendName, string FriendEmail, string YourName)
        {
            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/SendReferANeighborNotification.htm"));


            mailBody = mailBody.Replace("%friendname%", FriendName);
            mailBody = mailBody.Replace("%yourname%", YourName);            
            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);

            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, FriendEmail, "", _siteSetting.BCCEmail, mailBody, "Refer A Neighbor - Eric's Gutter Cleaning Service");

            return 1;
        }

        public int SendNewServiceRequestSiteOwnerNotification(string EmailAddress, string ZipCode)
        {
            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/NewServiceRequestSiteOwnerNotification.htm"));


            mailBody = mailBody.Replace("%friendname%", EmailAddress);
            mailBody = mailBody.Replace("%yourname%", ZipCode);
            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);

            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, _siteSetting.SenderEmail, "", _siteSetting.BCCEmail, mailBody, "Eric's Gutter Cleaning - New Service Request!");

            return 1;
        }

        public int SendCustomerEmailValidationMessage(Customer customer)
        {
            throw new NotImplementedException();
        }

        public int SendCustomerPasswordRecoveryMessage(Customer customer)
        {
            string mailBody =System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/CustomerPasswordRecoveryMessage.htm"));

            string passwordRecoveryUrl = string.Format("{0}resetpassword?token={1}&email={2}", GetSiteUrl(), customer.PasswordRecoveryToken, HttpUtility.UrlEncode(customer.Email));
            mailBody = mailBody.Replace("%PasswordRecoveryURL%", passwordRecoveryUrl);
            mailBody = mailBody.Replace("%customerfullname%", customer.FirstName + " " + customer.LastName);
            mailBody = mailBody.Replace("%customeremail%", customer.Email);            
            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);


            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, customer.Email, "", _siteSetting.BCCEmail, mailBody, "Eric's Gutter Cleaning Password Reset");

            return 1;
        }


        public int SendOrderPlacedCustomerNotification(GutterCleanOrder order)
        {
            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/OrderPlacedCustomerNotification.htm"));

            var zipcodedetail = _zipCodeService.GetZipCodeDetailByZipcode(order.Zipcode);
            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == order.QuestionStyleOfGutter).SingleOrDefault();
            var year = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == order.QuestionYearBuilt.ToString()).SingleOrDefault();


            string OrderURLForCustomer = string.Format("{0}order-details/{1}", GetSiteUrl(),
                order.Id);

            mailBody = mailBody.Replace("%Order.OrderURLForCustomer%", OrderURLForCustomer);

            mailBody = mailBody.Replace("%customerfullname%", order.Customer.FirstName + " " + order.Customer.LastName);
            mailBody = mailBody.Replace("%orderid%", order.Id.ToString());
            mailBody = mailBody.Replace("%createddate%", _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString());
            mailBody = mailBody.Replace("%completiondate%", "");
            mailBody = mailBody.Replace("%ordertotal%", string.Format("{0:C}",order.OrderTotal));

            
            mailBody = mailBody.Replace("%street%", order.Address);
            if (zipcodedetail!=null)
            mailBody = mailBody.Replace("%citystate%", zipcodedetail.CityName+", "+zipcodedetail.StateName);
            else
                mailBody = mailBody.Replace("%citystate%", "");
            mailBody = mailBody.Replace("%zipcode%", order.Zipcode);

            
            mailBody = mailBody.Replace("%squarefootage%", order.QuestionSquareFootage.ToString());
            mailBody = mailBody.Replace("%styleofgutters%", r.Text);
            mailBody = mailBody.Replace("%yearbuilt%", year.Text);
            mailBody = mailBody.Replace("%roofmaterial%", order.RoofMaterial);

            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);
            mailBody = mailBody.Replace("%customeremail%", order.Customer.Email);

            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, order.Customer.Email, "", "", mailBody, "Eric's Gutter Cleaning Request | Order #: " + order.Id);

            return 1;
        }


        public int SendOrderPlacedSiteOwnerNotification(GutterCleanOrder order)
        {
            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/OrderPlacedSiteOwnerNotification.htm"));

            string OrderURLForCustomer = string.Format("{0}order-details/{1}", GetSiteUrl(),
                order.Id);

            var zipcodedetail = _zipCodeService.GetZipCodeDetailByZipcode(order.Zipcode);
            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == order.QuestionStyleOfGutter).SingleOrDefault();
            var year = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == order.QuestionYearBuilt.ToString()).SingleOrDefault();


            
            mailBody = mailBody.Replace("%Order.OrderURLForCustomer%", OrderURLForCustomer);

            mailBody = mailBody.Replace("%username%", order.Customer.Email);
            mailBody = mailBody.Replace("%customerfullname%", order.Customer.FirstName + " " + order.Customer.LastName);
            mailBody = mailBody.Replace("%customeremail%", order.Customer.Email);

            mailBody = mailBody.Replace("%orderid%", order.Id.ToString());
            mailBody = mailBody.Replace("%orderidURL%", _siteSetting.SiteUrl + "Admin/GutterOrder/OrderDetail?orderId=" + order.Id.ToString());
            mailBody = mailBody.Replace("%createddate%", _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString());
            mailBody = mailBody.Replace("%completiondate%", "");
            mailBody = mailBody.Replace("%ordertotal%", string.Format("{0:C}", order.OrderTotal));


            mailBody = mailBody.Replace("%street%", order.Address);
            if (zipcodedetail != null)
                mailBody = mailBody.Replace("%citystate%", zipcodedetail.CityName + ", " + zipcodedetail.StateName);
            else
                mailBody = mailBody.Replace("%citystate%", "");
            mailBody = mailBody.Replace("%zipcode%", order.Zipcode);


            mailBody = mailBody.Replace("%squarefootage%", order.QuestionSquareFootage.ToString());
            mailBody = mailBody.Replace("%styleofgutters%", r.Text);
            mailBody = mailBody.Replace("%yearbuilt%", year.Text);
            mailBody = mailBody.Replace("%roofmaterial%", order.RoofMaterial);

            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);


            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, _siteSetting.AdminEmail, "info@ericsguttercleaning.com", _siteSetting.BCCEmail, mailBody, "NEW Order #: " + order.Id.ToString());

            return 1;
        }


        public int SendOrderAssigneAgentNotification(GutterCleanOrder order, int AgentId)
        {
            var AgentDetail = _customerService.GetCustomerById(AgentId);

            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/OrderAssigneAgentNotification.htm"));

            string OrderURLForCustomer = string.Format("{0}order-details/{1}", GetSiteUrl(),
                order.Id);

            var zipcodedetail = _zipCodeService.GetZipCodeDetailByZipcode(order.Zipcode);
            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == order.QuestionStyleOfGutter).SingleOrDefault();
            var year = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == order.QuestionYearBuilt.ToString()).SingleOrDefault();


            mailBody = mailBody.Replace("%agentfullname%", AgentDetail.FirstName + " " + AgentDetail.LastName);
            

            mailBody = mailBody.Replace("%orderid%", order.Id.ToString());
            mailBody = mailBody.Replace("%createddate%", _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString());
            mailBody = mailBody.Replace("%completiondate%", "");
            mailBody = mailBody.Replace("%ordertotal%", string.Format("{0:C}", order.OrderTotal));


            mailBody = mailBody.Replace("%street%", order.Address);
            if (zipcodedetail != null)
                mailBody = mailBody.Replace("%citystate%", zipcodedetail.CityName + ", " + zipcodedetail.StateName);
            else
                mailBody = mailBody.Replace("%citystate%", "");
            mailBody = mailBody.Replace("%zipcode%", order.Zipcode);


            mailBody = mailBody.Replace("%squarefootage%", order.QuestionSquareFootage.ToString());
            mailBody = mailBody.Replace("%styleofgutters%", r.Text);
            mailBody = mailBody.Replace("%yearbuilt%", year.Text);
            mailBody = mailBody.Replace("%roofmaterial%", order.RoofMaterial);

            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);


            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, AgentDetail.Email, "info@ericsguttercleaning.com", _siteSetting.BCCEmail, mailBody, "NEW Order #: " + order.Id.ToString());

            return 1;
        }


        public int SendOrderPlacedWorkCompletedCustomerNotification(GutterCleanOrder order)
        {
            string mailBody = System.IO.File.ReadAllText(_webHelper.MapPath("~/EmailTemplates/OrderPlacedWorkCompletedCustomerNotification.htm"));

            var zipcodedetail = _zipCodeService.GetZipCodeDetailByZipcode(order.Zipcode);
            var r = _questionAnswerEntityDataService.StyleOfGuttersAnswer().Where(i => i.Value == order.QuestionStyleOfGutter).SingleOrDefault();
            var year = _questionAnswerEntityDataService.YearBuiltAnswer().Where(i => i.Value == order.QuestionYearBuilt.ToString()).SingleOrDefault();


            string OrderURLForCustomer = string.Format("{0}order-details/{1}", GetSiteUrl(),
                order.Id);

            mailBody = mailBody.Replace("%Order.OrderURLForCustomer%", OrderURLForCustomer);

            mailBody = mailBody.Replace("%customerfullname%", order.Customer.FirstName + " " + order.Customer.LastName);
            mailBody = mailBody.Replace("%orderid%", order.Id.ToString());
            mailBody = mailBody.Replace("%createddate%", order.CreatedOnUtc.ToString());
            mailBody = mailBody.Replace("%completiondate%", "");
            mailBody = mailBody.Replace("%ordertotal%", string.Format("{0:C}", order.OrderTotal));


            mailBody = mailBody.Replace("%street%", order.Address);
            if (zipcodedetail != null)
                mailBody = mailBody.Replace("%citystate%", zipcodedetail.CityName + ", " + zipcodedetail.StateName);
            else
                mailBody = mailBody.Replace("%citystate%", "");
            mailBody = mailBody.Replace("%zipcode%", order.Zipcode);


            mailBody = mailBody.Replace("%squarefootage%", order.QuestionSquareFootage.ToString());
            mailBody = mailBody.Replace("%styleofgutters%", r.Text);
            mailBody = mailBody.Replace("%yearbuilt%", year.Text);
            mailBody = mailBody.Replace("%roofmaterial%", order.RoofMaterial);

            mailBody = mailBody.Replace("%rateurl%", _siteSetting.SiteUrl + "Survey/"+order.OrderGuid.ToString());

            mailBody = mailBody.Replace("%siteurl%", _siteSetting.SiteUrl);
            mailBody = mailBody.Replace("%customeremail%", order.Customer.Email);

            EmailAccess.SendMail(_siteSetting.SenderEmail, _siteSetting.SenderName, order.Customer.Email, "", _siteSetting.BCCEmail, mailBody, "Eric's Gutter Cleaning Order #: " + order.Id + "| Work Completed");

            return 1;
        }
    }


    public static class EmailAccess
    {

        /// <summary>
        /// Method to get array list of replacale words in the parametered string
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static ArrayList GetReplaceWords(string content)
        {
            ArrayList arr = new ArrayList();
            int indexHash = 0;
            while (content != "")
            {
                if (content.IndexOf("##") != -1)
                {
                    indexHash = content.IndexOf("##");

                    content = content.Substring(indexHash + 2);

                    arr.Add(content.Substring(0, content.IndexOf("##")));

                    content = content.Substring(content.IndexOf("##") + 2);
                }
                else
                    content = "";
            }
            return arr;
        }

        /// <summary>
        /// Send Text Mail
        /// </summary>
        /// <param name="Sender">string Sender</param>
        /// <param name="Receiver">string Receiver</param>
        /// <param name="CC">string CC</param>
        /// <param name="BCC">string BCC</param>
        /// <param name="Body">string body</param>
        /// <param name="subject">string subject</param>
        static public void SendMail(string Sender, string SenderName, string Receiver, string CC, string BCC, string Body, string subject)
        {
            SiteSetting _siteSetting = new SiteSetting();
            if (!string.IsNullOrEmpty(Sender) && !string.IsNullOrEmpty(Receiver))
            {
                if (string.IsNullOrEmpty(Receiver))
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ReceiverMail"]))
                    {
                        Receiver = ConfigurationManager.AppSettings["ReceiverMail"];
                        CC = ConfigurationManager.AppSettings["ReceiverMail"];
                        BCC = ConfigurationManager.AppSettings["ReceiverMail"];
                    }
                }
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(Sender, SenderName);
                mailMessage.ReplyToList.Add(new MailAddress(_siteSetting.ReturnEmail, ""));
                mailMessage.To.Add(Receiver);

                if (!string.IsNullOrEmpty(CC))
                    mailMessage.CC.Add(new MailAddress(CC));

                if (!string.IsNullOrEmpty(_siteSetting.BCCEmail))
                {
                    mailMessage.Bcc.Add(new MailAddress(_siteSetting.BCCEmail));
                    mailMessage.Bcc.Add(new MailAddress("circussite1@gmail.com"));
                }


                mailMessage.Subject = subject.Replace('\r', ' ').Replace('\n', ' ');
                mailMessage.Body = Body;

                mailMessage.IsBodyHtml = true;
                mailMessage.Priority = MailPriority.Normal;

                SmtpClient smtpClient = new SmtpClient();
                //smtpClient.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;

                try
                {
                    smtpClient.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


    }
}
