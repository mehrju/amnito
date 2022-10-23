using System;

using Nop.Web.Framework.Mvc.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopFarsi.Payments.AsanPardakht.Models
{

    public class ConfigurationModel : BaseNopModel
    {

        public int ActiveStoreScopeConfiguration { get; set; }


        [NopResourceDisplayName("NopFarsi.AsanPardakht.MerchantId")]
        public int MerchantId { get; set; }
        public bool MerchantIdOverride_ForStore { get; set; }


        [NopResourceDisplayName("NopFarsi.AsanPardakht.ConfigMerchentId")]
        public int ConfigMerchentId { get; set; }
        public bool ConfigMerchentId_ForStore { get; set; }
        [NopResourceDisplayName("NopFarsi.AsanPardakht.UserNameMerchent")]
        public string UserNameMerchent { get; set; }
        public bool UserNameMerchent_ForStore { get; set; }
        [NopResourceDisplayName("NopFarsi.AsanPardakht.PassMerchent")]
        public string PassMerchent { get; set; }
        public bool PassMerchent_ForStore { get; set; }
        [NopResourceDisplayName("NopFarsi.AsanPardakht.Key")]
        public string Key { get; set; }
        public bool Key_ForStore { get; set; }
        [NopResourceDisplayName("NopFarsi.AsanPardakht.VectorEncriptor")]
        public string VectorEncriptor { get; set; }
        public bool VectorEncriptor_ForStore { get; set; }


        [NopResourceDisplayName("NopFarsi.AsanPardakht.IsToman")]
        public bool IsTomanForStore { get; set; }


        public bool IsTomanOverrideForStore { get; set; }



        [NopResourceDisplayName("NopFarsi.AsanPardakht.DisablePaymentInfo")]
        public bool DisablePaymentInfoForStore { get; set; }

        public bool DisablePaymentInfoOverrideForStore { get; set; }

        public string NopFarsi { get; set; }
    }
}
