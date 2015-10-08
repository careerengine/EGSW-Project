using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGSW.Data;

namespace EGSW.Services.Directory
{
    public partial interface IZipCodeService
    {
        /// <summary>
        /// Get customer by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Customer</returns>
        ZipCode GetZipCodeDetailByZipcode(string zipcode);
    }
}
