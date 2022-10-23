using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Models.Grid;
using Nop.Plugin.Misc.ShippingSolutions.Models.Search;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageCustomerServiceProviderController : BaseAdminController
    {
        private readonly IRepository<Tbl_Dealer_Customer_ServiceProvider> _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider;
        private readonly IRepository<Tbl_ServicesProviders> _repositoryTbl_ServicesProviders;

        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;


        public ManageCustomerServiceProviderController
                    (
             IRepository<Tbl_ServicesProviders> repositoryTbl_ServicesProviders,

                IRepository<Tbl_Dealer_Customer_ServiceProvider> repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider,
                IWorkContext workContext,
                IDbContext dbContext,
                IPermissionService permissionService,
                ICustomerService customerService,
                IStaticCacheManager cacheManager,
                ILocalizationService localizationService,
                ICustomerActivityService customerActivityService


            )
        {
            _repositoryTbl_ServicesProviders = repositoryTbl_ServicesProviders;

            _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider = repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
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



            return View("/Plugins/Misc.ShippingSolutions/Views/Customer/List.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult CustomerList(DataSourceRequest command, Search_Customer model, int[] searchCustomerRoleIds)
        {
            String ErrrorMassege = "";
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();
            var gridModel = new DataSourceResult();
            var Final_Customer =new  List< Grid_Customer>();

            List<Tbl_Dealer_Customer_ServiceProvider> tbl_customersserviceprovider = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.TypeUser == 1).GroupBy(p => p.CustomerId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();

            try
            {
                if (tbl_customersserviceprovider.Count > 0)
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

                            Final_Customer = (from a in tbl_customersserviceprovider
                                              join c in search_Customer on a.CustomerId equals c.Id
                                              select new Grid_Customer
                                              {
                                                  Id = a.Id,
                                                  Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                                                  Username = c.Username,
                                                  FullName = c.GetFullName(),
                                                  CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList()),
                                                  Grid_State_Negative_credit = a.StateNegative_credit_amount,
                                                  Grid_Negative_credit_amount = a.Negative_credit_amount
                                                }).ToList();
                        }
                        else
                        {
                            tbl_customersserviceprovider = tbl_customersserviceprovider.Where(p => p.StateNegative_credit_amount == model.Search_State_Negative_credit).ToList();
                            tbl_customersserviceprovider = tbl_customersserviceprovider.Where(p => p.Negative_credit_amount == model.Search_Negative_credit_amount).ToList();
                            foreach (var item in tbl_customersserviceprovider)
                            {
                                Grid_Customer temp = new Grid_Customer();
                                var c = _customerService.GetCustomerById(item.CustomerId);
                                temp.Id = item.Id;
                                temp.Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest");
                                temp.Username = c.Username;
                                temp.FullName = c.GetFullName();
                                temp.CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList());
                                temp.Grid_State_Negative_credit = item.StateNegative_credit_amount;
                                temp.Grid_Negative_credit_amount = item.Negative_credit_amount;
                                Final_Customer.Add(temp);

                            }
                        }

                    }

                    else
                    {

                        foreach (var item in tbl_customersserviceprovider)
                        {
                            Grid_Customer temp = new Grid_Customer();
                            var c = _customerService.GetCustomerById(item.CustomerId);
                            temp.Id = item.Id;
                            temp.Email = c.IsRegistered() ? c.Email : _localizationService.GetResource("Admin.Customers.Guest");
                            temp.Username = c.Username;
                            temp.FullName = c.GetFullName();
                            temp.CustomerRoleNames = GetCustomerRolesNames(c.CustomerRoles.ToList());
                            temp.Grid_State_Negative_credit = item.StateNegative_credit_amount;
                            temp.Grid_Negative_credit_amount = item.Negative_credit_amount;
                            Final_Customer.Add(temp);

                        }
                       
                    }
                }
                else
                {
                    ErrrorMassege = "No Data Exist";
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
                    Total = Final_Customer.Count,
                    Errors=ErrrorMassege
                };

            }

            return Json(gridModel);
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

        [HttpPost]
        public virtual IActionResult ProvidersList(DataSourceRequest command, int Id)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_Dealer_Customer_ServiceProvider> tbl_Dealer_Customer_ServiceProvider = new List<Tbl_Dealer_Customer_ServiceProvider>();
            var gridModel = new DataSourceResult();
            var Final_Dealer_Customer_ServiceProvider = new List<Grid_Provider_In_Collector>();
            try
            {
                if (Id > 0)
                {
                    var idcustomer = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.GetById(Id).CustomerId;
                    tbl_Dealer_Customer_ServiceProvider = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.CustomerId == idcustomer && p.TypeUser == 1).ToList();
                    if (tbl_Dealer_Customer_ServiceProvider.Count > 0)
                    {
                        Final_Dealer_Customer_ServiceProvider = (from q in tbl_Dealer_Customer_ServiceProvider
                                                                 select new Grid_Provider_In_Collector
                                                                 {
                                                                     Id = q.Id,
                                                                     Grid_Provider_Name = _repositoryTbl_ServicesProviders.GetById(q.ProviderId).ServicesProviderName,
                                                                     Grid_Provider_IsActive = q.IsActive,
                                                                     Grid_Provider_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                                                     Grid_Provider_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                                                     Grid_Provider_DateInsert = q.DateInsert,
                                                                     Grid_Provider_DateUpdate = q.DateUpdate,
                                                                 }).ToList();
                    }
                    else
                    {
                        ErrrorMassege = "No Exsit Data";
                    }

                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
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
                    Data = Final_Dealer_Customer_ServiceProvider,
                    Total = tbl_Dealer_Customer_ServiceProvider.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }
        #endregion

        #region Diables & active 
        [HttpPost]
        public virtual IActionResult DisableProvider(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Dealer_Customer_ServiceProvider temp = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("DisableProviderinCustomer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActiveProvider(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Dealer_Customer_ServiceProvider temp = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.Id == id).FirstOrDefault();

                if (temp != null)
                {

                    temp.IsActive = true;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("ActiveProviderinCustomer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }
        #endregion

        #region Create / Edit 

        public virtual IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            var model = new Tbl_Dealer_Customer_ServiceProvider();
            model.ListCustomer.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            #region  get list provider
            var Providers = _repositoryTbl_ServicesProviders.Table.Where(p => p.IsActive == true).ToList();
            if (Providers.Count > 0)
            {
                model._ListProvider.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Providers)
                {

                    model._ListProvider.Add(new SelectListItem { Text = c.ServicesProviderName, Value = c.Id.ToString() });
                }
            }
            else
            {
                model._ListProvider.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            #region ListStateApplyPricingPolicy
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/Customer/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_Dealer_Customer_ServiceProvider model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            if (model._ProviderId.Count() > 0)
            {
                foreach (var item in model._ProviderId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_Dealer_Customer_ServiceProvider Duplicate = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.CustomerId == model.CustomerId && p.ProviderId == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_Dealer_Customer_ServiceProvider temp = new Tbl_Dealer_Customer_ServiceProvider();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.StateApplyPricingPolicy = model.StateApplyPricingPolicy;
                        temp.IsActive = true;
                        temp.CustomerId = model.CustomerId;
                        temp.StateNegative_credit_amount = model.StateNegative_credit_amount;
                        temp.Negative_credit_amount = model.Negative_credit_amount;
                        temp.ProviderId = item;
                        temp.TypeUser = 1;
                        _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Insert(temp);

                    }

                }

            }


            //activity log
            _customerActivityService.InsertActivity("AddNewCustomerinServiceProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), model.CustomerId.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = model.CustomerId });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/TransportationTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Customer = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.GetById(id) ;
            if (Customer == null || Customer.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = new Tbl_Dealer_Customer_ServiceProvider();
            model = Customer;
            #region user
            if (Customer.CustomerId > 0)
            {
                var Users = _customerService.GetCustomerById(Customer.CustomerId);
                if (Users != null)
                {


                    model.ListCustomer.Add(new SelectListItem { Text = Users.GetFullName(), Value = Users.Id.ToString(), Selected = true });

                    model.NameCustomer = "" + Users.GetFullName();
                }
                else
                {
                    model.ListCustomer.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
            }



            #endregion
            #region  get list provider
            var Providers = _repositoryTbl_ServicesProviders.Table.Where(p => p.IsActive == true).ToList();
            if (Providers.Count > 0)
            {
                model._ListProvider.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Providers)
                {

                    model._ListProvider.Add(new SelectListItem { Text = c.ServicesProviderName, Value = c.Id.ToString() });
                }
            }
            else
            {
                model._ListProvider.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            #region ListStateApplyPricingPolicy
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/Customer/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Dealer_Customer_ServiceProvider model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Customer = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.GetById(model.Id);
            if (Customer == null || Customer.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update all recorde
            //Customer.CustomerId = model.CustomerId;
            //Customer.TypeUser = 1; 
            Customer.DateUpdate = DateTime.Now;
            Customer.IdUserUpdate = _workContext.CurrentCustomer.Id;
            Customer.StateApplyPricingPolicy = model.StateApplyPricingPolicy;
            Customer.StateNegative_credit_amount = model.StateNegative_credit_amount;
            Customer.Negative_credit_amount = model.Negative_credit_amount;
            _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Update(Customer);

            if (model._ProviderId.Count() > 0)
            {
                foreach (var item in model._ProviderId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_Dealer_Customer_ServiceProvider Duplicate = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.CustomerId == model.CustomerId && p.ProviderId == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_Dealer_Customer_ServiceProvider temp = new Tbl_Dealer_Customer_ServiceProvider();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.IsActive = true;
                        temp.CustomerId = model.CustomerId;
                        temp.ProviderId = item;
                        temp.TypeUser = 1;
                        _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Insert(temp);

                    }

                }

            }
            //activity log
            _customerActivityService.InsertActivity("EditCustomerServiceProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Customer.Id });
            }
            return RedirectToAction("List");




            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);

        }



        public virtual IActionResult Editlimitation(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var DealerCustomer = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.GetById(id);
            if (DealerCustomer == null || DealerCustomer.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = new Tbl_Dealer_Customer_ServiceProvider();

            model = DealerCustomer;
            return View("/Plugins/Misc.ShippingSolutions/Views/DealerCustomerServiceProvider/Edit.cshtml", model);
        }

        public virtual IActionResult EditPricingPolicy(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var DealerCustomer = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.GetById(id);
            if (DealerCustomer == null || DealerCustomer.IsActive == false)
            {
                //No category found with the specified id
                return RedirectToAction("List");
            }
            else
            {
                Tbl_PricingPolicy model = new Tbl_PricingPolicy();
                model.TypeUser = 1;
                model.Dealer_Customer_Id = DealerCustomer.Id;
                model.CountryId = 0;
                model.ProviderId = DealerCustomer.ProviderId;
                model.NameUser = "مشتری: " + _customerService.GetCustomerById(DealerCustomer.CustomerId).GetFullName() + " سرویس دهنده:" + _repositoryTbl_ServicesProviders.GetById(DealerCustomer.ProviderId).ServicesProviderName;
                return View("/Plugins/Misc.ShippingSolutions/Views/PricingPolicy/Edit.cshtml", model);

            }
        }

        #endregion
    }
}
