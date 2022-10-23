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


    public class ManageFAQController : BaseAdminController
    {
        private readonly IRepository<Tbl_FAQCategory> _repositoryTbl_FAQCategory;
        private readonly IRepository<Tbl_FAQ> _repositoryTbl_FAQ;

        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;

        public ManageFAQController
            (
            IRepository<Tbl_FAQ> repositoryTbl_FAQ,
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
            _repositoryTbl_FAQ = repositoryTbl_FAQ;
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
            var Model = new Search_FAQ_Model();

            var Deps = _repositoryTbl_FAQCategory.Table.Where(p => p.IsActive == true).ToList();
            if (Deps.Count > 0)
            {
                Model.ListFAQCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    Model.ListFAQCategory.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                Model.ListFAQCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }

            return View("/Plugins/Misc.Ticket/Views/FAQ/ListFAQ.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult FAQList(DataSourceRequest command, Search_FAQ_Model model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_FAQ> pros = new List<Tbl_FAQ>();
            var gridModel = new DataSourceResult();
            var Final_pro = (dynamic)null;
            try
            {
                pros = _repositoryTbl_FAQ.Table.ToList();
                if (pros.Count > 0)
                {
                    if (model.Search_FAQ_ActiveSearch == true)
                    {

                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_FAQ_Question))
                        {
                            pros = pros.Where(p => p.Question.Contains(model.Search_FAQ_Question)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.Search_FAQ_Answer))
                        {
                            pros = pros.Where(p => p.Answer.Contains(model.Search_FAQ_Answer)).ToList();
                        }
                        if (model.SearchFAQCategoryId > 0)
                        {
                            pros = pros.Where(p => p.IdCategory == model.SearchFAQCategoryId).ToList();
                        }
                        pros = pros.Where(p => p.IsActive == model.Search_FAQ_IsActive).ToList();

                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_pro = (from q in pros
                             select new Grid_FAQ
                             {
                                 Id = q.Id,
                                 Grid_FAQ_Question = q.Question,
                                 Grid_FAQ_Answer = q.Answer,
                                 Grid_FAQ_CategoryName = _repositoryTbl_FAQCategory.GetById(q.IdCategory).Name,
                                 Grid_FAQ_IsActive = q.IsActive,
                                 Grid_FAQ_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                 Grid_FAQ_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                 Grid_FAQ_DateInsert = q.DateInsert,
                                 Grid_FAQ_DateUpdate = q.DateUpdate,
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
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_FAQ Dep = _repositoryTbl_FAQ.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_FAQ.Update(Dep);
                }
                //activity log
                _customerActivityService.InsertActivity("DisableFAQ", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"), Dep.Id.ToString());
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
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_FAQ pro = _repositoryTbl_FAQ.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pro != null)
                        {

                            pro.IsActive = false;
                            pro.DateUpdate = DateTime.Now;
                            pro.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_FAQ.Update(pro);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisableFAQ", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_FAQ Pro = _repositoryTbl_FAQ.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_FAQ.Update(Pro);
                }
                //activity log
                _customerActivityService.InsertActivity("ActiveFAQ", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), Pro.Id.ToString());
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
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_FAQ Priority = _repositoryTbl_FAQ.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Priority != null)
                        {

                            Priority.IsActive = true;
                            Priority.DateUpdate = DateTime.Now;
                            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_FAQ.Update(Priority);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActiveFAQ", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
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
            var model = new Tbl_FAQ();

            #region  get list category
            var Deps = _repositoryTbl_FAQCategory.Table.Where(p => p.IsActive == true).ToList();
            if (Deps.Count > 0)
            {
                model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    model.ListCategory.Add(new SelectListItem { Text = s.Name , Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            return View("/Plugins/Misc.Ticket/Views/FAQ/Create.cshtml",model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_FAQ model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            
            Tbl_FAQ Priority = new Tbl_FAQ();
            Priority.Question = model.Question;
            Priority.Answer = model.Answer;
            Priority.IdCategory = model.IdCategory;
            Priority.IsActive = true;
            Priority.DateInsert = DateTime.Now;
            Priority.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_FAQ.Insert(Priority);

            //activity log
            _customerActivityService.InsertActivity("AddNewFAQ", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), Priority.Id.ToString());

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

            var Priority = _repositoryTbl_FAQ.GetById(id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = Priority;

            #region  get list category
            var Deps = _repositoryTbl_FAQCategory.Table.Where(p => p.IsActive == true).ToList();
            if (Deps.Count > 0)
            {
                model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var s in Deps)
                {

                    model.ListCategory.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
                }
            }
            else
            {
                model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion

            return View("/Plugins/Misc.Ticket/Views/FAQ/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_FAQ model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Priority = _repositoryTbl_FAQ.GetById(model.Id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Priority.Question = model.Question;
            Priority.Answer = model.Answer;
            Priority.IdCategory = model.IdCategory;
            Priority.DateUpdate = DateTime.Now;
            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_FAQ.Update(Priority);
            //activity log
            _customerActivityService.InsertActivity("EditFAQ", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), Priority.Id.ToString());

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
