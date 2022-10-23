using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Mvc.Models;
using System;
using System.Runtime.CompilerServices;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Models
{
    public class PaymentRulesSettingsModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRulesSettings.Enabled")]
        public bool Enabled
        {
            get;
            set;
        }

        private bool _IsRegisted = true;
        public bool IsRegisted
        {
            get => _IsRegisted;
            set => _IsRegisted = true;
        }

        [NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRulesSettings.SerialNumber")]
        public string SerialNumber
        {
            get;
            set;
        }

        [NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRulesSettings.showDebugInfo")]
        public bool showDebugInfo
        {
            get;
            set;
        }

        [NopResourceDisplayName("Admin.FoxNetSoft.PaymentRules.PaymentRulesSettings.StoreUrl")]
        public string StoreUrl
        {
            get;
            set;
        }

        public PaymentRulesSettingsModel()
        {

        }
    }
}