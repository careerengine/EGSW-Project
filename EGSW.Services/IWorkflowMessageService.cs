using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services
{
    public partial interface IWorkflowMessageService
    {
        #region Customer workflow

        
        int SendCustomerRegisteredNotificationMessage(Customer customer);

        
        int SendCustomerWelcomeMessage(Customer customer);

        int SendContactUsMessage(ContactU contactUs);

        int SendCustomerPasswordRecoveryMessage(Customer customer);

        
        int SendCustomerEmailValidationMessage(Customer customer);


        int SendReferANeighborMessage(string FriendName, string FriendEmail, string YourName);

        int SendNewServiceRequestSiteOwnerNotification(string EmailAddress, string ZipCode);

        #endregion


        int SendOrderPlacedCustomerNotification(GutterCleanOrder order);

        int SendOrderPlacedSiteOwnerNotification(GutterCleanOrder order);

        int SendOrderAssigneAgentNotification(GutterCleanOrder order, int AgentId);

        int SendOrderPlacedWorkCompletedCustomerNotification(GutterCleanOrder order);
    }
}
