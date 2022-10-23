using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Models;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ISekehService
    {
        string EncryptData(string MobileNumber);
        bool Authenticate(string HashPassword);
        SekehOutputModel GetToken(SekehInputModel inputModel);
        string VerifyToken(string token);
        bool IsValidCustomer(Customer customer);
        Customer Register(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false);
    }
}
