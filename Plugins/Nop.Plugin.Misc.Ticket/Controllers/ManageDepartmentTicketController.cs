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
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Ticket.Controllers
{
    public class ManageDepartmentTicketController : BaseAdminController
    {
        private readonly IRepository<Tbl_Ticket_Department> _repositoryTbl_Ticket_Department;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;

        public ManageDepartmentTicketController
            (
            IRepository<Tbl_Ticket_Department> repositoryTbl_Ticket_Department,
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

            var Model = new Search_DepartmentTicket_Model();
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
            return View("/Plugins/Misc.Ticket/Views/DepartmentTicket/ListDepartmentTicket.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult DepartmentList(DataSourceRequest command, Search_DepartmentTicket_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();


            List<Tbl_Ticket_Department> Departments = new List<Tbl_Ticket_Department>();
            var gridModel = new DataSourceResult();
            var Final_Deps = (dynamic)null;
            try
            {
                Departments = _repositoryTbl_Ticket_Department.Table.ToList();
                if (Departments.Count > 0)
                {
                    if (model.Search_DepartmentTicket_ActiveSearch == true)
                    {
                        
                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_DepartmentTicket_Name))
                        {
                            Departments = Departments.Where(p => p.Name.Contains(model.Search_DepartmentTicket_Name)).ToList();
                        }

                        Departments = Departments.Where(p => p.IsActive == model.Search_DepartmentTicket_IsActive).ToList();
                        if (model.Search_DepartmentTicket_StoreId> 0)
                        {
                            Departments = Departments.Where(p=>p.StoreId==(model.Search_DepartmentTicket_StoreId)).ToList();

                        }
                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Deps = (from q in Departments
                              select new Grid_DepartmentTicket
                                  {
                                      Id = q.Id,
                                      Grid_DepartmentTicket_Name = q.Name,
                                      Grid_DepartmentTicket_IsActive = q.IsActive,
                                      Grid_DepartmentTicket_StoreName= q.StoreId==0? "همه فروشگاه ها": _storeService.GetStoreById(q.StoreId).Name,
                                      Grid_DepartmentTicket_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                      Grid_DepartmentTicket_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                      Grid_DepartmentTicket_DateInsert = q.DateInsert,
                                      Grid_DepartmentTicket_DateUpdate = q.DateUpdate,
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
                    Total = Departments.Count,
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
                Tbl_Ticket_Department Dep = _repositoryTbl_Ticket_Department.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_Ticket_Department.Update(Dep);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"), Dep.Name);
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
                        Tbl_Ticket_Department Service = _repositoryTbl_Ticket_Department.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Service != null)
                        {

                            Service.IsActive = false;
                            Service.DateUpdate = DateTime.Now;
                            Service.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Ticket_Department.Update(Service);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                Tbl_Ticket_Department Dep = _repositoryTbl_Ticket_Department.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    Dep.IsActive = true;
                    _repositoryTbl_Ticket_Department.Update(Dep);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), Dep.Name);
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
                        Tbl_Ticket_Department Service = _repositoryTbl_Ticket_Department.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Service != null)
                        {

                            Service.IsActive = true;
                            Service.DateUpdate = DateTime.Now;
                            Service.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Ticket_Department.Update(Service);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
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
            var model = new Tbl_Ticket_Department();
            var Stores = _storeService.GetAllStores(true).ToList();
            if (Stores.Count > 0)
            {
                model.ListStores.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Stores)
                {

                    model.ListStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListStores.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            return View("/Plugins/Misc.Ticket/Views/DepartmentTicket/Create.cshtml",model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_Ticket_Department model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_Ticket_Department duplicate = _repositoryTbl_Ticket_Department.Table.Where(p => p.Name == model.Name&& p.StoreId==model.StoreId).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Duplicate"));
                return View("/Plugins/Misc.Ticket/Views/DepartmentTicket/Create.cshtml");

            }
            #endregion
            Tbl_Ticket_Department Department = new Tbl_Ticket_Department();
            Department.Name = model.Name;
            Department.StoreId = model.StoreId;
            Department.IsActive = true;
            Department.DateInsert = DateTime.Now;
            Department.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Ticket_Department.Insert(Department);

            //activity log
            _customerActivityService.InsertActivity("AddNewDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), Department.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Department.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Department = _repositoryTbl_Ticket_Department.GetById(id);
            if (Department == null || Department.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            var model = Department;
            var Stores = _storeService.GetAllStores(true).ToList();
            if (Stores.Count > 0)
            {
                model.ListStores.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Stores)
                {

                    model.ListStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListStores.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            return View("/Plugins/Misc.Ticket/Views/DepartmentTicket/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Ticket_Department model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Department = _repositoryTbl_Ticket_Department.GetById(model.Id);
            if (Department == null || Department.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Department.Name = model.Name;
            Department.StoreId = model.StoreId;
            Department.DateUpdate = DateTime.Now;
            Department.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Ticket_Department.Update(Department);
            //activity log
            _customerActivityService.InsertActivity("EditDepartmentTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), Department.Name);

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Department.Id });
            }
            return RedirectToAction("List");




            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);

        }




        #endregion

    }
}
