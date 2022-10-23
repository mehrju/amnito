using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Models;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class DiscountsResponseModel : BaseResponse
    {
        public DiscountsResponseModel()
        {
            DiscountModels = new List<DiscountModel>();
        }

        public IList<DiscountModel> DiscountModels { get; set; }
    }

    #region Nested Class
    public partial class DiscountModel : BaseNopEntityModel
    {
        public string CouponCode { get; set; } 
    }
    #endregion
}
