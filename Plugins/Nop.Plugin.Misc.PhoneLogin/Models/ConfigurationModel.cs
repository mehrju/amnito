using FluentValidation.Attributes;
using Nop.Plugin.Misc.PhoneLogin.Validators;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;

namespace Nop.Plugin.Misc.PhoneLogin.Models
{
    [Validator(typeof(ConfigurationModelValidator))]
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Misc.PhoneLogin.Fields.LineNumber")]
        public string LineNumber { get; set; }
        public bool LineNumberOverride_ForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.PhoneLogin.Fields.ApiKey")]
        public string ApiKey { get; set; }
        public bool ApiKeyOverride_ForStore { get; set; }

        [NopResourceDisplayName("Plugins.Misc.PhoneLogin.Fields.Enabled")]
        public bool Enabled { get; set; }
        public bool EnabledOverride_ForStore { get; set; }

    }
}