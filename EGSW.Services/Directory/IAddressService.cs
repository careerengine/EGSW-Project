using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services.Directory
{
    /// <summary>
    /// Address service interface
    /// </summary>
    public partial interface IAddressService
    {
       
        void DeleteAddress(Address address);

        
       
        
        Address GetAddressById(int addressId);

        
        void InsertAddress(Address address);

       
        void UpdateAddress(Address address);

       
        bool IsAddressValid(Address address);
    }
}
