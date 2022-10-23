using FluentValidation;
using FluentValidation.Attributes;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{

    public class CustomerInfModel
    {
        public CustomerInfoModel CustomerInfoModel { get; set; }

        public Nop.Web.Models.Customer.ChangePasswordModel ChangePasswordModel { get; set; }
    }

    [Validator(typeof(CustomerInfoModelValidator))]
    public class CustomerInfoModel
    {
        public string Email { get; set; }

        public string Phone { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ShebaNo { get; set; }

        public string AvatarUrl { get; set; }
    }

    public class CustomerInfoModelValidator : BaseNopValidator<CustomerInfoModel>
    {
        public CustomerInfoModelValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Email.Required"));
            RuleFor(x => x.Email).EmailAddress().WithMessage(localizationService.GetResource("Common.WrongEmail"));
            RuleFor(x => x.FirstName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.FirstName.Required"));
            RuleFor(x => x.LastName).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.LastName.Required"));
            RuleFor(x => x.Phone).NotEmpty().WithMessage(localizationService.GetResource("Account.Fields.Phone.Required"));
        }
    }
}