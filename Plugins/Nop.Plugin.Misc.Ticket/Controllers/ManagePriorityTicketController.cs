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
    public class ManagePriorityTicketController : BaseAdminController
    {
        private readonly IRepository<Tbl_Ticket_Priority> _repositoryTbl_Ticket_Priority;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;

        public ManagePriorityTicketController
            (
            IRepository<Tbl_Ticket_Priority> repositoryTbl_Ticket_Priority,
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
            _repositoryTbl_Ticket_Priority = repositoryTbl_Ticket_Priority;
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

            var Model = new Search_PriorityTicket_Model();
         
            return View("/Plugins/Misc.Ticket/Views/PriorityTicket/ListPriorityTicket.cshtml");
        }
        [HttpPost]
        public virtual IActionResult PriorityList(DataSourceRequest command, Search_PriorityTicket_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedKendoGridJson();


            List<Tbl_Ticket_Priority> pros = new List<Tbl_Ticket_Priority>();
            var gridModel = new DataSourceResult();
            var Final_pro = (dynamic)null;
            try
            {
                pros = _repositoryTbl_Ticket_Priority.Table.ToList();
                if (pros.Count > 0)
                {
                    if (model.Search_PriorityTicket_ActiveSearch == true)
                    {
                        
                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_PriorityTicket_Name))
                        {
                            pros = pros.Where(p => p.Name.Contains(model.Search_PriorityTicket_Name)).ToList();
                        }

                        pros = pros.Where(p => p.IsActive == model.Search_PriorityTicket_IsActive).ToList();
                       
                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_pro = (from q in pros
                              select new Grid_ProirityTicket
                                  {
                                      Id = q.Id,
                                      Grid_ProirityTicket_Name = q.Name,
                                      Grid_ProirityTicket_IsActive = q.IsActive,
                                      Grid_ProirityTicket_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                      Grid_ProirityTicket_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                      Grid_ProirityTicket_DateInsert = q.DateInsert,
                                      Grid_ProirityTicket_DateUpdate = q.DateUpdate,
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
                Tbl_Ticket_Priority Dep = _repositoryTbl_Ticket_Priority.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_Ticket_Priority.Update(Dep);
                }
                //activity log
                _customerActivityService.InsertActivity("DisablePriorityTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"), Dep.Name);
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
                        Tbl_Ticket_Priority pro = _repositoryTbl_Ticket_Priority.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pro != null)
                        {

                            pro.IsActive = false;
                            pro.DateUpdate = DateTime.Now;
                            pro.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Ticket_Priority.Update(pro);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisablePriorityTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                Tbl_Ticket_Priority Pro = _repositoryTbl_Ticket_Priority.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_Ticket_Priority.Update(Pro);
                }
                //activity log
                _customerActivityService.InsertActivity("ActivePriorityTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), Pro.Name);
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
                        Tbl_Ticket_Priority Priority = _repositoryTbl_Ticket_Priority.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Priority != null)
                        {

                            Priority.IsActive = true;
                            Priority.DateUpdate = DateTime.Now;
                            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Ticket_Priority.Update(Priority);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActivePriorityTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
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
           
            return View("/Plugins/Misc.Ticket/Views/PriorityTicket/Create.cshtml");
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_Ticket_Department model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_Ticket_Priority duplicate = _repositoryTbl_Ticket_Priority.Table.Where(p => p.Name == model.Name).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Duplicate"));
                return View("/Plugins/Misc.Ticket/Views/PriorityTicket/Create.cshtml");

            }
            #endregion
            Tbl_Ticket_Priority Priority = new Tbl_Ticket_Priority();
            Priority.Name = model.Name;
            Priority.IsActive = true;
            Priority.DateInsert = DateTime.Now;
            Priority.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Ticket_Priority.Insert(Priority);

            //activity log
            _customerActivityService.InsertActivity("AddNewPriorityTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), Priority.Name);

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

            var Priority = _repositoryTbl_Ticket_Priority.GetById(id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = Priority;
            return View("/Plugins/Misc.Ticket/Views/PriorityTicket/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Ticket_Department model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Priority = _repositoryTbl_Ticket_Priority.GetById(model.Id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Priority.Name = model.Name;
            Priority.DateUpdate = DateTime.Now;
            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Ticket_Priority.Update(Priority);
            //activity log
            _customerActivityService.InsertActivity("EditPriorityTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), Priority.Name);

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
