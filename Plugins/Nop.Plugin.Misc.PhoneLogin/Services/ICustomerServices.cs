using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Customers;
using Nop.Services.Customers;

namespace Nop.Plugin.Misc.PhoneLogin.Services
{
    public partial interface ICustomerServices
    {
        bool ChangePassword(Customer customer, string newPassword, out string msg);
    }
}
