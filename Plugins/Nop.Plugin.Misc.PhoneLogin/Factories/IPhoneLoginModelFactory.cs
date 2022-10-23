using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Misc.PhoneLogin.Models;
using Nop.Web.Models.Customer;

namespace Nop.Plugin.Misc.PhoneLogin.Factories
{
    public interface IPhoneLoginModelFactory
    {
        Nop.Plugin.Misc.PhoneLogin.Models.LoginModel PrepareLoginModel(bool? checkoutAsGuest);

        Nop.Plugin.Misc.PhoneLogin.Models.RegisterModel PrepareRegisterModel(Nop.Plugin.Misc.PhoneLogin.Models.RegisterModel model, bool excludeProperties, string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false);
    }
}
