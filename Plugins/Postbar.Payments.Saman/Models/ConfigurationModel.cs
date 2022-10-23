using System;

using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopFarsi.Payments.SepShaparak.Models
{

    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Postbar.Saman.MerchantId")]
        public int MerchantId { get; set; }
        public bool MerchantIdOverride_ForStore { get; set; }

        //[NopResourceDisplayName("Postbar.Saman.ConfigMerchentId")]
        //public int ConfigMerchentId { get; set; }
        //public bool ConfigMerchentId_ForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.IsToman")]
        public bool IsTomanForStore { get; set; }
        public bool IsTomanOverrideForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.DisablePaymentInfo")]
        public bool DisablePaymentInfoForStore { get; set; }
        public bool DisablePaymentInfoOverrideForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.RefundUserName")]
        public string RefundUserName { get; set; }
        public bool RefundUserNameOverride_ForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.RefundPassword")]
        public string RefundPassword { get; set; }
        public bool RefundPasswordOverride_ForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.RefundEmail")]
        public string RefundEmail { get; set; }
        public bool RefundEmailOverride_ForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.RefundCellPhone")]
        public string RefundCellPhone { get; set; }
        public bool RefundCellPhoneOverride_ForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.TransactionTermId")]
        public long TransactionTermId { get; set; }
        public bool TransactionTermIdOverride_ForStore { get; set; }

        [NopResourceDisplayName("Postbar.Saman.RefundTermId")]
        public long RefundTermId { get; set; }
        public bool RefundTermIdOverride_ForStore { get; set; }
    }
}