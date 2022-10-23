using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Nop.Plugin.Misc.PhoneLogin.Validators;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Validators.Customer;

namespace Nop.Plugin.Misc.PhoneLogin.Models
{
    [Validator(typeof(MobileProviderModelValidator))]
    public partial class LoginModel : Nop.Web.Models.Customer.LoginModel
    {
    }
}
