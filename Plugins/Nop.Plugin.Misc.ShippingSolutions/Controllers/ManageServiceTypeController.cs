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
    public class ManageServiceTypeController : BaseAdminController
    {
        private readonly IRepository<Tbl_ServicesTypes> _repositoryTbl_ServicesTypes;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        public ManageServiceTypeController
            (
        IRepository<Tbl_ServicesTypes> repositoryTbl_ServicesTypes,
        IWorkContext workContext,
        IDbContext dbContext,
        IPermissionService permissionService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService
            )
        {
            _repositoryTbl_ServicesTypes = repositoryTbl_ServicesTypes;
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

            var Model = new Search_ServiceTypes_Model();
            return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/ListServiceTypes.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult ServiceTypesList(DataSourceRequest command, Search_ServiceTypes_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();


            List<Tbl_ServicesTypes> Services = new List<Tbl_ServicesTypes>();
            var gridModel = new DataSourceResult();
            var Final_Services = (dynamic)null;
            try
            {
                Services = _repositoryTbl_ServicesTypes.Table.ToList();
                if (Services.Count > 0)
                {
                    if (model.Search_ServiceTypes_ActiveSearch == true)
                    {
                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_ServiceTypes_Name))
                        {
                            Services = Services.Where(p => p.Name.Contains(model.Search_ServiceTypes_Name)).ToList();
                        }

                        Services = Services.Where(p => p.IsActive == model.Search_ServiceTypes_IsActive).ToList();

                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Services = (from q in Services
                                  select new Grid_ServiceTypes
                                  {
                                      Id = q.Id,
                                      Grid_Service_Name = q.Name,
                                      Grid_Service_IsActive = q.IsActive,
                                      Grid_Service_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                      Grid_Service_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                      Grid_Service_DateInsert = q.DateInsert,
                                      Grid_Service_DateUpdate = q.DateUpdate,
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
                    Data = Final_Services,
                    Total = Services.Count,
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
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_ServicesTypes Service = _repositoryTbl_ServicesTypes.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Service == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Service.IsActive = false;
                    _repositoryTbl_ServicesTypes.Update(Service);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableServiceProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"), Service.Name);
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
                        Tbl_ServicesTypes Service = _repositoryTbl_ServicesTypes.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Service != null)
                        {

                            Service.IsActive = false;
                            Service.DateUpdate = DateTime.Now;
                            Service.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_ServicesTypes.Update(Service);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableServiceProvider", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
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
                Tbl_ServicesTypes Service = _repositoryTbl_ServicesTypes.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Service == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Service.IsActive = true;
                    _repositoryTbl_ServicesTypes.Update(Service);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveServiceType", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"), Service.Name);
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
                        Tbl_ServicesTypes Service = _repositoryTbl_ServicesTypes.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Service != null)
                        {

                            Service.IsActive = true;
                            Service.DateUpdate = DateTime.Now;
                            Service.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_ServicesTypes.Update(Service);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveServiceType", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
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

            return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml");
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_ServicesTypes model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_ServicesTypes duplicate = _repositoryTbl_ServicesTypes.Table.Where(p => p.Name == model.Name).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml");

            }
            #endregion
            Tbl_ServicesTypes ServiceType = new Tbl_ServicesTypes();
            ServiceType.Name = model.Name;
            ServiceType.IsActive = true;
            ServiceType.DateInsert = DateTime.Now;
            ServiceType.IdUserInsert = _workContext.CurrentCustomer.Id;
            //ServiceType.Id = 1;
            _repositoryTbl_ServicesTypes.Insert(ServiceType);

            //activity log
            _customerActivityService.InsertActivity("AddNewServiceType", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), ServiceType.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = ServiceType.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var ServiceType = _repositoryTbl_ServicesTypes.GetById(id);
            if (ServiceType == null || ServiceType.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = ServiceType;
            return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_ServicesTypes model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var ServiceType = _repositoryTbl_ServicesTypes.GetById(model.Id);
            if (ServiceType == null || ServiceType.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            ServiceType.Name = model.Name;
            ServiceType.DateUpdate = DateTime.Now;
            ServiceType.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_ServicesTypes.Update(ServiceType);
            //activity log
            _customerActivityService.InsertActivity("EditServicesType", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"), ServiceType.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = ServiceType.Id });
            }
            return RedirectToAction("List");




            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);

        }




        #endregion




    }
}
