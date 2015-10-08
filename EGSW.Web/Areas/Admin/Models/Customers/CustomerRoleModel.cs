using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGSW.Web.Areas.Admin.Models.Customers
{
    public class CustomerRoleModel
    {            
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool IsSystemRole { get; set; }
        public string SystemName { get; set; }
    }
}