using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Controllers
{
    public class GetByteAvatarCustomer_Service: IGetByteAvatarCustomer_Service
    {
        private readonly IRepository<Tbl_CheckAvatarCustomer> _repositoryTbl_CheckAvatarCustomer;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;

        public GetByteAvatarCustomer_Service
            (
             IRepository<Tbl_CheckAvatarCustomer> repositoryTbl_CheckAvatarCustomer,
        IWorkContext workContext,
        IDbContext dbContext,
        IPermissionService permissionService,
        ICustomerService customerService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        IPictureService pictureService,
        MediaSettings mediaSettings
            )
        {
            _repositoryTbl_CheckAvatarCustomer = repositoryTbl_CheckAvatarCustomer;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
        }

        public (bool, byte[]) GetByteAvatarCustomer(int CustomerId)
        {
            bool state = false;
            byte[] bs = null;
            Tbl_CheckAvatarCustomer temp = _repositoryTbl_CheckAvatarCustomer.Table.Where(p => p.CustomerId == CustomerId).OrderByDescending(p => p.Id).FirstOrDefault();
            //اگر عکس مورد تایید قرارگرفته بود
            if (temp.StateVerify == 2)
            {
                try
                {
                    Customer c = _customerService.GetCustomerById(temp.CustomerId);
                    string AvatarUrl = _pictureService.GetPictureUrl(
                                                           c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                                           _mediaSettings.AvatarPictureSize,
                                                           false);
                    if (string.IsNullOrEmpty(AvatarUrl))
                    {
                        state = true;
                        bs = Encoding.ASCII.GetBytes(AvatarUrl);
                    }
                }
                catch
                {

                }
                state = true;


            }

            return (state, bs);
        }
    }
}
