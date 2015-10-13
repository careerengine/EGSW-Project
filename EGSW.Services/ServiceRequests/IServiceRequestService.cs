using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services.ServiceRequests
{
    public partial interface IServiceRequestService
    {
        void InsertServiceRequest(ServiceRequest entity);
    }
}
