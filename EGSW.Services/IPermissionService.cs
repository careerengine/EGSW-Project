using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services
{
    public partial interface IPermissionService
    {
        bool AuthorizeAdminAccess();


        bool AuthorizeAdminRoleAccess();
    }
}
