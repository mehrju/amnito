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
    public class ManageDealerController : BaseAdminController
    {
        private readonly IRepository<Tbl_Dealer_Customer_ServiceProvider> _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider;

        private readonly IRepository<Tbl_ServicesProviders> _repositoryTbl_ServicesProviders;
        private readonly IRepository<Tbl_Dealer> _repositoryTbl_Dealer;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        public ManageDealerController
                   (
               IRepository<Tbl_Dealer_Customer_ServiceProvider> repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider,

             IRepository<Tbl_ServicesProviders> repositoryTbl_ServicesProviders,
               IRepository<Tbl_Dealer> repositoryTbl_Dealer,
               IWorkContext workContext,
               IDbContext dbContext,
               IPermissionService permissionService,
               IStaticCacheManager cacheManager,
               ILocalizationService localizationService,
               ICustomerActivityService customerActivityService,
               ICustomerService customerService
                   )
        {
            _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider = repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider;

            _repositoryTbl_ServicesProviders = repositoryTbl_ServicesProviders;
            _repositoryTbl_Dealer = repositoryTbl_Dealer;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
        }

        #region Index
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

            var Model = new Search_Dealer_Model();
            return View("/Plugins/Misc.ShippingSolutions/Views/Dealer/ListDealers.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult DealerList(DataSourceRequest command, Search_Dealer_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_Dealer> Dealer = new List<Tbl_Dealer>();
            var gridModel = new DataSourceResult();
            var Final_Dealer = (dynamic)null;
            try
            {
                Dealer = _repositoryTbl_Dealer.Table.ToList();
                if (Dealer.Count > 0)
                {
                    if (model.Search_Dealer_ActiveSearch == true)
                    {
                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_Dealer_Name))
                        {
                            Dealer = Dealer.Where(p => p.Name.Contains(model.Search_Dealer_Name)).ToList();
                        }

                        Dealer = Dealer.Where(p => p.IsActive == model.Search_Dealer_IsActive).ToList();

                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Dealer = (from q in Dealer
                                select new Grid_Dealer
                                {
                                    Id = q.Id,
                                    Grid_Dealer_Name = q.Name,
                                    Grid_Dealer_IsActive = q.IsActive,
                                    Grid_Dealer_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                    Grid_Dealer_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                    Grid_Dealer_DateInsert = q.DateInsert,
                                    Grid_Dealer_DateUpdate = q.DateUpdate,
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
                    Data = Final_Dealer,
                    Total = Dealer.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }
        [HttpPost]
        public virtual IActionResult ProvidersList(DataSourceRequest command, int Id)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_Dealer_Customer_ServiceProvider> tbl_Dealer_Customer_ServiceProvider = new List<Tbl_Dealer_Customer_ServiceProvider>();
            var gridModel = new DataSourceResult();
            var Final_Dealer_Customer_ServiceProvider = (dynamic)null;
            try
            {
                tbl_Dealer_Customer_ServiceProvider = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.DealerId == Id && p.TypeUser == 2).ToList();
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
        public virtual IActionResult Disable(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_Dealer Dealer = _repositoryTbl_Dealer.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Dealer == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                else
                {
                    Dealer.IsActive = false;
                    _repositoryTbl_Dealer.Update(Dealer);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"), Dealer.Name);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_Dealer Dealer = _repositoryTbl_Dealer.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Dealer != null)
                        {

                            Dealer.IsActive = false;
                            Dealer.DateUpdate = DateTime.Now;
                            Dealer.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Dealer.Update(Dealer);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_Dealer Dealer = _repositoryTbl_Dealer.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Dealer == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Dealer.IsActive = true;
                    _repositoryTbl_Dealer.Update(Dealer);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), Dealer.Name);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_Dealer Dealer = _repositoryTbl_Dealer.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Dealer != null)
                        {

                            Dealer.IsActive = true;
                            Dealer.DateUpdate = DateTime.Now;
                            Dealer.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Dealer.Update(Dealer);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }

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
                _customerActivityService.InsertActivity("DisableProviderinDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
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
                _customerActivityService.InsertActivity("ActiveProviderinDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
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
            var model = new Tbl_Dealer();
            #region ListStateApplyPricingPolicy
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/Dealer/Create.cshtml",model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_Dealer model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_Dealer duplicate = _repositoryTbl_Dealer.Table.Where(p => p.Name == model.Name).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                return View("/Plugins/Misc.ShippingSolutions/Views/Dealer/Create.cshtml");

            }
            #endregion
            Tbl_Dealer Dealer = new Tbl_Dealer();
            Dealer.Name = model.Name;
            Dealer.StateApplyPricingPolicy = model.StateApplyPricingPolicy;
            Dealer.IsActive = true;
            Dealer.DateInsert = DateTime.Now;
            Dealer.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Dealer.Insert(Dealer);

            //activity log
            _customerActivityService.InsertActivity("AddNewDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), Dealer.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Dealer.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/TransportationTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Dealer = _repositoryTbl_Dealer.GetById(id);
            if (Dealer == null || Dealer.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = new Tbl_Dealer();
            model = Dealer;
            #region ListStateApplyPricingPolicy
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion
            #region  get list provider
            var Providers = _repositoryTbl_ServicesProviders.Table.Where(p => p.IsActive == true).ToList();
            
            if (Providers.Count > 0)
            {
                model.ListProvider.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in Providers)
                {

                    model.ListProvider.Add(new SelectListItem { Text = c.ServicesProviderName, Value = c.Id.ToString() });
                }
            }
            else
            {
                model.ListProvider.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/Dealer/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Dealer model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Dealer = _repositoryTbl_Dealer.GetById(model.Id);
            if (Dealer == null || Dealer.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Dealer.Name = model.Name;
            Dealer.StateApplyPricingPolicy = model.StateApplyPricingPolicy;
            Dealer.DateUpdate = DateTime.Now;
            Dealer.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Dealer.Update(Dealer);

            if (model.ProviderId.Count() > 0)
            {
                foreach (var item in model.ProviderId)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_Dealer_Customer_ServiceProvider Duplicate = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Table.Where(p => p.DealerId == model.Id && p.ProviderId == item).FirstOrDefault();
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
                        temp.DealerId = model.Id;
                        temp.ProviderId = item;
                        temp.TypeUser = 2;
                        _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Insert(temp);

                    }

                }

            }
            //activity log
            _customerActivityService.InsertActivity("EditDealer", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"), Dealer.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Dealer.Id });
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
                model.TypeUser = 2;
                model.Dealer_Customer_Id = DealerCustomer.Id;
                model.CountryId = 0;
                model.ProviderId = DealerCustomer.ProviderId;
                model.NameUser ="واسطه:"+ _repositoryTbl_Dealer.GetById(DealerCustomer.DealerId).Name+" سرویس دهنده:"+_repositoryTbl_ServicesProviders.GetById(DealerCustomer.ProviderId).ServicesProviderName;
                return View("/Plugins/Misc.ShippingSolutions/Views/PricingPolicy/Edit.cshtml", model);

            }
        }
        #endregion


    }
}
