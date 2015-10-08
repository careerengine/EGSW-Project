using EGSW.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services
{
    /// <summary>
    /// Customer registration request
    /// </summary>
    public class CustomerRegistrationRequest
    {
        /// <summary>
        /// Customer
        /// </summary>
        public Customer Customer { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Password
        /// </summary>
        public string Password { get; set; }
       
        /// <summary>
        /// Is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="email">Email</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <param name="passwordFormat">Password fprmat</param>
        /// <param name="isApproved">Is approved</param>
        public CustomerRegistrationRequest(Customer customer, string email, string password, bool isApproved = true)
        {
            this.Customer = customer;
            this.Email = email;            
            this.Password = password;            
            this.IsApproved = isApproved;
        }
    }
}
