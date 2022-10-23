using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Misc.Ticket.Models.Grid;
using Nop.Plugin.Misc.Ticket.Models.Search;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    public class ManageFAQCategoryController : BaseAdminController
    {
        private readonly IRepository<Tbl_FAQCategory> _repositoryTbl_FAQCategory;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;

        public ManageFAQCategoryController
            (
            IRepository<Tbl_FAQCategory> repositoryTbl_FAQCategory,
               IWorkContext workContext,
        IDbContext dbContext,
        IPermissionService permissionService,
        IStaticCacheManager cacheManager,
        ILocalizationService localizationService,
        ICustomerActivityService customerActivityService,
        ICustomerService customerService,
        IStoreService storeService
            )
        {
            _repositoryTbl_FAQCategory = repositoryTbl_FAQCategory;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerService = customerService;
            _storeService = storeService;
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

         
            return View("/Plugins/Misc.Ticket/Views/FAQCategory/ListFAQCategory.cshtml");
        }
        [HttpPost]
        public virtual IActionResult FAQCategoryList(DataSourceRequest command, Search_FAQCategory_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();


            List<Tbl_FAQCategory> pros = new List<Tbl_FAQCategory>();
            var gridModel = new DataSourceResult();
            var Final_pro = (dynamic)null;
            try
            {
                pros = _repositoryTbl_FAQCategory.Table.ToList();
                if (pros.Count > 0)
                {
                    if (model.Search_FAQCategory_ActiveSearch == true)
                    {
                        
                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_FAQCategory_Name))
                        {
                            pros = pros.Where(p => p.Name.Contains(model.Search_FAQCategory_Name)).ToList();
                        }

                        pros = pros.Where(p => p.IsActive == model.Search_FAQCategory_IsActive).ToList();
                       
                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_pro = (from q in pros
                              select new Grid_FAQCategory
                                  {
                                      Id = q.Id,
                                      Grid_FAQCategory_Name = q.Name,
                                      Grid_FAQCategory_IsActive = q.IsActive,
                                      Grid_FAQCategory_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                      Grid_FAQCategory_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                      Grid_FAQCategory_DateInsert = q.DateInsert,
                                      Grid_FAQCategory_DateUpdate = q.DateUpdate,
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
                    Data = Final_pro,
                    Total = pros.Count,
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
                Tbl_FAQCategory Dep = _repositoryTbl_FAQCategory.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Dep == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Dep.IsActive = false;
                    Dep.DateUpdate = DateTime.Now;
                    Dep.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_FAQCategory.Update(Dep);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableFAQCategory", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"), Dep.Name);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                        Tbl_FAQCategory pro = _repositoryTbl_FAQCategory.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pro != null)
                        {

                            pro.IsActive = false;
                            pro.DateUpdate = DateTime.Now;
                            pro.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_FAQCategory.Update(pro);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableFAQCategory", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                Tbl_FAQCategory Pro = _repositoryTbl_FAQCategory.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Pro == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Pro.IsActive = true;
                    _repositoryTbl_FAQCategory.Update(Pro);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveFAQCategory", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), Pro.Name);
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
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
                        Tbl_FAQCategory Priority = _repositoryTbl_FAQCategory.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Priority != null)
                        {

                            Priority.IsActive = true;
                            Priority.DateUpdate = DateTime.Now;
                            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_FAQCategory.Update(Priority);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveFAQCategory", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));

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
           
            return View("/Plugins/Misc.Ticket/Views/FAQCategory/Create.cshtml");
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_FAQCategory model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_FAQCategory duplicate = _repositoryTbl_FAQCategory.Table.Where(p => p.Name == model.Name).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Duplicate"));
                return View("/Plugins/Misc.Ticket/Views/FAQCategory/Create.cshtml");

            }
            #endregion
            Tbl_FAQCategory Priority = new Tbl_FAQCategory();
            Priority.Name = model.Name;
            Priority.IsActive = true;
            Priority.DateInsert = DateTime.Now;
            Priority.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_FAQCategory.Insert(Priority);

            //activity log
            _customerActivityService.InsertActivity("AddNewFAQCategory", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), Priority.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Priority.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Priority = _repositoryTbl_FAQCategory.GetById(id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = Priority;
            return View("/Plugins/Misc.Ticket/Views/FAQCategory/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_FAQCategory model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Priority = _repositoryTbl_FAQCategory.GetById(model.Id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Priority.Name = model.Name;
            Priority.DateUpdate = DateTime.Now;
            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_FAQCategory.Update(Priority);
            //activity log
            _customerActivityService.InsertActivity("EditFAQCategory", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), Priority.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Priority.Id });
            }
            return RedirectToAction("List");




            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);

        }




        #endregion

    }
}
