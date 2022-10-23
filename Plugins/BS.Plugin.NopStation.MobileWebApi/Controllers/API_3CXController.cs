using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class API_3CXController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        public API_3CXController
            (
                IWorkContext workContext,
                IDbContext dbContext,
                IPermissionService permissionService,
                ICustomerService customerService,
                IStaticCacheManager cacheManager,
                ILocalizationService localizationService,
                ICustomerActivityService customerActivityService
            )
        {
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
        }

        
        [HttpGet("api/API_3CX/Search/nubmer={Nubmer}")]
        public JsonResult Search(int Nubmer)
        {
            var result = (dynamic)null;

            return Json(result);
        }

        }
}
