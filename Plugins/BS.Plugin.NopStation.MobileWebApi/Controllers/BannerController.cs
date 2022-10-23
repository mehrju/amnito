using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Stores;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    //[Route("api/[controller]")]
    public class BannerController : BaseApiController
    {

        //#region Field
        //private readonly IPluginFinder _pluginFinder;
        //private readonly IStoreService _storeService;
        //private readonly IWorkContext _workContext;
        //private readonly ISettingService _settingService;
        //private readonly IPictureService _pictureService;
        //private readonly IBS_SliderService _bsSliderService;
        
        //#endregion

        //#region Ctor
        //public BannerController(IPluginFinder pluginFinder,
        //    IStoreService storeService,
        //    IWorkContext workContext,
        //    ISettingService settingService,
        //    IPictureService pictureService,
        //    IBS_SliderService bsSliderService)
        //{
        //    this._pluginFinder = pluginFinder;
        //    this._storeService = storeService;
        //    this._workContext = workContext;
        //    this._settingService = settingService;
        //    this._pictureService = pictureService;
        //    this._bsSliderService = bsSliderService;
        //}
        //#endregion

        //#region Utility

        //private HomePageBannerResponseModel.BannerModel GetPictureUrl(int pictureId)
        //{
        //    var imageUrl = _pictureService.GetPictureUrl(pictureId, 300, showDefaultPicture: false);
        //    var picture = new HomePageBannerResponseModel.BannerModel()
        //    {
        //        ImageUrl = imageUrl ==string.Empty?null:imageUrl
        //    };
        //    return picture;
        //}
        //#endregion

        //#region Action Method
        //[Route("api/homepagebanner")]
        //[HttpGet]
        //public IActionResult HomePageBanner()
        //{
            
        //    var result = new HomePageBannerResponseModel();
        //    //var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
        //    //var nivoSliderSettings = _settingService.LoadSetting<BsNopMobileSettings>(storeScope);
        //    //int campaignType = type;
        //    var sliderDomainList = _bsSliderService.GetBSSliderImagesByDate();

        //    var pictureList = (from sliderDomain in sliderDomainList
        //                       let picture = _pictureService.GetPictureById(sliderDomain.PictureId)
        //                       select new HomePageBannerResponseModel.BannerModel
        //                       {
                               
        //                           ImageUrl = _pictureService.GetPictureUrl(picture),
        //                           Text = "",
        //                           Link = "",
        //                           IsProduct = Convert.ToInt32(sliderDomain.IsProduct),
        //                           ProdOrCatId = Convert.ToString(sliderDomain.ProdOrCatId),
        //                       }).ToList();

        //    result.IsEnabled = pictureList.Count > 0;

        //    result.Data = pictureList;

        //    return Ok(result);
        //}
        //#endregion
    }
}
