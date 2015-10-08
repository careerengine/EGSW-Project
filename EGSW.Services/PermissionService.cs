using EGSW.Data;
using EGSW.Data.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services
{
    public partial class PermissionService : IPermissionService
    {
        private readonly IWorkContext _workContext;

        public PermissionService(IWorkContext workContext)
        {
            this._workContext = workContext;
        }
        public bool AuthorizeAdminAccess()
        {
            var customer = _workContext.CurrentCustomer;

            return (customer.IsAdmin() || customer.IsAgent());

        }


        public bool AuthorizeAdminRoleAccess()
        {
            var customer = _workContext.CurrentCustomer;

            return (customer.IsAdmin());
        }
    }
}
