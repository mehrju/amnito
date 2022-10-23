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

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageDamagesController : BaseAdminController
    {
        private readonly IRepository<Tbl_Damages> _repositoryTbl_Damages;
        private readonly IRepository<Tbl_Damages_Detail> _repositoryTbl_Damages_Detail;
        private readonly IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
        private readonly IRepository<Ticket.Domain.Tbl_Ticket> _repositoryTbl_Ticket;

        private readonly IOrderService _orderService;
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
        public ManageDamagesController
            (
             IOrderService orderService,
        IRepository<Tbl_Damages> repositoryTbl_Damages,
        IRepository<Tbl_Damages_Detail> repositoryTbl_Damages_Detail,
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
            IRepository<Ticket.Domain.Tbl_Ticket> repositoryTbl_Ticket
            )
        {
            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD = repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
            _repositoryTbl_Damages = repositoryTbl_Damages;
            _repositoryTbl_Damages_Detail = repositoryTbl_Damages_Detail;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _storeService = storeService;
            _hostingEnvironment = hostingEnvironment;
            _orderService = orderService;
            _shipmentService = shipmentService;
            _notificationService = notificationService;
            _repositoryTbl_LogSMS = repositoryTbl_LogSMS;
            _storeContext = storeContext;
            _repositoryTbl_Ticket = repositoryTbl_Ticket;
        }

        #region Index & List
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();

            var Model = new Search_Damages();
            try
            {
                #region Stor
                var Stores = _storeService.GetAllStores(true).ToList();
                if (Stores.Count > 0)
                {
                    Model.ListStores.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                    foreach (var s in Stores)
                    {

                        Model.ListStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                    }
                }
                else
                {
                    Model.ListStores.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
                #endregion
                #region  role
                var defaultRoleIds = new List<int> { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Registered).Id };

                var allRoles = _customerService.GetAllCustomerRoles(true);
                foreach (var role in allRoles)
                {
                    Model.AvailableCustomerRoles.Add(new SelectListItem
                    {
                        Text = role.Name,
                        Value = role.Id.ToString(),
                        Selected = defaultRoleIds.Any(x => x == role.Id)
                    });
                }
                #endregion

                #region Status
                Model.ListStatus.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                Model.ListStatus.Add(new SelectListItem { Text = "در صف انتظار", Value = "0", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "در حال بررسی", Value = "1", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "پاسخ داده شده", Value = "2", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "عدم رعایت قانون 24", Value = "3", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "عدم تایید، بسته شده", Value = "4", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "تایید،پرداخت غرامت، بسته شده", Value = "5", Selected = false });


                #endregion

            }
            catch (Exception ex)
            {

            }
            return View("/Plugins/Misc.Ticket/Views/Damages/ListDamages.cshtml", Model);
        }
        private string GetOrderId_By_TrackingCode(string trackingcode)
        {
            var shipment = _shipmentService.GetAllShipments(trackingNumber: trackingcode).FirstOrDefault();
            var order = _orderService.GetOrderById(shipment.OrderId);
            return order.Id.ToString();
        }
        [HttpPost]
        public virtual IActionResult DamagesList(DataSourceRequest command, Search_Damages model, int[] searchCustomerRoleIds)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            var gridModel = new DataSourceResult();
            var Final_Damages = (dynamic)null;
            try
            {
                List<Tbl_Damages> ListAllDamages = _repositoryTbl_Damages.Table.Where(p => p.StaffIdAccept == _workContext.CurrentCustomer.Id | p.StaffIdAccept == null).ToList();

                if (ListAllDamages.Count > 0)
                {
                    if (model.ActiveSearch == true)
                    {
                        if (searchCustomerRoleIds.Count() > 0 || !string.IsNullOrEmpty(model.SearchEmail) || !string.IsNullOrEmpty(model.SearchFirstName) || !string.IsNullOrEmpty(model.SearchLastName))
                        {
                            var search_Customer = _customerService.GetAllCustomers(
                             customerRoleIds: searchCustomerRoleIds,
                             email: model.SearchEmail,
                             firstName: model.SearchFirstName,
                             lastName: model.SearchLastName);

                            var temp = (from l in ListAllDamages
                                        join c in search_Customer on l.IdCustomer equals c.Id
                                        select l).ToList();

                            ListAllDamages = temp;

                        }


                        ListAllDamages = ListAllDamages.Where(p => p.IsActive != model.Search_Damages_IsDeleted).ToList();
                        #region Search

                        if (model.SearchUsername_customer > 0)
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.IdCustomer == model.SearchUsername_customer).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.SearchNameGoods))
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.NameGoods.Contains(model.SearchNameGoods)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.SearchNameBerand))
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.Berand.Contains(model.SearchNameBerand)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.SearchTrackingCodeDamages))
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.TrackingCode.Contains(model.SearchTrackingCodeDamages)).ToList();
                        }

                        if (model.SearchDamagesNumber > 0)
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.Id == (model.SearchDamagesNumber)).ToList();

                        }

                        if (model.SearchIdStatusDamages > 0)
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.Status == (model.SearchIdStatusDamages)).ToList();

                        }
                        if (model.SearchStoreId > 0)
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.StoreId == (model.SearchStoreId)).ToList();

                        }
                        if (model.SearchId > 0)
                        {
                            ListAllDamages = ListAllDamages.Where(p => p.Id == model.SearchId).ToList();
                        }
                        #endregion
                    }
                    else
                    {
                        ListAllDamages = ListAllDamages.Where(p => p.IsActive && (p.Status == 0 || p.Status == 1)).ToList();
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Damages = (from q in ListAllDamages
                                 select new Grid_Damages
                                 {
                                     Id = q.Id,
                                     Grid_Damages_DamagesNumber = q.Id.ToString(),
                                     Grid_Damages_StoreName = _storeService.GetStoreById(q.StoreId).Name,
                                     Grid_Damages_NameGoods = q.NameGoods,
                                     Grid_Damages_FullName = _customerService.GetCustomerById(q.IdCustomer).GetFullName(),
                                     Grid_Damages_TrackingCode = q.TrackingCode,
                                     Grid_Damages_Status = q.Status == 0 ? "در صف انتظار" : q.Status == 1 ? "در حال بررسی" : q.Status == 2 ? "پاسخ داده شده" : q.Status == 3 ? "عدم رعایت قانون 24" : q.Status == 4 ? "عدم تایید و پایان" : q.Status == 5 ? " تایید،پرداخت غرامت و پایان" : "-",
                                     Grid_Damages_DateInsert = q.DateInsert,
                                     Grid_Damages_NameStaff = q.StaffIdAccept != null ? _customerService.GetCustomerById((q.StaffIdAccept.GetValueOrDefault())).GetFullName() : "",
                                     Grid_Damages_DateOpen = q.DateStaffAccept,
                                     Grid_Damages_LastDateAnswer = q.DateStaffLastAnswer,
                                     Grid_Damages_NameStaffLastAnswer = q.StaffIdLastAnswer != null ? _customerService.GetCustomerById(q.StaffIdLastAnswer.GetValueOrDefault()).GetFullName() : "",
                                     Grid_Damages_IsActive = q.IsActive,
                                     UrlPage = GetOrderId_By_TrackingCode(q.TrackingCode),


                                 }).ToList();
            }
            catch (Exception ex)
            {

                ErrrorMassege = ex.ToString();
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_Damages,
                    Total = Final_Damages.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }

        public virtual IActionResult RelationController(int Id)
        {
            Search_Damages search_Damages = new Search_Damages();
            search_Damages.ActiveSearch = true;
            search_Damages.SearchId = Id;
            TempData["search_COD"] = JsonConvert.SerializeObject(search_Damages);
            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult GetListPatternAnswer()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var listPattern = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.IsActive && p.Type == 2).Select(p => new
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


        #region active &...
        [HttpPost]
        public virtual IActionResult DisableDamages(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Damages temp = _repositoryTbl_Damages.GetById(id);

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Damages.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableDamages", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult AcceptedFinishedDamagesPayment(int id)//,string PaymentPrice,DateTime PaymentDate,string Des
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Damages temp = _repositoryTbl_Damages.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.Status = 5;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    temp.StaffIdAccept = _workContext.CurrentCustomer.Id;
                    temp.DateStaffAccept = DateTime.Now;
                    _repositoryTbl_Damages.Update(temp);
                    #region update Ticket
                    Tbl_Ticket tbl_Ticket = _repositoryTbl_Ticket.GetById(temp.IdTicket);
                    if (tbl_Ticket != null)
                    {

                        tbl_Ticket.Status = 3;
                        tbl_Ticket.DateUpdate = DateTime.Now;
                        tbl_Ticket.IdUserUpdate = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_Ticket.Update(tbl_Ticket);
                    }
                    #endregion
                }

                //activity log
                _customerActivityService.InsertActivity("AcceptedFinishedDamagesPayment", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), temp.Id.ToString());
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));

            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }
        [HttpPost]
        public virtual IActionResult disapprovalFinishedDamages(int id)//,DateTime PaymentDate,string Des
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Damages temp = _repositoryTbl_Damages.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.Status = 4;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    temp.StaffIdAccept = _workContext.CurrentCustomer.Id;
                    temp.DateStaffAccept = DateTime.Now;
                    _repositoryTbl_Damages.Update(temp);
                    #region update Ticket
                    Tbl_Ticket tbl_Ticket = _repositoryTbl_Ticket.GetById(temp.IdTicket);
                    if (tbl_Ticket != null)
                    {

                        tbl_Ticket.Status = 3;
                        tbl_Ticket.DateUpdate = DateTime.Now;
                        tbl_Ticket.IdUserUpdate = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_Ticket.Update(tbl_Ticket);
                    }

                    #endregion
                }

                //activity log
                _customerActivityService.InsertActivity("disapprovalFinishedDamagesPayment", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), temp.Id.ToString());
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));

            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }
        #endregion


        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            var Damages = _repositoryTbl_Damages.GetById(id);

            if (Damages == null || Damages.IsActive == false)
            {
                //No category found with the specified id
                return RedirectToAction("List");
            }
            if (Damages.StaffIdAccept == null)
            {
                Damages.StaffIdAccept = _workContext.CurrentCustomer.Id;
                _repositoryTbl_Damages.Update(Damages);
            }
            if (Damages.DateStaffAccept == null)
            {
                Damages.DateStaffAccept = DateTime.Now;
                Damages.Status = 1;
                _repositoryTbl_Damages.Update(Damages);
            }

            vmDetailDamages model = new vmDetailDamages();
            model.Tbl_Damages = _repositoryTbl_Damages.GetById(id);
            model.TypeGoods = model.Tbl_Damages.Stock == true ? "کالای دست دوم" : "کالای نو";
            model.NameCustomer = _workContext.CurrentCustomer.GetFullName();
            model.Status = model.Tbl_Damages.Status == 0 ? "در صف انتظار" : model.Tbl_Damages.Status == 1 ? "در حال بررسی" : model.Tbl_Damages.Status == 2 ? "پاسخ داده شده" : model.Tbl_Damages.Status == 3 ? "عدم رعایت قانون 24" : model.Tbl_Damages.Status == 4 ? "عدم تایید و پایان" : model.Tbl_Damages.Status == 5 ? " تایید،پرداخت غرامت و پایان" : "-";
            List<Tbl_Damages_Detail> lll = _repositoryTbl_Damages_Detail.Table.Where(p => p.IdDamages == model.Tbl_Damages.Id).OrderByDescending(p => p.Id).ToList();
            model.vmDamages_Detail = new List<vmDamages_Detail>();
            foreach (var item in lll)
            {
                vmDamages_Detail temp = new vmDamages_Detail();
                temp.NameStaff = item.StaffId != null ? _customerService.GetCustomerById(item.StaffId.GetValueOrDefault()).GetFullName() : "";
                temp.List_Detail = item;
                model.vmDamages_Detail.Add(temp);

            }
            return View("/Plugins/Misc.Ticket/Views/Damages/AnswerDamages.cshtml", model);
        }


        [HttpPost]
        [AdminAntiForgery(false)]
        public IActionResult AddDetailDamages(int Id, string Description, bool statenewpattern, string Title)//, HttpPostedFileBase[] _files
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                    return Challenge();//login
                Ticket.Domain.Tbl_Damages Damages = _repositoryTbl_Damages.GetById(Id);

                //var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "TicketFiles");
                //var filename = "";
                //List<string> listfilename = new List<string>();
                #region
                //foreach (HttpPostedFileBase file in _files)
                //{
                //    //Checking file is available to save.  
                //    if (file != null)
                //    {
                //        if (file.ContentLength > 3145728)
                //        {
                //            //return new System.Web.Mvc.HttpStatusCodeResult(532);
                //            return Json(new { error = false, status = 532 });
                //        }
                //        var number = new Random();
                //        filename = ticket.Id.ToString() + "_" + number.Next(1, 999999999).ToString();
                //        listfilename.Add(filename);
                //        var path = Path.Combine(uploads, filename);
                //        file.SaveAs(path);
                //    }
                //}
                #endregion
                #region  add Deateil
                Ticket.Domain.Tbl_Damages_Detail temp = new Ticket.Domain.Tbl_Damages_Detail();
                temp.DateInsert = DateTime.Now;
                temp.Description = Description;
                temp.IdDamages = Damages.Id;
                temp.StaffId = _workContext.CurrentCustomer.Id;
                temp.Type = true;
                //if (listfilename.Count == 1)
                //{
                //    temp.UrlFile1 = listfilename[0];
                //}
                //if (listfilename.Count == 2)
                //{
                //    temp.UrlFile2 = listfilename[1];
                //}
                //if (listfilename.Count == 3)
                //{
                //    temp.UrlFile3 = listfilename[2];
                //}

                _repositoryTbl_Damages_Detail.Insert(temp);

                #endregion
                #region Update Ticket
                Damages.Status = 3;
                Damages.StaffIdLastAnswer = _workContext.CurrentCustomer.Id;
                Damages.DateStaffLastAnswer = DateTime.Now;
                _repositoryTbl_Damages.Update(Damages);
                #endregion
                #region send sms to customer
                string msg = "کاربر گرامی ، پاسخ درخواست خسارت به شماره: " + Damages.Id.ToString() + " داده شده است، بررسی بفرمایید، سامانه امنیتو";
                var sended = _notificationService._sendSms(_customerService.GetCustomerById(Damages.IdCustomer).Username, msg);
                #region log sms
                Tbl_LogSMS log1 = new Tbl_LogSMS();
                log1.Type = 3;
                log1.Mobile = _workContext.CurrentCustomer.Username;
                log1.StoreId = _storeContext.CurrentStore.Id;
                log1.TextMessage = msg;
                log1.Status = sended == true ? 1 : 0;
                log1.DateSend = DateTime.Now;
                _repositoryTbl_LogSMS.Insert(log1);

                #endregion
                #endregion
                #region pattern answer
                if (statenewpattern)
                {
                    Tbl_PatternAnswer_Ticket_Damages_RequestCOD tempp = new Tbl_PatternAnswer_Ticket_Damages_RequestCOD();
                    tempp.TitlePatternAnswer = Title;
                    tempp.DescriptipnPatternAnswer = Description;
                    tempp.DateInsert = DateTime.Now;
                    tempp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    tempp.IsActive = true;
                    tempp.Type = 2;
                    _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Insert(tempp);

                }
                #endregion

                return Json(new { success = true });
            }
            catch
            {
                return Json(new { error = true });
            }
        }





    }
}
