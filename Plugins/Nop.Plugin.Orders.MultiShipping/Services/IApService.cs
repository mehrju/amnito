using Nop.Core.Domain.Customers;
using Nop.Plugin.Orders.MultiShipping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface IApService
    {
        bool AuthenticateMyIrancell(string token);
        bool VarifyPayment(ApHostResponce model, out string error);
        Customer AuthenticatApUser(out string msg, string authToken);
        bool IsValidCustomer(Customer customer);
        Customer Register(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false);
        Customer RegisterUnknown(string mobileNo, out string msg, int affilateId = 0, bool PasswordSameUserName = false, bool sendSms = false);
        _paymentRequest CreatePaymentRequest(int orderId, out string error);
        _paymentRequest CreatePaymentRequestForCatoon(int orderId, out string error);
        Customer AuthHyperJet(string mobile, out string msg, int affilateId = 0);
    }
}
