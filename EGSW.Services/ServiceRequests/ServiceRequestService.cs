using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services.ServiceRequests
{
    public partial class ServiceRequestService:IServiceRequestService
    {
        private readonly IRepository<ServiceRequest> _serviceRequestRepository;

        public ServiceRequestService(IRepository<ServiceRequest> serviceRequestRepository)
        {
            this._serviceRequestRepository = serviceRequestRepository;
        }

        public void InsertServiceRequest(ServiceRequest entity)
        {
            if (entity == null)
                throw new ArgumentNullException("ServiceRequest");

            _serviceRequestRepository.Insert(entity);
        }
    }
}
