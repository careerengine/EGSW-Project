using EGSW.Data;
using EGSW.Data.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGSW.Services
{
    public partial class CustomerService : ICustomerService
    {
         private readonly IDbContext _context;
         private readonly IRepository<Customer> _customerRepository;
         private readonly IRepository<ContactU> _contactUsRepository;
         private readonly IRepository<CustomerRole> _customerRoleRepository;

         public CustomerService(IDbContext context, IRepository<Customer> customerRepository, IRepository<CustomerRole> customerRoleRepository, IRepository<ContactU> contactUsRepository)
        {
            this._context = context;
            this._customerRepository = customerRepository;
             this._customerRoleRepository=customerRoleRepository;
             this._contactUsRepository = contactUsRepository;
        }


         public virtual Customer GetCustomerById(int customerId)
         {
             if (customerId == 0)
                 return null;

             return _customerRepository.GetById(customerId);
         }

         /// <summary>
         /// Gets a customer role
         /// </summary>
         /// <param name="systemName">Customer role system name</param>
         /// <returns>Customer role</returns>
         public virtual CustomerRole GetCustomerRoleBySystemName(string systemName)
         {
             if (String.IsNullOrWhiteSpace(systemName))
                 return null;


             var query = from cr in _customerRoleRepository.Table
                         orderby cr.Id
                         where cr.SystemName == systemName
                         select cr;
             var customerRole = query.FirstOrDefault();
             return customerRole;
         }



         public IList<Customer> GetAllCustomers(int[] customerRoleIds = null)
        {
            var query = _customerRepository.Table;
            if (customerRoleIds != null && customerRoleIds.Length > 0)
                query = query.Where(c => c.CustomerRoles.Select(cr=>cr.Id).Intersect(customerRoleIds).Any());
            query = query.OrderByDescending(o => o.CreatedOnUtc);
            return query.ToList();
        }



         public PagedList.IPagedList<Customer> GetAllCustomers(int? customerId = null, string firstName = null, string lastName = null, string customerEmail = null, string zipCode = null, int pageIndex = 0, int pageSize = int.MaxValue, int[] customerRoleIds = null)
        {
            var query = _customerRepository.Table;
            query = query.Where(c => c.CustomerRoles.Any(cr => cr.Id != 3));

            if (customerId.HasValue)
                query = query.Where(o => o.Id==customerId.Value);

            if (!String.IsNullOrWhiteSpace(firstName))
                query = query.Where(o => o.FirstName.Contains(firstName));

            if (!String.IsNullOrWhiteSpace(lastName))
                query = query.Where(o => o.LastName.Contains(lastName));

            if (!String.IsNullOrWhiteSpace(customerEmail))
                query = query.Where(o => o.Email.Contains(customerEmail));

            if (!String.IsNullOrWhiteSpace(zipCode))
                query = query.Where(o => o.ZipPostalCode.Contains(zipCode));

            if (customerRoleIds != null && customerRoleIds.Length > 0)
                query = query.Where(c => c.CustomerRoles.Select(cr => cr.Id).Intersect(customerRoleIds).Any());

            query = query.OrderByDescending(o => o.CreatedOnUtc);
            //database layer paging
            return new PagedList.PagedList<Customer>(query, pageIndex, pageSize);
        }


        /// <summary>
        /// Gets a customer by GUID
        /// </summary>
        /// <param name="customerGuid">Customer GUID</param>
        /// <returns>A customer</returns>
        public virtual Customer GetCustomerByGuid(Guid customerGuid)
        {
            if (customerGuid == Guid.Empty)
                return null;

            var query = from c in _customerRepository.Table
                        where c.CustomerGuid == customerGuid
                        orderby c.Id
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Get customer by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns>Customer</returns>
        public virtual Customer GetCustomerByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Email == email
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        /// <summary>
        /// Get customer by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Customer</returns>
        public virtual Customer GetCustomerBySystemName(string systemName)
        {
            if (string.IsNullOrWhiteSpace(systemName))
                return null;

            var query = from c in _customerRepository.Table
                        orderby c.Id
                        where c.Email == "search@gmail.com"
                        select c;
            var customer = query.FirstOrDefault();
            return customer;
        }

        


        /// <summary>
        /// Insert a guest customer
        /// </summary>
        /// <returns>Customer</returns>
        public virtual Customer InsertGuestCustomer()
        {
            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };

            //add to 'Guests' role
            var guestRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Guests);
            if (guestRole == null)
                throw new EGSWException("'Guests' role could not be loaded");
            customer.CustomerRoles.Add(guestRole);

            _customerRepository.Insert(customer);

            return customer;
        }

        /// <summary>
        /// Insert a customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public virtual void InsertCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            _customerRepository.Insert(customer);

            
        }

        /// <summary>
        /// Updates the customer
        /// </summary>
        /// <param name="customer">Customer</param>
        public virtual void UpdateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            _customerRepository.Update(customer);            
        }

        public virtual CustomerLoginResults ValidateCustomer(string usernameOrEmail, string password)
        {
            Customer customer;           
           
                customer =GetCustomerByEmail(usernameOrEmail);

            if (customer == null)
                return CustomerLoginResults.CustomerNotExist;
            if (customer.Deleted)
                return CustomerLoginResults.Deleted;
            if (!customer.Active)
                return CustomerLoginResults.NotActive;
            
            ////only registered can login

            //if (!customer.IsRegistered())
            //    return CustomerLoginResults.NotRegistered;

            string pwd = "";
            pwd = password;
            //switch (customer.PasswordFormat)
            //{
            //    case PasswordFormat.Encrypted:
            //        pwd = _encryptionService.EncryptText(password);
            //        break;
            //    case PasswordFormat.Hashed:
            //        pwd = _encryptionService.CreatePasswordHash(password, customer.PasswordSalt, _customerSettings.HashedPasswordFormat);
            //        break;
            //    default:
            //        pwd = password;
            //        break;
            //}

            bool isValid = pwd == customer.Password;
            if (!isValid)
                return CustomerLoginResults.WrongPassword;

            //save last login date
            customer.LastLoginDateUtc = DateTime.UtcNow;
            UpdateCustomer(customer);
            return CustomerLoginResults.Successful;
        }


        /// <summary>
        /// Register customer
        /// </summary>
        /// <param name="request">Request</param>
        /// <returns>Result</returns>
        public virtual CustomerRegistrationResult RegisterCustomer(CustomerRegistrationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Customer == null)
                throw new ArgumentException("Can't load current customer");

            var result = new CustomerRegistrationResult();
            if (request.Customer.IsSearchEngineAccount())
            {
                result.AddError("Search engine can't be registered");
                return result;
            }

            
            if (request.Customer.IsRegistered())
            {
                result.AddError("Current customer is already registered");
                return result;
            }
            if (String.IsNullOrEmpty(request.Email))
            {
                result.AddError("Email Is Not Provided.");
                return result;
            }
            if (!CommonHelper.IsValidEmail(request.Email))
            {
                result.AddError("Wrong Email.");
                return result;
            }
            if (String.IsNullOrWhiteSpace(request.Password))
            {
                result.AddError("Password Is Not Provided");
                return result;
            }
            

           // validate unique user
            if (GetCustomerByEmail(request.Email) != null)
            {
                result.AddError("Email Already Exists.");
                return result;
            }
            

            //at this point request is valid
            //request.Customer.Username = request.Username;
            request.Customer.Email = request.Email;
            //request.Customer.PasswordFormat = request.PasswordFormat;

            request.Customer.Password = request.Password;

            switch (PasswordFormat.Clear)
            {
                case PasswordFormat.Clear:
                    {
                        request.Customer.Password = request.Password;
                    }
                    break;
                default:
                    break;
                //case PasswordFormat.Encrypted:
                //    {
                //        request.Customer.Password = _encryptionService.EncryptText(request.Password);
                //    }
                //    break;
                //case PasswordFormat.Hashed:
                //    {
                //        string saltKey = _encryptionService.CreateSaltKey(5);
                //        request.Customer.PasswordSalt = saltKey;
                //        request.Customer.Password = _encryptionService.CreatePasswordHash(request.Password, saltKey, _customerSettings.HashedPasswordFormat);
                //    }
                //    break;
                
            }

            request.Customer.Active = request.IsApproved;

            //add to 'Registered' role
            var registeredRole = GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered);
            if (registeredRole == null)
                throw new EGSWException("'Registered' role could not be loaded");
            request.Customer.CustomerRoles.Add(registeredRole);
            //remove from 'Guests' role
            var guestRole = request.Customer.CustomerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests);
            if (guestRole != null)
                request.Customer.CustomerRoles.Remove(guestRole);

           
            UpdateCustomer(request.Customer);
            return result;
        }


        public void InsertContactUs(ContactU contactUs)
        {
            if (contactUs == null)
                throw new ArgumentNullException("contactUs");

            _contactUsRepository.Insert(contactUs);
        }



        public IList<CustomerRole> GetAllCustomerRoles(bool showHidden = false)
        {
            
                var query = from cr in _customerRoleRepository.Table
                            orderby cr.Name
                            where (showHidden || cr.Active)
                            select cr;
                var customerRoles = query.ToList();
                return customerRoles;
            
        }
    }
}
