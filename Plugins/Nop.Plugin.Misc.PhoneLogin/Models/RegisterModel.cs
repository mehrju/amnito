using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Validators.Customer;

namespace Nop.Plugin.Misc.PhoneLogin.Models
{
    [Validator(typeof(RegisterValidator))]
    public partial class RegisterModel : Nop.Web.Models.Customer.RegisterModel
    {
        public RegisterModel()
        {
        }

        [NopResourceDisplayName("Plugins.Misc.PhoneLogin.ActivationCode")]
        public string ActivationCode { get; set; }
        public String NationalCode { get; set; }
        public String CountryCodeNum { get; set; }
        public String CodeTpye { get; set; }

    }
}
