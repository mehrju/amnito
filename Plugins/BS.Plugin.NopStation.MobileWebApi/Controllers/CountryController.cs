using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Factories;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Directory;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Directory;
using Nop.Services.Localization;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class CountryController : BaseApiController
    {
        #region Fields

        private readonly ICountryModelFactoryApi _countryModelFactoryApi;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ICacheManager _cacheManager;
        #endregion

        #region Constructors

        public CountryController(ICountryModelFactoryApi countryModelFactoryApi,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ICacheManager cacheManager)
        {
            this._countryModelFactoryApi = countryModelFactoryApi;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        #endregion

        #region States / provinces
        [Route("api/town/getTowns")]
        [HttpGet]
        public IActionResult GetTowns()
        {
            var data = _stateProvinceService.GetStateProvinces().Select(p => new
            {
                townName = p.Name,
                townId = p.Id,
                stateId = p.CountryId
            }).ToList();
            return Json(data);
        }
        [Route("api/town/getTownsByStateId")]
        [HttpGet]
        public IActionResult GetTownsByCountryId(int stateId)
        {
            var data = _stateProvinceService.GetStateProvincesByCountryId(stateId).Select(p => new
            {
                townName = p.Name,
                townId = p.Id,
                stateId = p.CountryId
            }).ToList();
            return Json(data);
        }
        [Route("api/state/getState")]
        [HttpGet]
        public IActionResult GetState()
        {
            var data = _countryService.GetAllCountries().Select(p => new
            {
                stateName = p.Name,
                stateId = p.Id,
            }).ToList();
            return Json(data);
        }
        #endregion
    }
}
