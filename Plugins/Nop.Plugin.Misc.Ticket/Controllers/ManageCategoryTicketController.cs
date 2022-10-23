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
    public class ManageCategoryTicketController : BaseAdminController
    {

        //

        private readonly IRepository<Tbl_Ticket_Department> _repositoryTbl_Ticket_Department;
        private readonly IRepository<Tbl_CategoryTicket>    _repositoryTbl_CategoryTicket;

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;

        public ManageCategoryTicketController
            (
        IRepository<Tbl_Ticket_Department> repositoryTbl_Ticket_Department,
        IRepository<Tbl_CategoryTicket> repositoryTbl_CategoryTicket,
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
            _repositoryTbl_Ticket_Department = repositoryTbl_Ticket_Department;
            _repositoryTbl_CategoryTicket = repositoryTbl_CategoryTicket;
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

            var Model = new Search_CategoryTicket();
            var Deps = _repositoryTbl_Ticket_Department.Table.Where(p=>p.IsActive).ToList();
            if (Deps.Count > 0)
            {
                Model.ListDepartment.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    Model.ListDepartment.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                Model.ListDepartment.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            return View("/Plugins/Misc.Ticket/Views/CategoryTicket/List.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult CategoryList(DataSourceRequest command, Search_CategoryTicket model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();


            List<Tbl_CategoryTicket> Category = new List<Tbl_CategoryTicket>();
            var gridModel = new DataSourceResult();
            var Final_Deps = (dynamic)null;
            try
            {
                Category = _repositoryTbl_CategoryTicket.Table.ToList();
                if (Category.Count > 0)
                {
                    if (model.Search_CategoryTicket_ActiveSearch == true)
                    {

                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_CategoryTicket_NameCategory))
                        {
                            Category = Category.Where(p => p.NameCategoryTicket.Contains(model.Search_CategoryTicket_NameCategory)).ToList();
                        }

                        Category = Category.Where(p => p.IsActive == model.Search_CategoryTicket_IsActive).ToList();
                        if (model.Search_CategoryTicket_DepartmentId > 0)
                        {
                            Category = Category.Where(p => p.DepartmentId == (model.Search_CategoryTicket_DepartmentId)).ToList();

                        }
                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Deps = (from q in Category
                              select new Grid_CategoryTicket
                              {
                                  Id = q.Id,
                                  Grid_CategoryTicket_CategoryName = q.NameCategoryTicket,
                                  Grid_CategoryTicket_IsActive = q.IsActive,
                                  Grid_CategoryTicket_DepartmentName =_repositoryTbl_Ticket_Department.GetById(q.DepartmentId).Name,
                                  Grid_CategoryTicket_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                  Grid_CategoryTicket_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                  Grid_CategoryTicket_DateInsert = q.DateInsert,
                                  Grid_CategoryTicket_DateUpdate = q.DateUpdate,
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
                    Data = Final_Deps,
                    Total = Category.Count,
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
                Tbl_CategoryTicket Cat = _repositoryTbl_CategoryTicket.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Cat == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Cat.IsActive = false;
                    Cat.DateUpdate = DateTime.Now;
                    Cat.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CategoryTicket.Update(Cat);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableCategoryTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"), Cat.NameCategoryTicket);
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
                        Tbl_CategoryTicket Cat = _repositoryTbl_CategoryTicket.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Cat != null)
                        {

                            Cat.IsActive = false;
                            Cat.DateUpdate = DateTime.Now;
                            Cat.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_CategoryTicket.Update(Cat);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableCategoryTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                Tbl_CategoryTicket Cat = _repositoryTbl_CategoryTicket.Table.Where(p => p.Id == id).FirstOrDefault();

                if (Cat == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    Cat.IsActive = true;
                    Cat.DateUpdate = DateTime.Now;
                    Cat.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_CategoryTicket.Update(Cat);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveCategoryTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), Cat.NameCategoryTicket);
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
                        Tbl_CategoryTicket Cat = _repositoryTbl_CategoryTicket.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Cat != null)
                        {

                            Cat.IsActive = true;
                            Cat.DateUpdate = DateTime.Now;
                            Cat.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_CategoryTicket.Update(Cat);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveCategoryTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
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
            var model = new Tbl_CategoryTicket();
            var Deps = _repositoryTbl_Ticket_Department.Table.Where(p => p.IsActive).ToList();
            if (Deps.Count > 0)
            {
                model.ListDepartments.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    model.ListDepartments.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListDepartments.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            return View("/Plugins/Misc.Ticket/Views/CategoryTicket/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_CategoryTicket model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_CategoryTicket duplicate = _repositoryTbl_CategoryTicket.Table.Where(p => p.NameCategoryTicket == model.NameCategoryTicket && p.DepartmentId == model.DepartmentId).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Duplicate"));
                return View("/Plugins/Misc.Ticket/Views/CategoryTicket/Create.cshtml");

            }
            #endregion
            Tbl_CategoryTicket Cat = new Tbl_CategoryTicket();
            Cat.NameCategoryTicket = model.NameCategoryTicket;
            Cat.DepartmentId = model.DepartmentId;
            Cat.IsActive = true;
            Cat.DateInsert = DateTime.Now;
            Cat.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_CategoryTicket.Insert(Cat);

            //activity log
            _customerActivityService.InsertActivity("AddNewCategoryTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), Cat.NameCategoryTicket);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Cat.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Cat = _repositoryTbl_CategoryTicket.GetById(id);
            if (Cat == null || Cat.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = Cat;
            var Deps = _repositoryTbl_Ticket_Department.Table.Where(p => p.IsActive).ToList();
            if (Deps.Count > 0)
            {
                model.ListDepartments.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    model.ListDepartments.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListDepartments.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            return View("/Plugins/Misc.Ticket/Views/CategoryTicket/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_CategoryTicket model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Cat = _repositoryTbl_CategoryTicket.GetById(model.Id);
            if (Cat == null || Cat.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Cat.NameCategoryTicket = model.NameCategoryTicket;
            Cat.DepartmentId = model.DepartmentId;
            Cat.DateUpdate = DateTime.Now;
            Cat.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_CategoryTicket.Update(Cat);
            //activity log
            _customerActivityService.InsertActivity("EditCategoryTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), Cat.NameCategoryTicket);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Cat.Id });
            }
            return RedirectToAction("List");
            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);

        }




        #endregion

    }
}
