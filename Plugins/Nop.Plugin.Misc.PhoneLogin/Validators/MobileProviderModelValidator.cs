using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.PhoneLogin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using FluentValidation;

namespace Nop.Plugin.Misc.PhoneLogin.Validators
{
    public class MobileProviderModelValidator : BaseNopValidator<LoginModel>
    {
        public MobileProviderModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.PhoneLogin.Fields.UserName.Required"));
            RuleFor(x => x.Password).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.PhoneLogin.Fields.Password.Required"));
           // RuleFor(x => x.RememberMe).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.PhoneLogin.Fields.Password.Required"));
        }
    }
}
