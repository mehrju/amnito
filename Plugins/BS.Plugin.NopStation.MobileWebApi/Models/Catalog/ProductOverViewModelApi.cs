using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Web.Framework.Mvc.Models;
using PictureModel = BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel.PictureModel;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class ProductOverViewModelApi : BaseNopEntityModel
    {
        public ProductOverViewModelApi()
        {
            ProductPrice = new ProductOverviewModel.ProductPriceModel();
            DefaultPictureModel = new PictureModel();
            ReviewOverviewModel = new ProductReviewOverviewModel();
        }

        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        //price
        public ProductOverviewModel.ProductPriceModel ProductPrice { get; set; }
        //picture
        public PictureModel DefaultPictureModel { get; set; }
        //price
        public ProductReviewOverviewModel ReviewOverviewModel { get; set; }

		#region Nested Classes

        //public partial class ProductPriceModel 
        //{
        //    public string OldPrice { get; set; }
        //    public string Price {get;set;}
        //}
        
        public partial class ProductReviewOverviewModel 
        {
            public int ProductId { get; set; }
            public int RatingSum { get; set; }
            public bool AllowCustomerReviews { get; set; }
            public int TotalReviews { get; set; }
        }
		#endregion
    }
}
