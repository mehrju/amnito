using Nop.Core.Domain.Customers;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface ISepService
    {
        Customer AuthenticatItSazUser(out string msg, string mobile);
        Customer AuthenticatSepUser(out string msg, string mobile);
        bool IsValidCustomer(Customer customer);
        Customer Register(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false);
        Customer RegisterForNotCurrentCustomer(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false);
    }
}