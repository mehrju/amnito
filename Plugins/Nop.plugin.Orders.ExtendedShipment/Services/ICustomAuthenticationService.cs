using Microsoft.AspNetCore.Authentication;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ICustomAuthenticationService
    {
        void SignIn(Customer customer, bool isPersistent,string SuperAdminCode=null);
        void SignOut();
        Customer GetAuthenticatedCustomer();
        bool IsSupperAdmin();
        string getSupperAdminCode();
    }
}
