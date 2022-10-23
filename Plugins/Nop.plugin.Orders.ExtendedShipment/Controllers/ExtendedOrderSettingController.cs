using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Linq;
using Nop.Core.Data;
using Nop.Services.Directory;
using System;
using Nop.Services.Tasks;
using Nop.Core.Domain.Tasks;
using Nop.plugin.Orders.ExtendedShipment.Services;
using System.IO;
using Nop.Services.Media;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Core.Domain.Customers;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    [AdminAntiForgery(true)]
    public class ExtendedOrderSettingController : BaseAdminController
    {
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IRepository<StateCodemodel> _repositoryStateCode;
        private readonly IRepository<CountryCodeModel> _repositoryCountryCode;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IScheduleTaskService _scheduleTaskService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        public ExtendedOrderSettingController(IStoreService storeService
            , IWorkContext workContext
            , IPermissionService permissionService
            , ISettingService settingService
            , ICategoryService categoryService
            , IProductService productService
            , IStoreMappingService storeMappingService
            , IAclService aclService
            , ILocalizationService localizationService
            , ICustomerService customerService
            , IRepository<StateCodemodel> repositoryStateCode
            , IRepository<CountryCodeModel> repositoryCountryCode
            , ICountryService countryService
            , IStateProvinceService stateProvinceService
            , IScheduleTaskService scheduleTaskService
            , IExtendedShipmentService extendedShipmentService
            )
        {
            this._customerService = customerService;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._workContext = workContext;
            this._settingService = settingService;
            this._categoryService = categoryService;
            this._productService = productService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._localizationService = localizationService;
            this._repositoryCountryCode = repositoryCountryCode;
            this._repositoryStateCode = repositoryStateCode;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._scheduleTaskService = scheduleTaskService;
            this._extendedShipmentService = extendedShipmentService;
        }
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var extendedShipmentSetting = _settingService.LoadSetting<ExtendedShipmentSetting>(storeScope);
            var model = new ExtendedShipmentSettingModel
            {
                PostAdminRoleId = extendedShipmentSetting.PostAdminRoleId,
                PostmanRoleId = extendedShipmentSetting.PostmanRoleId,
                PostPassword = extendedShipmentSetting.PostPassword,
                PostUserName = extendedShipmentSetting.PostUserName,
                PostType = extendedShipmentSetting.PostType,
                StoreAdminMessageTemplate = extendedShipmentSetting.StoreAdminMessageTemplate,
                PostAdminMessageTemplate = extendedShipmentSetting.PostAdminMessageTemplate,
                PostmanMessageTemplate = extendedShipmentSetting.PostmanMessageTemplate,
                UpdateFromPostInterval = extendedShipmentSetting.UpdateFromPostInterval,
                StoreAdminRoleId = extendedShipmentSetting.StoreAdminRoleId,
                CustomerMessageTemplate = extendedShipmentSetting.CustomerMessageTemplate,
                RoleList = GetListOFRole(0),
                AllCategory = getAllCategory()
            };
            return View("/Plugins/Orders.ExtendedShipment/Views/Configure.cshtml", model);
        }
        [HttpPost]
        public IActionResult Configure(ExtendedShipmentSettingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();
            if (!ModelState.IsValid)
                return Configure();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var ShipmentSettin = _settingService.LoadSetting<ExtendedShipmentSetting>(storeScope);
            ShipmentSettin.PostAdminRoleId = model.PostAdminRoleId;
            ShipmentSettin.PostmanRoleId = model.PostmanRoleId;
            ShipmentSettin.PostPassword = model.PostPassword;
            ShipmentSettin.PostUserName = model.PostUserName;
            ShipmentSettin.PostType = model.PostType;
            ShipmentSettin.StoreAdminMessageTemplate = model.StoreAdminMessageTemplate;
            ShipmentSettin.PostAdminMessageTemplate = model.PostAdminMessageTemplate;
            ShipmentSettin.PostmanMessageTemplate = model.PostmanMessageTemplate;
            ShipmentSettin.CustomerMessageTemplate = model.CustomerMessageTemplate;
            ShipmentSettin.UpdateFromPostInterval = model.UpdateFromPostInterval;
            ShipmentSettin.StoreAdminRoleId = model.StoreAdminRoleId;
            ShipmentSettin.TaskId = InsertTask(ShipmentSettin.TaskId, model.UpdateFromPostInterval);
            _settingService.SaveSetting(ShipmentSettin, storeScope);
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        private List<SelectListItem> GetListOFRole(int RoleId)
        {
            return _customerService.GetAllCustomerRoles(true).Where(p => p.Active == true).Select(p =>
             new SelectListItem()
             {
                 Text = p.Name,
                 Value = p.Id.ToString(),
                 Selected = RoleId != 0 ? (p.Id == RoleId ? true : false) : false
             }).ToList();
        }
        [HttpPost]
        public IActionResult SaveCode(CountryStateModel model)
        {
            if (model.countryId == 0 || model.stateId == 0)
            {
                return Json(new { errorMessage = "انتخاب استان و شهرستان الزامی می باشد" });
            }
            if (string.IsNullOrEmpty(model.countryCode) || string.IsNullOrEmpty(model.stateCode))
            {
                return Json(new { errorMessage = "وارد کردن کد استان و کد شهرستان الزامی می باشد" });
            }
            StateCodemodel SCM = null;
            if (_repositoryStateCode.Table.Any(p => p.stateId == model.stateId))
            {
                SCM = _repositoryStateCode.Table.First(p => p.stateId == model.stateId);
                SCM.StateCode = model.stateCode;
                SCM.SenderStateCode = model.SenderStateCode;
                _repositoryStateCode.Update(SCM);
            }
            else
            {
                SCM = new StateCodemodel()
                {
                    StateCode = model.stateCode,
                    stateId = model.stateId,
                    SenderStateCode = model.SenderStateCode
                };
                _repositoryStateCode.Insert(SCM);
            }
            CountryCodeModel CCM = null;
            if (_repositoryCountryCode.Table.Any(p => p.CountryId == model.countryId))
            {
                CCM = _repositoryCountryCode.Table.First(p => p.CountryId == model.countryId);
                CCM.CountryCode = model.countryCode;
                _repositoryCountryCode.Update(CCM);
            }
            else
            {
                CCM = new CountryCodeModel()
                {
                    CountryCode = model.countryCode,
                    CountryId = model.countryId
                };
                _repositoryCountryCode.Insert(CCM);
            }
            return Json(new { IsOk = true });

        }
        public IActionResult ReadCountryStateCode(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            List<CountryStateModel> data = new List<CountryStateModel>();
            var Countries = _countryService.GetAllCountries().ToList();
            foreach (var item in Countries)
            {
                var states = _stateProvinceService.GetStateProvincesByCountryId(item.Id, showHidden: true);
                foreach (var stateItem in states)
                {
                    data.Add(new CountryStateModel()
                    {
                        countryName = item.Name,
                        countryId = item.Id,
                        stateId = stateItem.Id,
                        stateName = stateItem.Name
                    });
                }
            }
            foreach (var item in data)
            {
                item.Id = Guid.NewGuid().ToString("N");
                var countryCode = _repositoryCountryCode.Table.FirstOrDefault(p => p.CountryId == item.countryId);
                if (countryCode != null)
                    item.countryCode = countryCode.CountryCode;
                var stateCode = _repositoryStateCode.Table.FirstOrDefault(p => p.stateId == item.stateId);
                if (stateCode != null)
                {
                    item.stateCode = stateCode.StateCode;
                    item.SenderStateCode = stateCode.SenderStateCode;
                }
            }
            var gridModel = new DataSourceResult
            {
                Data = data.Select(x => x),
                Total = data.Count
            };

            return Json(gridModel);
        }

        public IActionResult GetAllCountry()
        {
            var Countries = _countryService.GetAllCountries().Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
            return Json(Countries);
        }

        private int InsertTask(int? TaskId, int Minutes)
        {
            if (TaskId.HasValue)
            {
                var task = _scheduleTaskService.GetTaskById(TaskId.Value);

                if (task != null)
                {
                    if ((task.Seconds / 60) == Minutes)
                        return task.Id;
                    task.Seconds = Minutes * 60;
                    _scheduleTaskService.UpdateTask(task);
                    return task.Id;
                }
            }
            ScheduleTask tsk = new ScheduleTask()
            {
                Enabled = true,
                Name = "_PostTrakingTask",
                Seconds = Minutes * 60,
                StopOnError = false,
                Type = "Nop.plugin.Orders.ExtendedShipment.Services.RahgiriTask"
            };
            _scheduleTaskService.InsertTask(tsk);
            return tsk.Id;
        }
        [HttpPost]
        public IActionResult SaveBarcodeExcelFile()
        {
            var httpPostedFile = Request.Form.Files.FirstOrDefault();
            if (httpPostedFile == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No file uploaded",
                    downloadGuid = Guid.Empty
                });
            }

            var fileBinary = httpPostedFile.GetDownloadBits();

            var qqFileNameParameter = "qqfilename";
            var fileName = httpPostedFile.FileName;
            if (string.IsNullOrEmpty(fileName) && Request.Form.ContainsKey(qqFileNameParameter))
                fileName = Request.Form[qqFileNameParameter].ToString();
            //remove path (passed in IE)
            fileName = Path.GetFileName(fileName);

            var contentType = httpPostedFile.ContentType;

            var fileExtension = Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(fileExtension))
                fileExtension = fileExtension.ToLowerInvariant();

            string[] validFileExtension = new string[] { ".xsl", ".xlsx" };
            if (validFileExtension.All(p => p != fileExtension))
            {
                return Json(new
                {
                    success = false,
                    message = "فرمت فایل وارد شده نامعتبر می باشد",
                });
            }
            if (_extendedShipmentService.ReadExcelFile(new MemoryStream(fileBinary)))
            {
                return Json(new
                {
                    success = true,
                    message = "عملیات با موفقیت انجام شد",
                });
            }
            return Json(new
            {
                success = true,
                message = "بروز مشکل در زمان خواندن فایل",
            });
        }
        public IActionResult ReadBarcodeRepository(DataSourceRequest command, string barcode, int shipmentId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            List<BarcodeRepositoryModel> data = _extendedShipmentService.readBarcodeRepository(barcode, shipmentId);
            var gridModel = new DataSourceResult
            {
                Data = data.Select(x => x),
                Total = data.Count
            };

            return Json(gridModel);
        }
        public IActionResult SaveCateguryPostType(CateguryPostTypeModel model)
        {
            _extendedShipmentService.SaveCateguryPostType(model.CateguryName, model.CateguryId, model.PostType);
            return Json(new
            {
                success = true,
                message = "عملیات با موفقیت انجام شد",
            });
        }
        public IActionResult ReadCategoryPostType(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            List<CateguryPostTypeModel> data = _extendedShipmentService.ReadCategoryPostType();
            var gridModel = new DataSourceResult
            {
                Data = data.Select(x => x),
                Total = data.Count
            };

            return Json(gridModel);
        }
        public IActionResult DeleteCategoryPostType(int id)
        {
            if (_extendedShipmentService.DeleteCateguryPostType(id))
            {
                return Json(new
                {
                    success = true,
                    message = "عملیات با موفقیت انجام شد",
                });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = "خطا در زمان حذف فایل",
                });
            }
        }
        private List<SelectListItem> getAllCategory()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var categories = _categoryService.GetAllCategories(showHidden: true,storeId: storeScope);
            var items = categories.Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
            return items;
        }
    }
    public class CountryStateModel
    {
        public string Id { get; set; }
        public int countryId { get; set; }
        public int stateId { get; set; }
        public string countryName { get; set; }
        public string countryCode { get; set; }
        public string stateName { get; set; }
        public string stateCode { get; set; }
        public string SenderStateCode { get; set; }
    }
}

