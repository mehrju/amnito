using FluentValidation;
using Nop.Plugin.Misc.PhoneLogin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PhoneLogin.Validators
{
    public partial class ConfigurationModelValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.LineNumber).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.PhoneLogin.RequiredLineNumber"));
            RuleFor(x => x.ApiKey).NotEmpty().WithMessage(localizationService.GetResource("Plugins.Misc.PhoneLogin.RequiredApiKey"));
        }
    }
}
