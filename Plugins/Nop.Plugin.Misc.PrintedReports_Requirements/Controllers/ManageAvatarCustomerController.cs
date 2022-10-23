using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models.Grid;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models.Search;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nop.Plugin.Misc.PrintedReports_Requirements.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageAvatarCustomerController : BaseAdminController
    {
        private readonly IRepository<Tbl_CheckAvatarCustomer> _repositoryTbl_CheckAvatarCustomer;
        private readonly IRepository<Ticket.Domain.Tbl_Ticket> _repositoryTbl_Ticket;
        private readonly IRepository<Tbl_LogSMS> _repositoryTbl_LogSMS;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly INotificationService _notificationService;
        private readonly IStoreContext _storeContext;
        public ManageAvatarCustomerController
            (
        IRepository<Tbl_CheckAvatarCustomer> repositoryTbl_CheckAvatarCustomer,
        IWorkContext workContext,
        IDbContext dbContext,
        IStoreContext storeContext,
        IPermissionService permissionService,
        ICustomerService customerService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        IPictureService pictureService,
        IRepository<Tbl_LogSMS> repositoryTbl_LogSMS,
        MediaSettings mediaSettings,
        INotificationService notificationService,
            IRepository<Ticket.Domain.Tbl_Ticket> repositoryTbl_Ticket
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
            _repositoryTbl_Ticket = repositoryTbl_Ticket;
            _repositoryTbl_LogSMS = repositoryTbl_LogSMS;
            _notificationService = notificationService;
            _storeContext = storeContext;
        }
        #region index&list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Model = new Search_Customer();
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

            try
            {
                var search_ = JsonConvert.DeserializeObject<Search_Customer>(TempData["_search_Customer"].ToString());
                if (search_.SearchId > 0)
                {
                    Model.SearchId = search_.SearchId;

                }
            }
            catch
            {

            }

            return View("/Plugins/Misc.PrintedReports_Requirements/Views/Customer/List.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult CustomerList(DataSourceRequest command, Search_Customer model, int[] searchCustomerRoleIds)
        {

            String ErrrorMassege = "";
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();
            var gridModel = new DataSourceResult();
            var Final_Customer = new List<Grid_Customer>();

            List<Tbl_CheckAvatarCustomer> Tbl_AvatarCustomer = _repositoryTbl_CheckAvatarCustomer.Table.GroupBy(p => p.CustomerId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();




            try
            {

                if (model.ActiveSearch == true)
                {


                    if (searchCustomerRoleIds.Count() > 0 || string.IsNullOrEmpty(model.SearchEmail) || string.IsNullOrEmpty(model.SearchUsername) || string.IsNullOrEmpty(model.SearchFirstName) || string.IsNullOrEmpty(model.SearchLastName))
                    {
                        var search_Customer = _customerService.GetAllCustomers(
                            customerRoleIds: searchCustomerRoleIds,
                            email: model.SearchEmail,
                            username: model.SearchUsername,
                            firstName: model.SearchFirstName,
                            lastName: model.SearchLastName,
                            loadOnlyWithShoppingCart: false,
                            pageIndex: command.Page - 1,
                            pageSize: command.PageSize);

                        Final_Customer = (from a in Tbl_AvatarCustomer
                                          join c in search_Customer on a.CustomerId equals c.Id
                                          select new Grid_Customer
                                          {
                                              Id = a.Id,
                                              Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                                              Username = c.Username,
                                              FullName = c.GetFullName(),
                                              CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList()),
                                              AvatarUrl = _pictureService.GetPictureUrl(
                                                               c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                                               _mediaSettings.AvatarPictureSize,
                                                               false),
                                              StateVerify = a.StateVerify == 0 ? "بدون بررسی" : a.StateVerify == 1 ? "عدم تایید  " : "تایید",
                                              DateVerify = a.DateVerify,
                                              NameUserVerify = a.IdUserVerify == null ? "" : _customerService.GetCustomerById((int)a.IdUserVerify).GetFullName(),
                                          }).ToList();

                    }
                    else if (model.UserOkAvatar)
                    {
                        Tbl_AvatarCustomer = Tbl_AvatarCustomer.Where(p => p.StateVerify == 2).ToList();
                        foreach (var item in Tbl_AvatarCustomer)
                        {
                            Grid_Customer temp = new Grid_Customer();
                            var c = _customerService.GetCustomerById(item.CustomerId);
                            temp.Id = item.Id;
                            temp.Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest");
                            temp.Username = c.Username;
                            temp.FullName = c.GetFullName();
                            temp.CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList());
                            temp.AvatarUrl = _pictureService.GetPictureUrl(
                                              c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                              _mediaSettings.AvatarPictureSize,
                                              false);
                            temp.StateVerify = item.StateVerify == 0 ? "بدون بررسی" : item.StateVerify == 1 ? "عدم تایید  " : "تایید";
                            temp.DateVerify = item.DateVerify;
                            temp.NameUserVerify = item.IdUserVerify == null ? "" : _customerService.GetCustomerById((int)item.IdUserVerify).GetFullName();
                            Final_Customer.Add(temp);

                        }
                    }
                    else if (model.UserNotOkAvatar)
                    {
                        //p => p.StateVerify == 1
                        Tbl_AvatarCustomer = Tbl_AvatarCustomer.ToList();
                        foreach (var item in Tbl_AvatarCustomer)
                        {
                            Grid_Customer temp = new Grid_Customer();
                            var c = _customerService.GetCustomerById(item.CustomerId);
                            temp.Id = item.Id;
                            temp.Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest");
                            temp.Username = c.Username;
                            temp.FullName = c.GetFullName();
                            temp.CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList());
                            temp.AvatarUrl = _pictureService.GetPictureUrl(
                                              c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                              _mediaSettings.AvatarPictureSize,
                                              false);
                            temp.StateVerify = item.StateVerify == 0 ? "بدون بررسی" : item.StateVerify == 1 ? "عدم تایید  " : "تایید";
                            temp.DateVerify = item.DateVerify;
                            temp.NameUserVerify = item.IdUserVerify == null ? "" : _customerService.GetCustomerById((int)item.IdUserVerify).GetFullName();
                            Final_Customer.Add(temp);

                        }
                    }
                    else if (model.SearchId > 0)
                    {
                        Tbl_CheckAvatarCustomer x = _repositoryTbl_CheckAvatarCustomer.GetById(model.SearchId);
                        Tbl_AvatarCustomer.Clear();
                        Final_Customer.Clear();
                        Tbl_AvatarCustomer.Add(x);
                        foreach (var item in Tbl_AvatarCustomer)
                        {
                            Grid_Customer temp = new Grid_Customer();
                            var c = _customerService.GetCustomerById(item.CustomerId);
                            temp.Id = item.Id;
                            temp.Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest");
                            temp.Username = c.Username;
                            temp.FullName = c.GetFullName();
                            temp.CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList());
                            temp.AvatarUrl = _pictureService.GetPictureUrl(
                                              c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                              _mediaSettings.AvatarPictureSize,
                                              false);
                            temp.StateVerify = item.StateVerify == 0 ? "بدون بررسی" : item.StateVerify == 1 ? "عدم تایید  " : "تایید";
                            temp.DateVerify = item.DateVerify;
                            temp.NameUserVerify = item.IdUserVerify == null ? "" : _customerService.GetCustomerById((int)item.IdUserVerify).GetFullName();
                            Final_Customer.Add(temp);

                        }
                    }
                    else
                    {
                        Tbl_AvatarCustomer = Tbl_AvatarCustomer.ToList();
                        foreach (var item in Tbl_AvatarCustomer)
                        {
                            Grid_Customer temp = new Grid_Customer();
                            var c = _customerService.GetCustomerById(item.CustomerId);
                            temp.Id = item.Id;
                            temp.Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest");
                            temp.Username = c.Username;
                            temp.FullName = c.GetFullName();
                            temp.CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList());
                            temp.AvatarUrl = _pictureService.GetPictureUrl(
                                              c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                              _mediaSettings.AvatarPictureSize,
                                              false);
                            temp.StateVerify = item.StateVerify == 0 ? "بدون بررسی" : item.StateVerify == 1 ? "عدم تایید  " : "تایید";
                            temp.DateVerify = item.DateVerify;
                            temp.NameUserVerify = item.IdUserVerify == null ? "" : _customerService.GetCustomerById((int)item.IdUserVerify).GetFullName();
                            Final_Customer.Add(temp);

                        }

                        //Final_Customer = (from a in Tbl_AvatarCustomer
                        //                  join c in search_Customer on a.CustomerId equals c.Id
                        //                  select new Grid_Customer
                        //                  {
                        //                      Id = a.Id,
                        //                      Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                        //                      Username = c.Username,
                        //                      FullName = c.GetFullName(),
                        //                      CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList()),
                        //                      StateVerify = a.StateVerify == 0 ? "بدون بررسی" : a.StateVerify == 1 ? "عدم تایید  " : "تایید",
                        //                      DateVerify = a.DateVerify,
                        //                      NameUserVerify = a.IdUserVerify == null ? "" : _customerService.GetCustomerById((int)a.IdUserVerify).GetFullName(),
                        //                      AvatarUrl = _pictureService.GetPictureUrl(
                        //                                       c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                        //                                       _mediaSettings.AvatarPictureSize,
                        //                                       false)

                        //                  }).ToList();

                    }
                }
                else
                {
                    Tbl_AvatarCustomer = Tbl_AvatarCustomer.Where(p => p.StateVerify == 0).ToList();
                    foreach (var item in Tbl_AvatarCustomer)
                    {
                        Grid_Customer temp = new Grid_Customer();
                        var c = _customerService.GetCustomerById(item.CustomerId);
                        temp.Id = item.Id;
                        temp.Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest");
                        temp.Username = c.Username;
                        temp.FullName = c.GetFullName();
                        temp.CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList());
                        temp.AvatarUrl = _pictureService.GetPictureUrl(
                                          c.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                                          _mediaSettings.AvatarPictureSize,
                                          false);
                        temp.StateVerify = item.StateVerify == 0 ? "بدون بررسی" : item.StateVerify == 1 ? "عدم تایید  " : "تایید";
                        temp.DateVerify = item.DateVerify;
                        temp.NameUserVerify = item.IdUserVerify == null ? "" : _customerService.GetCustomerById((int)item.IdUserVerify).GetFullName();
                        Final_Customer.Add(temp);

                    }

                }
            }
            catch (Exception ex)
            {
                ErrrorMassege = ex.ToString();
            }
            finally
            {
                gridModel = new DataSourceResult
                {
                    Data = Final_Customer,
                    Total = Final_Customer.Count
                };

            }

            return Json(gridModel);
        }

        public virtual IActionResult RelationController(int Id)
        {
            Search_Customer search_Customer = new Search_Customer();
            search_Customer.ActiveSearch = true;
            search_Customer.SearchId = Id;
            TempData["_search_Customer"] = JsonConvert.SerializeObject(search_Customer);
            return RedirectToAction("List");
        }
        protected virtual string GetCustomerRolesNames(IList<CustomerRole> customerRoles, string separator = ",")
        {
            var sb = new StringBuilder();
            for (var i = 0; i < customerRoles.Count; i++)
            {
                sb.Append(customerRoles[i].Name);
                if (i != customerRoles.Count - 1)
                {
                    sb.Append(separator);
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }

        #endregion

        #region Active & Disable
        public virtual IActionResult Disable(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_CheckAvatarCustomer checkavatar = _repositoryTbl_CheckAvatarCustomer.GetById(id);

                if (checkavatar == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    checkavatar.StateVerify = 1;
                    checkavatar.DateVerify = DateTime.Now;
                    checkavatar.IdUserVerify = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CheckAvatarCustomer.Update(checkavatar);
                }

                //activity log
                _customerActivityService.InsertActivity("NotOkAvatar", _localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyNotOk"), checkavatar.Id.ToString());

                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyNotOk"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual IActionResult DisableSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_CheckAvatarCustomer checkavatar = _repositoryTbl_CheckAvatarCustomer.GetById(item);

                        if (checkavatar != null)
                        {

                            checkavatar.StateVerify = 1;
                            checkavatar.DateVerify = DateTime.Now;
                            checkavatar.IdUserVerify = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_CheckAvatarCustomer.Update(checkavatar);
                            #region update Ticket
                            Tbl_Ticket tbl_Ticket = _repositoryTbl_Ticket.GetById(checkavatar.IdTicket);
                            if (tbl_Ticket != null)
                            {

                            tbl_Ticket.Status = 3;
                            tbl_Ticket.DateUpdate = DateTime.Now;
                            tbl_Ticket.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Ticket.Update(tbl_Ticket);
                            }
                            #endregion
                            #region send sms to customer
                            string msg = "کاربر گرامی ، عکس آواتار حساب کاربری شما تایید نشد : " + " سامانه امنیتو";
                            var sended = _notificationService._sendSms(_customerService.GetCustomerById(checkavatar.CustomerId).Username, msg);
                            #region log sms
                            Tbl_LogSMS log1 = new Tbl_LogSMS();
                            log1.Type = 7;
                            log1.Mobile = _workContext.CurrentCustomer.Username;
                            log1.StoreId = _storeContext.CurrentStore.Id;
                            log1.TextMessage = msg;
                            log1.Status = sended == true ? 1 : 0;
                            log1.DateSend = DateTime.Now;
                            _repositoryTbl_LogSMS.Insert(log1);

                            #endregion
                            #endregion
                        }
                    }
                    _customerActivityService.InsertActivity("NotOkAvatar", _localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyNotOk"));
                    SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyNotOk"));

                }
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }

        //Active
        [HttpPost]
        public virtual IActionResult Active(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_CheckAvatarCustomer checkavatar = _repositoryTbl_CheckAvatarCustomer.GetById(id);

                if (checkavatar == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    checkavatar.StateVerify = 2;
                    checkavatar.DateVerify = DateTime.Now;
                    checkavatar.IdUserVerify = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CheckAvatarCustomer.Update(checkavatar);
                }
                //activity log
                _customerActivityService.InsertActivity("OkAvatar", _localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyOk"), checkavatar.Id.ToString());
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyOk"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return RedirectToAction("List");
        }
        [HttpPost]
        public virtual IActionResult ActiveSelected(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_CheckAvatarCustomer checkavatar = _repositoryTbl_CheckAvatarCustomer.GetById(item);

                        if (checkavatar != null)
                        {

                            checkavatar.StateVerify = 2;
                            checkavatar.DateVerify = DateTime.Now;
                            checkavatar.IdUserVerify = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_CheckAvatarCustomer.Update(checkavatar);
                            #region update Ticket
                            Tbl_Ticket tbl_Ticket = _repositoryTbl_Ticket.GetById(checkavatar.IdTicket);
                            if (tbl_Ticket != null)
                            {
                            tbl_Ticket.Status = 3;
                            tbl_Ticket.DateUpdate = DateTime.Now;
                            tbl_Ticket.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Ticket.Update(tbl_Ticket);
                            }
                            #endregion
                            #region send sms to customer
                            string msg = "کاربر گرامی ، عکس آواتار حساب کاربری شما تایید شد : " + " سامانه امنیتو";
                            var sended = _notificationService._sendSms(_customerService.GetCustomerById(checkavatar.CustomerId).Username, msg);
                            #region log sms
                            Tbl_LogSMS log1 = new Tbl_LogSMS();
                            log1.Type = 7;
                            log1.Mobile = _workContext.CurrentCustomer.Username;
                            log1.StoreId = _storeContext.CurrentStore.Id;
                            log1.TextMessage = msg;
                            log1.Status = sended == true ? 1 : 0;
                            log1.DateSend = DateTime.Now;
                            _repositoryTbl_LogSMS.Insert(log1);

                            #endregion
                            #endregion
                        }
                    }
                }
                _customerActivityService.InsertActivity("OkAvatar", _localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyOk"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.PrintedReports_Requirements.MessageStateVerifyOk"));

            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }
        #endregion


    }
}
