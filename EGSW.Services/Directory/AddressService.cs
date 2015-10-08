using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services.Directory
{
    public partial class AddressService : IAddressService
    {
        #region Constants
        

        #endregion

        #region Fields

        private readonly IRepository<Address> _addressRepository;      

        #endregion

        #region Ctor

        
        public AddressService(
            IRepository<Address> addressRepository
            )
        {
            
            this._addressRepository = addressRepository;
           
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void DeleteAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _addressRepository.Delete(address);

           
        }

        

       

      
        public virtual Address GetAddressById(int addressId)
        {
            if (addressId == 0)
                return null;

            
            return  _addressRepository.GetById(addressId);
        }

        /// <summary>
        /// Inserts an address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void InsertAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            address.CreatedOnUtc = DateTime.UtcNow;            

            _addressRepository.Insert(address);

            
        }

        /// <summary>
        /// Updates the address
        /// </summary>
        /// <param name="address">Address</param>
        public virtual void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

           

            _addressRepository.Update(address);

           
        }

        /// <summary>
        /// Gets a value indicating whether address is valid (can be saved)
        /// </summary>
        /// <param name="address">Address to validate</param>
        /// <returns>Result</returns>
        public virtual bool IsAddressValid(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");            

            if (String.IsNullOrWhiteSpace(address.Email))
                return false;
           
            return true;
        }

        #endregion
    }
}
