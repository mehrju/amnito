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
    public partial class PasswordRecoveryValidator : BaseNopValidator<PasswordRecoveryModel>
    {
        public PasswordRecoveryValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.MobileNumber).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.PhoneLogin.RequiredMobileNumber"));
        }
    }
}
