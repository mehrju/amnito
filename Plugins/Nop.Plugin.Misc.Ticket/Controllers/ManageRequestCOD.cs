using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Misc.Ticket.Models;
using Nop.Plugin.Misc.Ticket.Models.Grid;
using Nop.Plugin.Misc.Ticket.Models.Search;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nop.plugin.Orders.ExtendedShipment.Services.CodService;

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageRequestCOD : BaseAdminController
    {
        #region Field
        private readonly IRepository<Tbl_RequestCODCustomer> _repositoryTbl_RequestCODCustomer;
        private readonly IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
        private readonly IRepository<Ticket.Domain.Tbl_Ticket> _repositoryTbl_Ticket;

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IShipmentService _shipmentService;
        private readonly INotificationService _notificationService;
        private readonly IRepository<Tbl_LogSMS> _repositoryTbl_LogSMS;
        private readonly IStoreContext _storeContext;
        private readonly ICodService _codService;
        #endregion

        #region Ctor
        public ManageRequestCOD
           (
               IRepository<Tbl_RequestCODCustomer> repositoryTbl_RequestCODCustomer,
               IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD,
               IStoreContext storeContext,
               ICustomerService customerService,
               IWorkContext workContext,
               IDbContext dbContext,
               IPermissionService permissionService,
               IStaticCacheManager cacheManager,
               ILocalizationService localizationService,
               ICustomerActivityService customerActivityService,
               IStoreService storeService,
               IHostingEnvironment hostingEnvironment,
               IShipmentService shipmentService,
               IRepository<Tbl_LogSMS> repositoryTbl_LogSMS,
               INotificationService notificationService,
               IRepository<Ticket.Domain.Tbl_Ticket> repositoryTbl_Ticket,
               ICodService codService
           )
        {
            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD = repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
            _repositoryTbl_RequestCODCustomer = repositoryTbl_RequestCODCustomer;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _storeService = storeService;
            _hostingEnvironment = hostingEnvironment;
            _shipmentService = shipmentService;
            _notificationService = notificationService;
            _repositoryTbl_LogSMS = repositoryTbl_LogSMS;
            _storeContext = storeContext;
            _repositoryTbl_Ticket = repositoryTbl_Ticket;
            _codService = codService;
        }
        #endregion



        #region Index & List
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();

            var Model = new Search_RequestCOD();
            try
            {

                #region Status
                Model.ListStatus.Add(new SelectListItem { Text = "انتخاب کنید", Value = "-1", Selected = true });
                Model.ListStatus.Add(new SelectListItem { Text = "در صف انتظار", Value = "0", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "در حال بررسی", Value = "1", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "عدم تایید", Value = "2", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "تایید", Value = "3", Selected = false });


                #endregion

            }
            catch (Exception ex)
            {

            }
            try
            {
                if (TempData["search_COD"] != null)
                {
                    var search_ = JsonConvert.DeserializeObject<Search_RequestCOD>(TempData["search_COD"].ToString());
                    if (search_.SearchId > 0)
                    {
                        Model.SearchId = search_.SearchId;

                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View("/Plugins/Misc.Ticket/Views/RequestCOD/ListRequestCOD.cshtml", Model);
        }

        [HttpPost]
        public virtual IActionResult RequestCODList(DataSourceRequest command, Search_RequestCOD model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            var gridModel = new DataSourceResult();
            string query = "EXEC dbo.Sp_GetRequestCodList @userName,@Name ,@Family,@Status,@requestId,@PageIndex,@PageSize,@TotalCount out";
            SqlParameter ToatlCount = new SqlParameter() { ParameterName = "TotalCount", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output, Value = 0 };
            SqlParameter[] prms = new SqlParameter[] {
                    new SqlParameter() { ParameterName = "userName", SqlDbType = SqlDbType.VarChar, Value = 
                    (string.IsNullOrEmpty(model.SearchUsername_customer) ?(object) DBNull.Value:(object)model.SearchUsername_customer) }
                    , new SqlParameter() { ParameterName = "Name", SqlDbType = SqlDbType.NVarChar, Value =
                    (string.IsNullOrEmpty(model.SearchFirstName) ?(object) DBNull.Value:(object)model.SearchFirstName) }
                    , new SqlParameter() { ParameterName = "Family", SqlDbType = SqlDbType.NVarChar, Value =
                    (string.IsNullOrEmpty(model.SearchLastName) ?(object) DBNull.Value:(object)model.SearchLastName) }

                     , new SqlParameter() { ParameterName = "Status", SqlDbType = SqlDbType.VarChar, Value =
                    (model.SearchIdStatusRequestCOD==-1 ?(object) DBNull.Value:(object)model.SearchIdStatusRequestCOD) }
                     , new SqlParameter() { ParameterName = "requestId", SqlDbType = SqlDbType.VarChar, Value =
                    (model.SearchRequestCODNumber==0 ?(object) DBNull.Value:(object)model.SearchRequestCODNumber) }

                    ,new SqlParameter() { ParameterName = "PageIndex", SqlDbType = SqlDbType.Int, Value = command.Page-1}
                    ,new SqlParameter() { ParameterName = "PageSize", SqlDbType = SqlDbType.Int, Value = command.PageSize }
                    ,ToatlCount

            };
            var data = _dbContext.SqlQuery<RequestCodSearchResult>(query, prms).ToList();
            var Final_RequestCOD = (dynamic)null;
            try
            {

                Final_RequestCOD= data.Select(q=>  new Grid_RequestCOD() {
                    Id = q.Id,
                    Grid_RequestCOD_RequestCODNumber = q.Id.ToString(),
                    Grid_RequestCOD_StoreName = "امنیتو",
                    Grid_RequestCOD_FullName = q.FullName,
                    Grid_RequestCOD_Status = q.Status == 0 ? "در صف انتظار" : q.Status == 1 ? "در حال بررسی" : q.Status == 2 ? "عدم تایید" : q.Status == 3 ? "تایید" : "-",
                    Grid_RequestCOD_DateInsert = q.DateInsert,
                    Grid_RequestCOD_NameStaff = q.StaffIdAccept != null ? _customerService.GetCustomerById((q.StaffIdAccept.GetValueOrDefault())).GetFullName() : "",
                    Grid_RequestCOD_DateOpen = q.DateStaffAccept,
                    Grid_RequestCOD_LastDateAnswer = q.DateStaffLastAnswer,
                    Grid_RequestCOD_NameStaffLastAnswer = q.StaffIdLastAnswer != null ? _customerService.GetCustomerById(q.StaffIdLastAnswer.GetValueOrDefault()).GetFullName() : "",
                    Grid_RequestCOD_RequestCODUserName = q.Username
                }).ToList(); ;
            }
            catch (Exception ex)
            {

                ErrrorMassege = ex.ToString();
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_RequestCOD,
                    Total = (int)ToatlCount.Value,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }

        public virtual IActionResult RelationController(int Id)
        {
            Search_RequestCOD search_COD = new Search_RequestCOD();
            search_COD.ActiveSearch = true;
            search_COD.SearchId = Id;
            TempData["search_COD"] = JsonConvert.SerializeObject(search_COD);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult GetListPatternAnswer()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var listPattern = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.IsActive && p.Type == 3).Select(p => new
            {
                Value = p.Id,
                Text = p.TitlePatternAnswer
            }).ToList();
            return Json(listPattern);

        }


        [HttpPost]
        public IActionResult GetDescriptionAnswer(int id)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            Tbl_PatternAnswer_Ticket_Damages_RequestCOD temp = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.GetById(id);
            if (temp != null)
            {
                return Json(new { success = true, state = 0, Description = temp.DescriptipnPatternAnswer });
            }
            else
            {
                return Json(new { success = false, state = 1, data = "" });
            }
        }


        #endregion

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var RequestCOD = _repositoryTbl_RequestCODCustomer.GetById(id);

            if (RequestCOD == null || RequestCOD.IsActive == false)
            {
                //No category found with the specified id
                return RedirectToAction("List");
            }
            if (RequestCOD.StaffIdAccept == null)
            {
                RequestCOD.StaffIdAccept = _workContext.CurrentCustomer.Id;
                RequestCOD.DateStaffAccept = DateTime.Now;
                RequestCOD.Status = 1;
                _repositoryTbl_RequestCODCustomer.Update(RequestCOD);
            }
            VmDetailRequestCOD model = new VmDetailRequestCOD();
            model.Tbl_RequestCODCustomer = _repositoryTbl_RequestCODCustomer.GetById(id);
            model.NameCustomer = _workContext.CurrentCustomer.GetFullName();
            model.Status = model.Tbl_RequestCODCustomer.Status == 0 ? "در صف انتظار" : model.Tbl_RequestCODCustomer.Status == 1 ? "در حال بررسی" : model.Tbl_RequestCODCustomer.Status == 2 ? "عدم تایید" : model.Tbl_RequestCODCustomer.Status == 3 ? "تایید درخواست" : "-";
            model.NameStaffAnswer = model.Tbl_RequestCODCustomer.StaffIdLastAnswer != null ? _customerService.GetCustomerById(model.Tbl_RequestCODCustomer.StaffIdLastAnswer.GetValueOrDefault()).GetFullName() : "";

            return View("/Plugins/Misc.Ticket/Views/RequestCOD/AnswerRequestCOD.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery(false)]
        public IActionResult ConfrimRequestCod(Tbl_RequestCODCustomer model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
           return saveRequestCod(model, true);
        }
        [HttpPost]
        [AdminAntiForgery(false)]
        public IActionResult DisAgreeRequestCod(Tbl_RequestCODCustomer model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
           return saveRequestCod(model, false);
        }
        private IActionResult saveRequestCod(Tbl_RequestCODCustomer model, bool isConfrim)
        {
            try
            {
                bool sendMessage = true;
                string tozihat = "";
                #region update data
                Ticket.Domain.Tbl_RequestCODCustomer RequestCOD = _repositoryTbl_RequestCODCustomer.GetById(model.Id);
                RequestCOD.AccountBranchName = model.AccountBranchName;
                RequestCOD.AccountNumber = model.AccountNumber;
                RequestCOD.Email = model.Email;
                RequestCOD.Fname = model.Fname;
                RequestCOD.Lname = model.Lname;
                RequestCOD.Phone = model.Phone;
                RequestCOD.CodePosti = model.CodePosti;
                RequestCOD.NatinolCode = model.NatinolCode;
                RequestCOD.ManagerNationalIDSerial = model.ManagerNationalIDSerial;
                RequestCOD.ManagerBirthDate = model.ManagerBirthDate;
                RequestCOD.Mobile = model.Mobile;
                RequestCOD.Shaba = model.Shaba;
                RequestCOD.StateId = model.StateId;
                RequestCOD.CityId = model.CityId;
                RequestCOD.Address = model.Address;
                RequestCOD.DesAnswerStaff = model.DesAnswerStaff;
                RequestCOD.StaffIdLastAnswer = _workContext.CurrentCustomer.Id; 
                #endregion
                if (isConfrim == true)
                {
                    var cityDateResult = getStateDate(RequestCOD.CityId);
                    if (cityDateResult == null)
                        return Json(new { error = true, message = "اطلاعاتی برای شهر مبدا یافت نشد" });
                    var shop = new Shop()
                    {
                        AccountBranchName = RequestCOD.AccountBranchName,
                        AccountIBAN = RequestCOD.Shaba,
                        AccountNumber = RequestCOD.AccountNumber,
                        Email = RequestCOD.Email,
                        ManagerBirthDate = RequestCOD.ManagerBirthDate,
                        ManagerNationalID = RequestCOD.NatinolCode,
                        ManagerNationalIDSerial = RequestCOD.ManagerNationalIDSerial,
                        Mobile = RequestCOD.Mobile,
                        Phone = RequestCOD.Phone,
                        PostalCode = RequestCOD.CodePosti,
                        Shopname = $"بازار متریال-{cityDateResult.StateName ?? ""}-{RequestCOD.Fname} {RequestCOD.Lname}",
                        WebSiteURL = String.IsNullOrEmpty(RequestCOD.WebSiteURL) ? "postbar.ir" : RequestCOD.WebSiteURL,
                        StartDate = Common.DateTimeToPersian(DateTime.Now, '/'),
                        EndDate = model.EndDate.Substring(0,10),
                        CityID = cityDateResult.StateCode,
                        ShopUsername = RequestCOD.Mobile,
                        ManagerName = RequestCOD.Fname,
                        ManagerFamily = RequestCOD.Lname,
                    };
                    var result = _codService.RegisterGatewayShop(shop);
                    if (result != null)
                    {
                        if (result.ShopId != 0)
                        {
                            RequestCOD.Status = 3;
                            tozihat += " " + " تایید درخواست انجام شد ";
                        }
                        else
                        {
                            if (result.UnhandledMessage)
                            {
                                tozihat += " " + " خطا در زمان ثبت فروشگاه در گیت وی ";
                                common.Log("خطا در زمان ثبت فروشگاه درگیت وی", result.Message);
                                sendMessage = false;
                            }
                            else
                            {
                                tozihat += " " + result.Message;
                                common.Log("خطا در زمان ثبت فروشگاه درگیت وی", result.Message);
                                sendMessage = false;
                            }
                        }
                    }
                    else
                    {
                        tozihat += "خطا در زمان ثبت فروشگاه در گیت وی";
                        common.Log("خطا در زمان ثبت فروشگاه درگیت وی", "اطلاعاتی از سرویس دریافت نشده");
                    }
                }
                else
                {
                    RequestCOD.Status = 2;
                    tozihat += " " + " عدم تایید درخواست با توضیحات زیل میباشد";
                    sendMessage = false;
                }
                RequestCOD.DesAnswerStaff = tozihat + @"\n" + model.DesAnswerStaff;
                _repositoryTbl_RequestCODCustomer.Update(RequestCOD);

                #region send sms to customer
                if (sendMessage)
                {
                    string msg = "کاربر گرامی ، پاسخ درخواست فعال سازی حساب پی کرایه به شماره: " + RequestCOD.Id.ToString() + " داده شده است، بررسی بفرمایید، سامانه امنیتو";
                    var sended = _notificationService._sendSms(_customerService.GetCustomerById(RequestCOD.IdCustomer).Username, msg);
                    #region log sms
                    Tbl_LogSMS log1 = new Tbl_LogSMS();
                    log1.Type = 5;
                    log1.Mobile = _workContext.CurrentCustomer.Username;
                    log1.StoreId = _storeContext.CurrentStore.Id;
                    log1.TextMessage = msg;
                    log1.Status = sended == true ? 1 : 0;
                    log1.DateSend = DateTime.Now;
                    _repositoryTbl_LogSMS.Insert(log1);

                    #endregion
                }
                #endregion


                #region update Ticket
                Tbl_Ticket tbl_Ticket = _repositoryTbl_Ticket.GetById(RequestCOD.IdTicket);
                if (tbl_Ticket != null)
                {

                    tbl_Ticket.Status = 3;
                    tbl_Ticket.DateUpdate = DateTime.Now;
                    tbl_Ticket.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Ticket.Update(tbl_Ticket);
                }
                #endregion

                return Json(new { success = true, message = tozihat });
            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, message = "خطا در زمان تایید و ثبت درخواست" });
            }
        }
        public gatewayStateDate getStateDate(int StateId)
        {
            string query = $@"SELECT
	                SP.Name StateName,
	                cast(sc.StateCode as int) StateCode
                FROM 
	                dbo.StateProvince AS SP
	                INNER JOIN dbo.StateCode AS SC ON SC.stateId = SP.Id
                WHERE
	                Sp.id = {StateId}";
            return _dbContext.SqlQuery<gatewayStateDate>(query, new object[0]).FirstOrDefault();
        }
        public class gatewayStateDate
        {
            public string StateName { get; set; }
            public int StateCode { get; set; }
        }
    }
}
