using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Extensions;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.plugin.Orders.ExtendedShipment.Services.Tozico;
using Nop.plugin.Orders.ExtendedShipment.Tools;
using Nop.Services.Customers;
using Nop.Services.ExportImport.Help;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Kendoui;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class ExtendedCustomerController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IRewardPointCashoutService _rewardPointCashoutService;
        private readonly ICustomerVehicleService _customerVehicleService;
        private readonly ITozicoService _tozicoService;
        private readonly IStoreService _storeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IAgentConfigService _agentConfigService;
        private readonly IExtendedShipmentService _extendedShipmentService;

        public ExtendedCustomerController(IPermissionService permissionService
        , ICustomerService customerService
        , IRewardPointService rewardPointService
        , IStoreService storeService
        , IDateTimeHelper dateTimeHelper
        , ILocalizationService localizationService
        , IRewardPointCashoutService rewardPointCashoutService,
            ICustomerVehicleService customerVehicleService,
            IWorkContext workContext,
            ITozicoService tozicoService,
            IAgentConfigService agentConfigService,
            IExtendedShipmentService extendedShipmentService)
        {
            _extendedShipmentService = extendedShipmentService;
            _dateTimeHelper = dateTimeHelper;
            _permissionService = permissionService;
            _customerService = customerService;
            _rewardPointService = rewardPointService;
            _storeService = storeService;
            _localizationService = localizationService;
            _rewardPointCashoutService = rewardPointCashoutService;
            _customerVehicleService = customerVehicleService;
            _tozicoService = tozicoService;
            this._workContext = workContext;
            _agentConfigService = agentConfigService;
        }
        [HttpPost]
        public IActionResult RewardPointsHistorySelect(DataSourceRequest command, int customerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            var customer = _customerService.GetCustomerById(customerId);
            if (customer == null)
                throw new ArgumentException("No customer found with the specified id");

            var rewardPoints = _rewardPointService.GetRewardPointsHistory(customer.Id, true, true, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = rewardPoints.Select(rph =>
                {
                    var store = _storeService.GetStoreById(rph.StoreId);
                    var activatingDate = ConvertToJalali(_dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc));

                    return new //CustomerModel.RewardPointsHistoryModel
                    {
                        StoreName = store != null ? store.Name : "Unknown",
                        Points = rph.Points,
                        PointsBalance = rph.PointsBalance.HasValue ? (rph.PointsBalance.GetValueOrDefault(0).ToString("N0"))
                            : string.Format(_localizationService.GetResource("Admin.Customers.Customers.RewardPoints.ActivatedLater"), activatingDate),
                        Message = rph.Message,
                        CreatedOn = activatingDate
                    };
                }),
                Total = rewardPoints.TotalCount
            };

            return Json(gridModel);
        }

        public IActionResult RewardPointsHistoryCashout()
        {
            return View("~/Plugins/Orders.ExtendedShipment/Views/RewardPointsHistoryCashout.cshtml");
        }

        public IActionResult DashboardMessaging()
        {
            return View("~/Plugins/Orders.ExtendedShipment/Views/DashboardMessaging.cshtml");
        }

        public IActionResult RequestFactor(int? RequestFactorId)
        {
            return View("~/Plugins/Orders.ExtendedShipment/Views/RequestFactor.cshtml", new FactorRequestModel() { requestFactorId = RequestFactorId });
        }

        [HttpPost]
        public IActionResult GridRewardPointsHistory(DataSourceRequest command, RewardPointsCashoutInputModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            //var rewardPoints = _rewardPointService.GetRewardPointsHistory(0, true, true, command.Page - 1, command.PageSize);

            int count = 0;
            var rewardPoints = _rewardPointCashoutService.getRewardPointCashout(model.dateFrom, model.dateTo, model.customers, model.chargeWalletType, "", command.PageSize, command.Page - 1, out count);

            var gridModel = new DataSourceResult
            {
                Data = rewardPoints.ToList(),
                Total = count
            };
            return Json(gridModel);
        }
        [HttpPost]
        public IActionResult SubmitRequestFactor(int requestFactorId, string description)
        {
            var mes = _rewardPointCashoutService.execSp<string>("MyDashboard_Sp_Operations", new { Operation = "SubmitBackEndFactor", requestFactorId, description, CustomerId = _workContext.CurrentCustomer.Id });

            return Json(new { message = mes.FirstOrDefault() });
        }
        [HttpPost]
        public IActionResult SendAdminMessage(AdminSendMessageModel model)
        {
            var mes = _rewardPointCashoutService.execSp<string>("MyDashboard_Sp_Operations", new { Operation = "AdminSendMessage", model.customers, model.roles, model.message, model.subject, CustomerId = _workContext.CurrentCustomer.Id });

            return Json(new { message = mes.FirstOrDefault() });
        }
        [HttpPost]
        public IActionResult RequestFactorDetails(int requestFactorId)
        {
            var item = _rewardPointCashoutService.execSp<FactorRequestResult>("MyDashboard_Sp_Operations", new { Operation = "GetBackEndFactorList", requestFactorId, Page = 1, PageSize = 1, CustomerId = _workContext.CurrentCustomer.Id }).FirstOrDefault();
            return Json(item);
        }

        [HttpPost]
        public IActionResult GridAdminMessageHistory(DataSourceRequest command, AdminSearchMessageModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            string outjson;
            var DashboardFactors = _rewardPointCashoutService.execSp<AdminMessageModelResult>("MyDashboard_Sp_Operations", new { Operation = "GetBackEndMessageList", model.dateFrom, model.dateTo, model.customers2, model.roles2, model, command.Page, command.PageSize, CustomerId = _workContext.CurrentCustomer.Id }, out outjson);

            var tt = new { TotalCount = (int)0 };
            tt = JsonConvert.DeserializeAnonymousType(outjson, tt);

            var gridModel = new DataSourceResult
            {
                Data = DashboardFactors.ToArray(),
                Total = tt.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult GridRequestFactor(DataSourceRequest command, FactorRequestModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            string outjson;
            var DashboardFactors = _rewardPointCashoutService.execSp<FactorRequestResult>("MyDashboard_Sp_Operations", new { Operation = "GetBackEndFactorList", model.dateFrom, model.dateTo
                , model.customers, model.requestFactorId, command.Page, command.PageSize }, out outjson);

            var tt = new { TotalCount = (int)0 };
            tt = JsonConvert.DeserializeAnonymousType(outjson, tt);

            var gridModel = new DataSourceResult
            {
                Data = DashboardFactors.ToArray(),
                Total = tt.TotalCount
            };
            return Json(gridModel);
        }
        [HttpGet]
        public IActionResult GetchargeWalletTypes()
        {
            return Json(_rewardPointCashoutService.getchargeWalletTypes());
        }

        [HttpGet]
        public IActionResult Getcustomers(string searchtext)
        {
            var items = _rewardPointCashoutService.getcustomers(searchtext).Select(p => new { id = p.Value, text = p.Text }).ToList();
            return Json(new { results = items });
        }
        [HttpGet]
        public IActionResult Getroles(string searchtext)
        {
            var items = _rewardPointCashoutService.execSp<_SelectListItem>("MyDashboard_Sp_Operations", new { Operation = "GetRolesForMessagingBackEnd", CustomerId = _workContext.CurrentCustomer.Id })
                .Select(p => new { id = p.Id.ToString(), text = p.Name }).ToList();
            return Json(new { results = items });
        }

        [HttpPost]
        public IActionResult ImportExcel(string name)
        {
            string ServerMessage = "عملیات با خطا مواجه شد";
            //return Json(new { fileCount = Request.Form.Files.Count.ToString() });
            var files = Request.Form.Files;
            try
            {

                if (files != null && files.Count() > 0)
                {
                    foreach (var item in files)
                    {
                        if (item != null)
                        {
                            if (files.First().Length > 3145728)
                            {

                                throw new NopException("حجم فایل انتخاب شده بیش از 3 مگا بایت میباشد");
                            }
                            else
                            {

                                var fullpath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ImportExcelCashout_Files");

                                if (!Directory.Exists(fullpath))
                                    Directory.CreateDirectory(fullpath);

                                var filename = System.IO.Path.GetFileNameWithoutExtension(item.FileName) + "-" + DateTime.Now.Ext_ToFilename() + System.IO.Path.GetExtension(item.FileName);

                                var filePath = Path.Combine(fullpath, filename);
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    item.CopyTo(fileStream);
                                }
                                Thread.Sleep(1000);
                                var info = ReadFromXlsx(new System.IO.FileInfo(filePath));

                                var jsondataTable = UtilityExtensions.Ext_ArrayToDataTable(info).Ext_ToJson();
                                ServerMessage = _rewardPointCashoutService.saveRewardPointCashout(jsondataTable);
                            }
                        }
                    }

                }
                ViewData["ServerMessage"] = ServerMessage;
            }
            catch (Exception ex)
            {
                ViewData["ServerMessage"] = ex.Message;
            }
            return RewardPointsHistoryCashout();
        }
        // Export DataTable into an excel file with field names in the header line
        // - Save excel file without ever making it visible if filepath is given
        // - Don't save excel file, just make it visible if no filepath is given
        public static List<ExcelImportModel> ReadFromXlsx(FileInfo fileInfo)
        {
            using (var xlPackage = new ExcelPackage(fileInfo))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new NopException("No worksheet found");

                var properties = GetPropertiesByExcelCells<ExcelImportModel>(worksheet);

                var manager = new PropertyManager<ExcelImportModel>(properties);

                List<ExcelImportModel> excelImportModels = new List<ExcelImportModel>();

                int rowId = 8;
                while (true)
                {
                    var m1 = new ExcelImportModel();
                    manager.ReadFromXlsx(worksheet, rowId);

                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[rowId, property.PropertyOrderPosition])
                        .All(cell => string.IsNullOrEmpty(cell?.Value?.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    foreach (var m in manager.GetProperties)
                    {
                        switch (m.PropertyName?.Trim())
                        {
                            case "مبلغ انتقال وجه":
                                m1.Transfer_Amount = m.StringValue;
                                break;
                            case "شبای مقصد":
                                m1.Sheba = m.StringValue;
                                break;
                            case "آماده پردازش":
                                m1.Description = m.StringValue;
                                break;
                            case "شناسه واریز":
                                m1.Factor_Number = m.StringValue;
                                break;
                        }
                    }
                    excelImportModels.Add(m1);
                    rowId++;
                }
                return excelImportModels;
            }
        }
        /// <summary>
        /// Get property list by excel cells
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="worksheet">Excel worksheet</param>
        /// <returns>Property list</returns>
        public static IList<PropertyByName<T>> GetPropertiesByExcelCells<T>(ExcelWorksheet worksheet)
        {
            var properties = new List<PropertyByName<T>>();
            var poz = 1;
            var row = 7;
            while (true)
            {
                try
                {
                    var cell = worksheet.Cells[row, poz];

                    if (string.IsNullOrEmpty(cell?.Value?.ToString()))
                        break;
                    poz += 1;
                    properties.Add(new PropertyByName<T>(cell.Value.ToString()));
                }
                catch
                {
                    break;
                }
            }

            return properties;
        }

        public IActionResult ExcelRewardPointsHistory(RewardPointsCashoutInputModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            int count = 0;

            var rewardPoints = _rewardPointCashoutService.getRewardPointCashout(model.dateFrom, model.dateTo, model.customers, model.chargeWalletType, "Stream:download", 0, 0, out count);

            var bts = ExportRewardPointsToXlsx(rewardPoints.ToArray());

            Thread.Sleep(1000);
            var stream = new MemoryStream(bts);

            return File(stream, MimeTypes.TextXlsx, $"RewardPointsHistory_{Guid.NewGuid().ToString()}.xlsx");
        }

        public IActionResult ExcelRewardPointsHistoryPreview(RewardPointsCashoutInputModel model)
        {
            if (!string.IsNullOrEmpty(model.date1))
                model.dateFrom = model.date1.FromPersianToGregorianDate();
            if (!string.IsNullOrEmpty(model.date2))
                model.dateTo = model.date2.FromPersianToGregorianDate();

            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedKendoGridJson();

            int count = 0;

            var rewardPoints = _rewardPointCashoutService.getRewardPointCashout(model.dateFrom, model.dateTo, model.customers, model.chargeWalletType, "Stream:preview", 0, 0, out count);

            var jsondataTable = UtilityExtensions.Ext_ArrayToDataTable(rewardPoints).Ext_ToJson();
            return Json(new { jsondataTable });
        }

        public byte[] ExportRewardPointsToXlsx(RewardPointsCashoutModel[] itemsToExport)
        {
            var properties = new[]
            {
                new PropertyByName<RewardPointsCashoutModel>("Destination Iban Number (Variz Be Sheba)", p => p.Sheba),
                new PropertyByName<RewardPointsCashoutModel>("Owner Name (Name e Sahebe Seporde)", p => p.CustomerName),
                new PropertyByName<RewardPointsCashoutModel>("Transfer Amount (Mablagh)", p => p.RewardPointBalance),
                new PropertyByName<RewardPointsCashoutModel>("Description (Sharh)", p => p.Message),
                new PropertyByName<RewardPointsCashoutModel>("Factor Number (Shomare Factor)", p => p.FactorNumber)
            };
            return UtilityExtensions.ExportToXlsx(properties, itemsToExport);
        }


        private string ConvertToJalali(DateTime dt)
        {
            PersianCalendar pr = new PersianCalendar();
            return pr.GetYear(dt) + "/" + pr.GetMonth(dt).ToString("00") + "/" +
                pr.GetDayOfMonth(dt).ToString("00") + " " + pr.GetHour(dt).ToString("00") + ":" + pr.GetMinute(dt).ToString("00")
                + ":" + pr.GetSecond(dt).ToString("00");
        }
        public IActionResult CustomerFnancialPlansIndex(CustomerModel model)
        {
            return View("~/Plugins/Orders.ExtendedShipment/Views/_CustomerFnancialPlans.cshtml");
        }
       
        public IActionResult getCustomerByRoleId(int RoleId)
        {
            var reuslt =_extendedShipmentService.getCustomerByRoleId(RoleId);
            return Json(reuslt);
        }
        public IActionResult CustomerVehiclesIndex()
        {
            return View("~//Plugins/Orders.ExtendedShipment/Views/_CustomerVehicles.cshtml");
        }


        public IActionResult GetVehicleTypes()
        {
            var enumList = Utility.GetEnumList<VehicleTypeEnum>();
            return Ok(enumList.Select(p => new { Text = p.DisplayName, Value = p.Value }));
        }

        public IActionResult GetCustomerVehicles(int customerId)
        {
            var vehicles = _customerVehicleService.GetCustomerVehicles(customerId);
            var gridModel = new DataSourceResult
            {
                Data = vehicles.Select(p => new { p.Id, p.Name, p.Phone, VehicleTypeName = p.VehicleTypeEnum.GetDisplayName() }).ToList(),
                Total = vehicles.Count
            };
            return Ok(gridModel);
        }


        [HttpPost]
        public IActionResult SaveCustomervehicle(Tbl_CustomerVehicle model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            _customerVehicleService.SaveCustomerVehicle(model);
            var tozicoResponse = _tozicoService.AddOrUpdateVehicles(new List<Vehicle>()
            {
                new Vehicle()
                {
                    Branch = model.CustomerId,
                    CapacityCount = model.CapacityCount,
                    CapacityPercent = model.CapacityPercent,
                    CapacityVolume = model.CapacityVolume,
                    CapacityWeight = model.CapacityWeight,
                    Description = model.Description,
                    Id = model.Id,
                    IsActive = model.IsActive,
                    Name = model.Name,
                    Phone = model.Phone,
                    PhoneAlt = "",
                    VehicleTypeEnum = model.VehicleTypeEnum
                }
            });
            if (tozicoResponse != null && tozicoResponse.Success && tozicoResponse.Results != null && tozicoResponse.Results.Any())
            {
                if (tozicoResponse.Results.All(p => p.Errors == null || !p.Errors.Any()))
                {
                    return Ok();
                }
            }
            return Ok(new { message = "اطلاعات در امنیتو ثبت شد، اما در ثبت اطلاعات در توزیکو خطایی رخ داده است" });
        }


        public IActionResult GetCustomervehicle(int id)
        {
            var model = _customerVehicleService.GetCustomervehicleById(id);
            if (model == null)
            {
                return BadRequest();
            }
            return Ok(model);
        }


        public IActionResult RemoveCustomervehicleById(int id)
        {
            var model = _customerVehicleService.GetCustomervehicleById(id);
            if (model == null)
            {
                return BadRequest();
            }
            _tozicoService.AddOrUpdateVehicles(new List<Vehicle>()
            {
                new Vehicle()
                {
                    Branch = model.CustomerId,
                    CapacityCount = model.CapacityCount,
                    CapacityPercent = model.CapacityPercent,
                    CapacityVolume = model.CapacityVolume,
                    CapacityWeight = model.CapacityWeight,
                    Description = model.Description,
                    Id = model.Id,
                    IsActive = model.IsActive,
                    Name = model.Name,
                    Phone = model.Phone,
                    PhoneAlt = "",
                    VehicleTypeEnum = model.VehicleTypeEnum,
                    IsDeleted = true
                }
            });
            _customerVehicleService.RemoveCustomerVehicle(model);
            return Ok();
        }
        [HttpPost]
        public IActionResult GetCustomerToken(int CustomerId)
        {
            return Json(_tozicoService.getLoginToken(CustomerId));
        }

        public IActionResult AgentConfig()
        {
            return View("~/Plugins/Orders.ExtendedShipment/Views/AgentConfig.cshtml");
        }

        [HttpPost]
        public IActionResult GridAgentConfig(DataSourceRequest command, AgentConfigInputModel model)
        {

            int count;
            var AgentConfigs = _agentConfigService.GridAgentConfig(model, command.PageSize, command.Page - 1, out count);

            var gridModel = new DataSourceResult
            {
                Data = AgentConfigs.ToArray(),
                Total = count
            };
            return Json(gridModel);
        }

        public IActionResult SaveAgentConfig(AgentConfigResultModel model)
        {
            _agentConfigService.SaveAgentConfig(model);
            return Json(new
            {
                success = true,
                message = "عملیات با موفقیت انجام شد",
            });
        }

        public IActionResult UpdateAgentConfig(AgentConfigResultModel model)
        {
            _agentConfigService.UpdateAgentConfig(model);
            return Json(new
            {
                success = true,
                message = "عملیات با موفقیت انجام شد",
            });
        }

        [HttpPost]
        public IActionResult GetAgents()
        {
            var items = _agentConfigService.GetAgents().Select(p => new { id = p.Value, text = p.Text }).ToList();
            return Json(items);
        }

        [HttpPost]
        public IActionResult GetCollectors()
        {
            var items = _agentConfigService.GetCollectors().Select(p => new { id = p.Value, text = p.Text }).ToList();
            return Json(items);
        }

        [HttpPost]
        public IActionResult GetServices()
        {
            var items = _agentConfigService.GetServices().Select(p => new { id = p.Value, text = p.Text }).ToList();
            return Json(items);
        }

        [HttpPost]
        public IActionResult GridServiceDiscount(int CustomerId)
        {
            var items = _agentConfigService.GridServiceDiscount(CustomerId);
            var gridModel = new DataSourceResult
            {
                Data = items.ToArray()
            };
            return Json(gridModel);
        }

        [HttpPost]
        public IActionResult SaveServiceDiscount(ServiceDiscountModel model)
        {
            model.ActiveCustomerId = _workContext.CurrentCustomer.Id;
            model.ActiveDate = DateTime.Now;
            _agentConfigService.SaveServiceDiscount(model);
            return Json(new
            {
                success = true,
                message = "عملیات با موفقیت انجام شد",
            });
        }
    }
}
