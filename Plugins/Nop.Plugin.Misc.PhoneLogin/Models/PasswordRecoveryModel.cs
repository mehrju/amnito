using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Attributes;
using Nop.Plugin.Misc.PhoneLogin.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Misc.PhoneLogin.Models
{
    [Validator(typeof(PasswordRecoveryValidator))]
    public partial class PasswordRecoveryModel : Nop.Web.Models.Customer.PasswordRecoveryModel
    {
        [NopResourceDisplayName("Plugins.Misc.PhoneLogin.MobileNumber")]
        public string MobileNumber { get; set; }
    }
    
}
