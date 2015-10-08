using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGSW.Web.Models.Customers
{
    public partial class CustomerAddressListModel 
    {
        public CustomerAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
    }
}