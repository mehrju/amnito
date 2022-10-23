using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Misc.Ticket.Models;
using Nop.Plugin.Misc.Ticket.Models.Grid;
using Nop.Plugin.Misc.Ticket.Models.Search;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security.AntiXss;

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    public class ManageTicketController : BaseAdminController
    {
        private readonly IRepository<Tbl_Ticket_Department> _repositoryTbl_Ticket_Department;
        private readonly IRepository<Tbl_Ticket_Priority> _repositoryTbl_Ticket_Priority;
        private readonly IRepository<Tbl_Ticket_Staff_Department> _repositoryTbl_Ticket_Staff_Department;
        private readonly IRepository<Tbl_Ticket> _repositoryTbl_Ticket;
        private readonly IRepository<Tbl_Ticket_Detail> _repositoryTbl_Ticket_Detail;
        private readonly IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
        private readonly IRepository<Tbl_CategoryTicket> _repositoryTbl_CategoryTicket;
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
        private readonly ICountryService _countryService;
        private readonly IRepository<Order> _repositoryTbl_Order;
       
        public ManageTicketController
            (
             IOrderService orderService,
            IRepository<Tbl_CategoryTicket> repositoryTbl_CategoryTicket,
        IRepository<Tbl_Ticket_Department> repositoryTbl_Ticket_Department,
        IRepository<Tbl_Ticket_Priority> repositoryTbl_Ticket_Priority,
        IRepository<Tbl_Ticket_Staff_Department> repositoryTbl_Ticket_Staff_Department,
        IRepository<Tbl_Ticket> repositoryTbl_Ticket,
        IRepository<Tbl_Ticket_Detail> repositoryTbl_Ticket_Detail,
        IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD,
        ICustomerService customerService,
        IWorkContext workContext,
        IDbContext dbContext,
        IPermissionService permissionService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        IStoreService storeService,
        IHostingEnvironment hostingEnvironment,
        ICountryService countryService,
        IRepository<Order> repositoryTbl_Order
            )
        {
            _repositoryTbl_CategoryTicket = repositoryTbl_CategoryTicket;
            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD = repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
            _repositoryTbl_Ticket_Priority = repositoryTbl_Ticket_Priority;
            _repositoryTbl_Ticket_Department = repositoryTbl_Ticket_Department;
            _repositoryTbl_Ticket_Staff_Department = repositoryTbl_Ticket_Staff_Department;
            _repositoryTbl_Ticket = repositoryTbl_Ticket;
            _repositoryTbl_Ticket_Detail = repositoryTbl_Ticket_Detail;
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
            _countryService = countryService;
            _repositoryTbl_Order = repositoryTbl_Order;
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

            var Model = new Search_Ticket();
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
                #region dep
                var Deps = _repositoryTbl_Ticket_Department.Table.Where(p => p.IsActive == true).ToList();
                if (Deps.Count > 0)
                {
                    Model.ListDepartmentTicket.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                    foreach (var s in Deps)
                    {

                        Model.ListDepartmentTicket.Add(new SelectListItem { Text = (s.Name + "-" + (s.StoreId == 0 ? "همه فروشگاهها" : _storeService.GetStoreById(s.StoreId).Name)), Value = s.Id.ToString() });
                    }
                }
                else
                {
                    Model.ListDepartmentTicket.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
                #endregion
                #region priority
                var Prois = _repositoryTbl_Ticket_Priority.Table.Where(p => p.IsActive == true).ToList();
                if (Prois.Count > 0)
                {
                    Model.ListPriority.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                    foreach (var s in Prois)
                    {

                        Model.ListPriority.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                    }
                }
                else
                {
                    Model.ListPriority.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
                #endregion
                #region Status
                Model.ListStatus.Add(new SelectListItem { Text = "انتخاب کنید", Value = "-1", Selected = true });
                Model.ListStatus.Add(new SelectListItem { Text = "در صف انتظار", Value = "0", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "در حال بررسی", Value = "1", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "پاسخ داده شده", Value = "2", Selected = false });
                Model.ListStatus.Add(new SelectListItem { Text = "بسته شده", Value = "3", Selected = false });


                #endregion
                #region CategoryTicket
                var Cats = _repositoryTbl_CategoryTicket.Table.Where(p => p.IsActive).ToList();
                if (Cats.Count > 0)
                {
                    Model.ListcategoryTicket.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                    foreach (var s in Cats)
                    {

                        Model.ListcategoryTicket.Add(new SelectListItem { Text = s.NameCategoryTicket, Value = s.Id.ToString() });
                    }
                }
                else
                {
                    Model.ListcategoryTicket.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
                #endregion
                #region list ostan
                var countries = _countryService.GetAllCountries();
                if (countries.Count > 0)
                {
                    Model.ListOstanTicket.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                    foreach (var s in countries)
                    {

                        Model.ListOstanTicket.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                    }
                }
                else
                {
                    Model.ListOstanTicket.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
                #endregion
            }
            catch (Exception ex)
            {

            }
            return View("/Plugins/Misc.Ticket/Views/Ticket/ListTicket.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult TicketList(DataSourceRequest command, Search_Ticket model, int[] searchCustomerRoleIds)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageOrders))
                return AccessDeniedKendoGridJson();
            
            var gridModel = new DataSourceResult();
            var Final_Ticket = (dynamic)null;
            int count = 0;
            try
            {
                List<Tbl_Ticket> ListAllTicket = _repositoryTbl_Ticket.Table.OrderByDescending(p => p.Id).ToList();
                List<Tbl_Ticket_Staff_Department> StaffDepartment = _repositoryTbl_Ticket_Staff_Department.Table.Where(p => p.IsActive == true).ToList();

                //List<Tbl_Ticket> ListTicketStaffDefault = ListAllTicket;

                //ListTicketStaffDefault = (from l in ListAllTicket
                //                          join d in StaffDepartment on l.DepartmentId equals d.IdDepartment
                //                          where d.UserId == _workContext.CurrentCustomer.Id
                //                          select l).OrderByDescending(p => p.Id).ToList();




                if (ListAllTicket.Count > 0)
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

                            var temp = (from l in ListAllTicket
                                        join c in search_Customer on l.IdCustomer equals c.Id
                                        select l).ToList();

                            ListAllTicket = temp;

                        }


                        ListAllTicket = ListAllTicket.Where(p => p.IsActive != model.Search_Ticket_Isdeleted).ToList();
                        #region Search

                        if (model.SearchTicketDateFrom != null)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.DateInsert >= model.SearchTicketDateFrom.Value.AddDays(-1)).ToList();
                        }
                        if (model.SearchTicketDateTo != null)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.DateInsert <= model.SearchTicketDateTo.Value).ToList();
                        }

                        if (model.SearchUsername_customer > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.IdCustomer == model.SearchUsername_customer).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.SearchIssue))
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.Issue.Contains(model.SearchIssue)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.SearchTrackingCode))
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.TrackingCode.Contains(model.SearchTrackingCode)).ToList();
                        }
                        if (model.SearchOrderId > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.OrderCode == (model.SearchOrderId)).ToList();

                        }
                        if (model.SearchTicketNumber > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.Id == (model.SearchTicketNumber)).ToList();

                        }
                        if (model.SearchDepartmentId > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.DepartmentId == (model.SearchDepartmentId)).ToList();

                        }
                        if (model.SearchPriorityId > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.ProrityId == (model.SearchPriorityId)).ToList();

                        }
                        if (model.SearchCategoryId > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.IdCategoryTicket == (model.SearchCategoryId)).ToList();

                        }
                        if (model.SearchIdStatus > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.Status == (model.SearchIdStatus)).ToList();

                        }
                        if (model.SearchStoreId > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.StoreId == (model.SearchStoreId)).ToList();

                        }
                        if (model.SearchOstanOrginId > 0)
                        {
                            ListAllTicket = ListAllTicket.Where(p => p.OrderCode > 0).ToList();
                            var ListAllOrder = _repositoryTbl_Order.Table.Where(p => p.BillingAddress != null && p.BillingAddress.StateProvinceId.HasValue && p.BillingAddress.CountryId == model.SearchOstanOrginId).ToList();
                            var listallorder = (from t in ListAllTicket
                                                join o in ListAllOrder on t.OrderCode equals o.Id
                                                where o.BillingAddress.CountryId == model.SearchOstanOrginId
                                                select t).ToList();
                            ListAllTicket = listallorder;
                        }

                        #endregion
                    }
                    else
                    {
                        ListAllTicket = ListAllTicket.Where(p => p.Status == 0 || p.Status == 1).ToList();
                    }
                    count = ListAllTicket.Count;
                    if (command.Page == 0) command.Page = 1;
                    ListAllTicket = ListAllTicket.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).ToList();
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Ticket = (from q in ListAllTicket
                                select new Grid_Ticket
                                {
                                    Id = q.Id,
                                    Grid_Ticket_TicketNumber = q.Id.ToString(),
                                    Grid_Ticket_CategoryName = _repositoryTbl_CategoryTicket.GetById(q.IdCategoryTicket)?.NameCategoryTicket,
                                    Grid_Ticket_StoreName = _storeService.GetStoreById(q.StoreId).Name,
                                    Grid_Ticket_Isuue = q.Issue,
                                    Grid_Ticket_FullName = _customerService.GetCustomerById(q.IdCustomer)?.GetFullName(),
                                    Grid_Ticket_DepName = _repositoryTbl_Ticket_Department.GetById(q.DepartmentId)?.Name,
                                    Grid_Ticket_PriorityName = _repositoryTbl_Ticket_Priority.GetById(q.ProrityId)?.Name,
                                    Grid_Ticket_OrderId = checkordercode(q.OrderCode),
                                    Grid_Ticket_TrackingCode = q.TrackingCode,
                                    Grid_Ticket_Status = q.Status == 0 ? "در صف انتظار" : q.Status == 1 ? "در حال بررسی" : q.Status == 2 ? "پاسخ داده شده" : "پایان یافته",
                                    Grid_Ticket_DateInsert = q.DateInsert,
                                    Grid_Ticket_NameStaffOpen = q.StaffIdAccept != null ? _customerService.GetCustomerById((q.StaffIdAccept.GetValueOrDefault()))?.GetFullName() : "",
                                    Grid_Ticket_DateOpen = q.DateStaffAccept,
                                    Grid_Ticket_LastDateAnswer = q.DateStaffLastAnswer,
                                    Grid_Ticket_NameStaffLastAnswer = q.StaffIdLastAnswer != null ? _customerService.GetCustomerById(q.StaffIdLastAnswer.GetValueOrDefault())?.GetFullName() : "",
                                    Grid_Ticket_LastRank = 0,
                                    TypeTicket = q.TypeTicket,
                                    UrlPage = q.OrderCode.ToString()

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
                    Data = Final_Ticket,
                    Total = count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);
        }

        private string checkordercode(int? code)
        {
            if (code == null)
            {
                return "-";
            }
            var order = _orderService.GetOrderById(code.GetValueOrDefault());
            if (order == null)
            {
                return "-";
            }
            else
            {
                return code.ToString();
            }
        }

        [HttpGet]
        public IActionResult GetListPatternAnswer()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return Challenge();//login
            var listPattern = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.IsActive && p.Type == 1).Select(p => new
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
        public virtual IActionResult DisableTicket(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Ticket temp = _repositoryTbl_Ticket.GetById(id);

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Ticket.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult FinishedTicket(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Ticket temp = _repositoryTbl_Ticket.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.Status = 3;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Ticket.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("FinishedTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), temp.Id.ToString());
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

            var Ticket = _repositoryTbl_Ticket.GetById(id);

            if (Ticket == null || Ticket.IsActive == false)
            {
                //No category found with the specified id
                return RedirectToAction("List");
            }
            if (Ticket.StaffIdAccept == null)
            {
                Ticket.StaffIdAccept = _workContext.CurrentCustomer.Id;
                Ticket.DateStaffAccept = DateTime.Now;
                Ticket.Status = 1;
                _repositoryTbl_Ticket.Update(Ticket);
            }


            vmDetailTicket model = new vmDetailTicket();
            model.Tbl_Ticket = _repositoryTbl_Ticket.GetById(id);
            model.NameCategory = _repositoryTbl_CategoryTicket.GetById(model.Tbl_Ticket.IdCategoryTicket).NameCategoryTicket;
            var customer = _customerService.GetCustomerById(Ticket.IdCustomer);
            model.NameCustomer = customer.GetFullName();
            model.CustomerUserName = customer.Username;
            // _workContext.CurrentCustomer.GetFullName();
            model.NameDep = _repositoryTbl_Ticket_Department.GetById(model.Tbl_Ticket.DepartmentId).Name.ToString();
            model.Proirity = _repositoryTbl_Ticket_Priority.GetById(model.Tbl_Ticket.ProrityId).Name.ToString();
            model.Status = model.Tbl_Ticket.Status == 0 ? "در صف انتظار" : model.Tbl_Ticket.Status == 1 ? "در حال بررسی" : model.Tbl_Ticket.Status == 2 ? "پاسخ داده شده" : model.Tbl_Ticket.Status == 3 ? "پایان یافته" : "-";//درحل بررسی، پاسخ داده شده، پایان یافته
            List<Tbl_Ticket_Detail> lll = _repositoryTbl_Ticket_Detail.Table.Where(p => p.IdTicket == model.Tbl_Ticket.Id).OrderByDescending(p => p.Id).ToList();

            model.vmTicket_Detail = new List<vmTicket_Detail>();
            foreach (var item in lll)
            {
                if (!string.IsNullOrEmpty(item.Description))
                {
                    //tem.Description = AntiXssEncoder.HtmlEncode(item.Description, false);
                    item.Description = item.Description.Replace("\n", "<br>");
                }
                vmTicket_Detail temp = new vmTicket_Detail();
                temp.NameStaff = item.StaffId != null ? _customerService.GetCustomerById(item.StaffId.GetValueOrDefault()).GetFullName() : "";
                temp.List_Detail = item;
                model.vmTicket_Detail.Add(temp);

            }
            return View("/Plugins/Misc.Ticket/Views/Ticket/AnswerTicket.cshtml", model);
        }


        [HttpPost]
        [AdminAntiForgery(false)]
        public IActionResult AddDetailTicket(int Id, string Description, bool statenewpattern, string Title)//, HttpPostedFileBase[] _files
        {
            try
            {
                if (!_workContext.CurrentCustomer.IsRegistered())
                    return Challenge();//login
                Ticket.Domain.Tbl_Ticket ticket = _repositoryTbl_Ticket.GetById(Id);

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
                Ticket.Domain.Tbl_Ticket_Detail temp = new Ticket.Domain.Tbl_Ticket_Detail();
                temp.DateInsert = DateTime.Now;
                Description = Description.Replace("\n", "<br>");
                //Description = AntiXssEncoder.HtmlEncode(Description, true);
                temp.Description = Description;
                temp.IdTicket = ticket.Id;
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

                _repositoryTbl_Ticket_Detail.Insert(temp);

                #endregion
                #region Update Ticket
                ticket.Status = 2;
                ticket.StaffIdLastAnswer = _workContext.CurrentCustomer.Id;
                ticket.DateStaffLastAnswer = DateTime.Now;
                _repositoryTbl_Ticket.Update(ticket);
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
                    tempp.Type = 1;
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
    public class EmbedePdfController : BasePublicController
    {
        public EmbedePdfController()
        {

        }
        [HttpPost]
        [HttpGet]
        public ActionResult ReadFilePdf(string url)
        {
            ViewBag.UrlFil = url;
            return PartialView("~/Plugins/Misc.Ticket/Views/Ticket/ReadFilePdf.cshtml");
        }
    }
}
