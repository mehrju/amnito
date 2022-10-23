using Microsoft.AspNetCore.Mvc;
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
    public class ManageDiscountPlan_AgentCustomerController : BaseAdminController
    {
        private readonly IRepository<Tbl_DiscountPlan_AgentCustomer> _repositoryTbl_DiscountPlan_AgentCustomer;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        public ManageDiscountPlan_AgentCustomerController
            (
        IRepository<Tbl_DiscountPlan_AgentCustomer> repositoryTbl_DiscountPlan_AgentCustomer,
        IWorkContext workContext,
        IDbContext dbContext,
        IPermissionService permissionService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService
            )
        {
            _repositoryTbl_DiscountPlan_AgentCustomer = repositoryTbl_DiscountPlan_AgentCustomer;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
        }
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

            var Model = new Search_DiscountPlan();
            return View("/Plugins/Misc.ShippingSolutions/Views/DiscountPlan_AgentCustomer/List.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult DiscountPlansList(DataSourceRequest command, Search_DiscountPlan model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_DiscountPlan_AgentCustomer> Plans = new List<Tbl_DiscountPlan_AgentCustomer>();
            var gridModel = new DataSourceResult();
            var Final_Plans = (dynamic)null;
            try
            {
                Plans = _repositoryTbl_DiscountPlan_AgentCustomer.Table.ToList();
                if (Plans.Count > 0)
                {
                    if (model.Search_DiscountPlan_ActiveSearch == true)
                    {
                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_DiscountPlan_Name))
                        {
                            Plans = Plans.Where(p => p.Name.Contains(model.Search_DiscountPlan_Name)).ToList();
                        }

                        Plans = Plans.Where(p => p.IsActive == model.Search_DiscountPlan_IsActive).ToList();
                        Plans = Plans.Where(p => p.IsAgent == model.Search_DiscountPlan_IsAgent).ToList();


                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Plans = (from q in Plans
                               select new Grid_DiscountPlan
                               {
                                   Id = q.Id,
                                   Grid_DiscountPlan_Name = q.Name,
                                   Grid_DiscountPlan_UpAmount = q.UpAmount,
                                   Grid_DiscountPlan_OfAmount = q.OfAmount,
                                   Grid_DiscountPlan_ExpireDay = q.ExpireDay,
                                   Grid_DiscountPlan_IsAgent = q.IsAgent,
                                   Grid_DiscountPlan_IsActive = q.IsActive,
                                   Grid_DiscountPlan_Percent=q._Percent,
                                   Grid_DiscountPlan_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                   Grid_DiscountPlan_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                   Grid_DiscountPlan_DateInsert = q.DateInsert,
                                   Grid_DiscountPlan_DateUpdate = q.DateUpdate,
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
                    Data = Final_Plans,
                    Total = Plans.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }
        #endregion
        #region Diable & Active
        [HttpPost]
        public virtual IActionResult Disable(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_DiscountPlan_AgentCustomer plan = _repositoryTbl_DiscountPlan_AgentCustomer.Table.Where(p => p.Id == id).FirstOrDefault();

                if (plan == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    plan.IsActive = false;
                    _repositoryTbl_DiscountPlan_AgentCustomer.Update(plan);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableDiscountPlan", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"), plan.Name);
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
                        Tbl_DiscountPlan_AgentCustomer plan = _repositoryTbl_DiscountPlan_AgentCustomer.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (plan != null)
                        {

                            plan.IsActive = false;
                            plan.DateUpdate = DateTime.Now;
                            plan.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_DiscountPlan_AgentCustomer.Update(plan);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableDisCountPlan", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
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
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_DiscountPlan_AgentCustomer plan = _repositoryTbl_DiscountPlan_AgentCustomer.Table.Where(p => p.Id == id).FirstOrDefault();

                if (plan == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    plan.IsActive = true;
                    _repositoryTbl_DiscountPlan_AgentCustomer.Update(plan);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveDiscountPlan", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), plan.Name);
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
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_DiscountPlan_AgentCustomer plan = _repositoryTbl_DiscountPlan_AgentCustomer.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (plan != null)
                        {

                            plan.IsActive = true;
                            plan.DateUpdate = DateTime.Now;
                            plan.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_DiscountPlan_AgentCustomer.Update(plan);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveDiscountplan", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
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

            return View("/Plugins/Misc.ShippingSolutions/Views/DiscountPlan_AgentCustomer/Create.cshtml");
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_DiscountPlan_AgentCustomer model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_DiscountPlan_AgentCustomer duplicate = _repositoryTbl_DiscountPlan_AgentCustomer.Table.Where(p => p.Name == model.Name).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                return View("/Plugins/Misc.ShippingSolutions/Views/DiscountPlan_AgentCustomer/Create.cshtml");

            }
            #endregion
            if (ModelState.IsValid)
            {
                
            }
            Tbl_DiscountPlan_AgentCustomer paln = new Tbl_DiscountPlan_AgentCustomer();
            paln.Name = model.Name;
            paln.OfAmount = model.OfAmount;
            paln.UpAmount = model.UpAmount;
            paln.ExpireDay = model.ExpireDay;
            paln.IsAgent = model.IsAgent;
            paln._Percent = model._Percent;
            paln.IsActive = true;
            paln.DateInsert = DateTime.Now;
            paln.IdUserInsert = _workContext.CurrentCustomer.Id;
            //ServiceType.Id = 1;
            _repositoryTbl_DiscountPlan_AgentCustomer.Insert(paln);

            //activity log
            _customerActivityService.InsertActivity("AddNewDisCountPlan", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), paln.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = paln.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var paln = _repositoryTbl_DiscountPlan_AgentCustomer.GetById(id);
            if (paln == null || paln.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = paln;
            return View("/Plugins/Misc.ShippingSolutions/Views/DiscountPlan_AgentCustomer/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_DiscountPlan_AgentCustomer model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var paln = _repositoryTbl_DiscountPlan_AgentCustomer.GetById(model.Id);
            if (paln == null || paln.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            paln.Name = model.Name;
            paln.OfAmount = model.OfAmount;
            paln.UpAmount = model.UpAmount;
            paln.ExpireDay = model.ExpireDay;
            paln.IsAgent = model.IsAgent;
            paln._Percent = model._Percent;
            paln.DateUpdate = DateTime.Now;
            paln.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_DiscountPlan_AgentCustomer.Update(paln);
            //activity log
            _customerActivityService.InsertActivity("EditDisCountPlan", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"), paln.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = paln.Id });
            }
            return RedirectToAction("List");




            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);

        }
        #endregion




    }
}
