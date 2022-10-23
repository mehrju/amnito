using Microsoft.AspNetCore.Mvc;
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
    public class ManagePatternAnswerDamagesController : BaseAdminController
    {
        private readonly IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;
        public ManagePatternAnswerDamagesController
            (
        IRepository<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD,
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
            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD = repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD;
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

            var Model = new Search_PatternAnswerTicket();
            return View("/Plugins/Misc.Ticket/Views/PatternAnswerDamages/List.cshtml");
        }
        [HttpPost]
        public virtual IActionResult PatternAnswerList(DataSourceRequest command, Search_PatternAnswerTicket model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_PatternAnswer_Ticket_Damages_RequestCOD> pros = new List<Tbl_PatternAnswer_Ticket_Damages_RequestCOD>();
            var gridModel = new DataSourceResult();
            var Final_pro = (dynamic)null;
            try
            {
                pros = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p=>p.Type==2).ToList();
                if (pros.Count > 0)
                {
                    if (model.Search_PatternAnswerTicket_ActiveSearch == true)
                    {

                        #region Search
                        if (!string.IsNullOrEmpty(model.Search_PatternAnswerTicket_Title))
                        {
                            pros = pros.Where(p => p.TitlePatternAnswer.Contains(model.Search_PatternAnswerTicket_Title)).ToList();
                        }
                        if (!string.IsNullOrEmpty(model.Search_PatternAnswerTicket_Descriptipn))
                        {
                            pros = pros.Where(p => p.DescriptipnPatternAnswer.Contains(model.Search_PatternAnswerTicket_Descriptipn)).ToList();
                        }
                        pros = pros.Where(p => p.IsActive == model.Search_PatternAnswerTicket_IsActive).ToList();

                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_pro = (from q in pros
                             select new Grid_PatternAnswerTicket
                             {
                                 Id = q.Id,
                                 Grid_PatternAnswerTicket_Title = q.TitlePatternAnswer,
                                 Grid_PatternAnswerTicket_Description = q.DescriptipnPatternAnswer,

                                 Grid_PatternAnswerTicket_IsActive = q.IsActive,
                                 Grid_PatternAnswerTicket_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                 Grid_PatternAnswerTicket_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                 Grid_PatternAnswerTicket_DateInsert = q.DateInsert,
                                 Grid_PatternAnswerTicket_DateUpdate = q.DateUpdate,
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
                Tbl_PatternAnswer_Ticket_Damages_RequestCOD Dep = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Update(Dep);
                }
                //activity log
                _customerActivityService.InsertActivity("DisablePatternAnswerDamages", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"), Dep.TitlePatternAnswer);
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
                        Tbl_PatternAnswer_Ticket_Damages_RequestCOD pro = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pro != null)
                        {

                            pro.IsActive = false;
                            pro.DateUpdate = DateTime.Now;
                            pro.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Update(pro);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisablePatternAnswerDamages", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageDisable"));
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
                Tbl_PatternAnswer_Ticket_Damages_RequestCOD Pro = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.Id == id).FirstOrDefault();

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
                    Pro.DateUpdate = DateTime.Now;
                    Pro.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Update(Pro);
                }
                //activity log
                _customerActivityService.InsertActivity("ActivePatternAnswerDamages", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"), Pro.TitlePatternAnswer);
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
                        Tbl_PatternAnswer_Ticket_Damages_RequestCOD Priority = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (Priority != null)
                        {

                            Priority.IsActive = true;
                            Priority.DateUpdate = DateTime.Now;
                            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Update(Priority);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActivePatternAnswerDamages", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageActive"));
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

            return View("/Plugins/Misc.Ticket/Views/PatternAnswerDamages/Create.cshtml");
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_PatternAnswer_Ticket_Damages_RequestCOD model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_PatternAnswer_Ticket_Damages_RequestCOD duplicate = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Table.Where(p => p.TitlePatternAnswer == model.TitlePatternAnswer && p.IsActive && p.Type==2).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.Ticket.Duplicate"));
                return View("/Plugins/Misc.Ticket/Views/PatternAnswerDamages/Create.cshtml");

            }
            #endregion
            Tbl_PatternAnswer_Ticket_Damages_RequestCOD Priority = new Tbl_PatternAnswer_Ticket_Damages_RequestCOD();
            Priority.TitlePatternAnswer = model.TitlePatternAnswer;
            Priority.DescriptipnPatternAnswer = model.DescriptipnPatternAnswer;
            Priority.IsActive = true;
            Priority.Type = 2;
            Priority.DateInsert = DateTime.Now;
            Priority.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Insert(Priority);

            //activity log
            _customerActivityService.InsertActivity("AddNewPatternAnswerTicket", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.Create"), Priority.TitlePatternAnswer);
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

            var Priority = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.GetById(id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = Priority;
            return View("/Plugins/Misc.Ticket/Views/PatternAnswerDamages/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_PatternAnswer_Ticket_Damages_RequestCOD model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Priority = _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.GetById(model.Id);
            if (Priority == null || Priority.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            Priority.TitlePatternAnswer = model.TitlePatternAnswer;
            Priority.DescriptipnPatternAnswer = model.DescriptipnPatternAnswer;

            Priority.DateUpdate = DateTime.Now;
            Priority.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_PatternAnswer_Ticket_Damages_RequestCOD.Update(Priority);
            //activity log
            _customerActivityService.InsertActivity("EditPatternAnswerDamages", _localizationService.GetResource("Nop.Plugin.Misc.Ticket.MessageUpdate"), Priority.TitlePatternAnswer);

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
