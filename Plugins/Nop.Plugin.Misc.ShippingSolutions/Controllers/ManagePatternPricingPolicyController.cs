using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Models;
using Nop.Plugin.Misc.ShippingSolutions.Models.Grid;
using Nop.Plugin.Misc.ShippingSolutions.Models.Search;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Helpers;
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
    public class ManagePatternPricingPolicyController : BaseAdminController
    {
        private readonly IRepository<Tbl_PatternPricingPolicy> _repositoryTbl_PatternPricingPolicy;


        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly ICategoryService _CategoryService;
        private readonly IStateProvinceService _StateProvinceService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreService _storeService;
        private readonly IPaterPricingPolicyService _paterPricingPolicyService;


        public ManagePatternPricingPolicyController
            (
            IRepository<Tbl_PatternPricingPolicy> repositoryTbl_PatternPricingPolicy,
        IWorkContext workContext,
            IDbContext dbContext,
            IPermissionService permissionService,
            ICategoryService CategoryService,
            IStaticCacheManager cacheManager,
            IStateProvinceService StateProvinceService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IStoreService storeService,
            IPaterPricingPolicyService paterPricingPolicyService
            )
        {
            _paterPricingPolicyService = paterPricingPolicyService;
            _repositoryTbl_PatternPricingPolicy = repositoryTbl_PatternPricingPolicy;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _CategoryService = CategoryService;
            _cacheManager = cacheManager;
            _StateProvinceService = StateProvinceService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _customerService = customerService;
            _storeService = storeService;
        }

        #region index & lists
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            //if (!_workContext.CurrentCustomer.IsAdmin())
            //    return AccessDeniedView();

            var Model = new Search_PatternPricingPolicy();

            var categories = SelectListHelper.GetCategoryList(_CategoryService, _cacheManager, true);
            if (categories.Count > 0)
            {
                Model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in categories)
                {

                    Model.ListCategory.Add(c);
                }
            }
            else
            {
                Model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }





            return View("/Plugins/Misc.ShippingSolutions/Views/PatternPricingPolicy/List.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult PatternCategoryList(DataSourceRequest command, Search_PatternPricingPolicy model)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_PatternPricingPolicy> Pattern = new List<Tbl_PatternPricingPolicy>();

            var gridModel = new DataSourceResult();
            var Final_Pattern = new List<Grid_PatternPricingPolicy>();
            try
            {
                Pattern = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.IdParent == 0).GroupBy(p => p.CategoryId).Select(g => g.OrderByDescending(c => c.Id).FirstOrDefault()).ToList();

                if (Pattern.Count > 0)
                {
                    if (model.ActiveSearch == true)
                    {
                        #region Search

                        Pattern = Pattern.Where(p => p.IsActive == model.SearchIsActive).ToList();

                        if (model.SearchCategoryId > 0)
                        {
                            Pattern = Pattern.Where(p => p.CategoryId == model.SearchCategoryId).ToList();
                        }

                        Pattern = Pattern.Where(p => p.MinCount == model.SearchMinCount).ToList();
                        Pattern = Pattern.Where(p => p.MaxCount == model.SearchMaxCount).ToList();

                        if (!string.IsNullOrEmpty(model.SearchName))
                        {
                            Pattern = Pattern.Where(p => p.Name.Contains(model.SearchName)).ToList();

                        }

                        #endregion
                    }
                }
                else
                {
                    ErrrorMassege = "No Exsit Data";
                }

                Final_Pattern = (from q in Pattern
                                 select new Grid_PatternPricingPolicy
                                 {
                                     Id = q.Id,
                                     Grid_PatternPricingPolicy_Category_Name = _CategoryService.GetCategoryById(q.CategoryId).Name,
                                     Grid_PatternPricingPolicy_IsActive = q.IsActive,
                                     Grid_PatternPricingPolicy_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                     Grid_PatternPricingPolicy_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                     Grid_PatternPricingPolicy_DateInsert = q.DateInsert,
                                     Grid_PatternPricingPolicy_DateUpdate = q.DateUpdate,
                                     Grid_PatternPricingPolicy_MaxCount = q.MaxCount,
                                     Grid_PatternPricingPolicy_MinCount = q.MinCount,
                                     Grid_PatternPricingPolicy_Name = q.Name
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
                    Data = Final_Pattern,
                    Total = Pattern.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }

        [HttpPost]
        public virtual IActionResult PricingPoliciesList(DataSourceRequest command, Tbl_PatternPricingPolicy _PP)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();

            var tbl_PatternPricingPolicies = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.CategoryId == _PP.CategoryId &&
                  p.IdParent == _PP.IdParent).ToList();



            var gridModel = new DataSourceResult();
            var Final_tbl_PricingPolicies = new List<Grid_PatternPricingPolicy1>();
            try
            {

                if (tbl_PatternPricingPolicies.Count > 0)
                {

                    Final_tbl_PricingPolicies = (from q in tbl_PatternPricingPolicies
                                                 select new Grid_PatternPricingPolicy1
                                                 {
                                                     Id = q.Id,
                                                     Grid_PricingPolicy_IsActive = q.IsActive,
                                                     Grid_PricingPolicy_MinCount = q.MinCount,
                                                     Grid_PricingPolicy_MaxCount = q.MaxCount,
                                                     Grid_PricingPolicy_MinWeight = q.MinWeight,
                                                     Grid_PricingPolicy_MaxWeight = q.MaxWeight,
                                                     Grid_PricingPolicy_Percent = q.Percent,
                                                     Grid_PricingPolicy_Mablagh = q.Mablagh,
                                                     Grid_PricingPolicy_Tashim = q.Tashim,
                                                     Grid_PricingPolicy_PercentTashim = q.PercentTashim,
                                                     Grid_PricingPolicy_UserInsert = _customerService.GetCustomerById(q.IdUserInsert).GetFullName(),
                                                     Grid_PricingPolicy_UserUpdate = q.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)q.IdUserUpdate).GetFullName(),
                                                     Grid_PricingPolicy_DateInsert = q.DateInsert,
                                                     Grid_PricingPolicy_DateUpdate = q.DateUpdate,
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
                    Data = Final_tbl_PricingPolicies,
                    Total = Final_tbl_PricingPolicies.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }
        #endregion
        #region Active & DeActive
        [HttpPost]
        public virtual IActionResult Disable(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_PatternPricingPolicy pattern_category = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.Id == id).FirstOrDefault();

                if (pattern_category == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    pattern_category.IsActive = false;
                    _repositoryTbl_PatternPricingPolicy.Update(pattern_category);
                }

                //activity log
                _customerActivityService.InsertActivity("DeletePatternPricingPolicyCategory", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));

                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
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
                        Tbl_PatternPricingPolicy pattern_category = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pattern_category != null)
                        {

                            pattern_category.IsActive = false;
                            pattern_category.DateUpdate = DateTime.Now;
                            pattern_category.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_PatternPricingPolicy.Update(pattern_category);
                        }
                    }
                }
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
                Tbl_PatternPricingPolicy pattern_category = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.Id == id).FirstOrDefault();

                if (pattern_category == null)
                {

                    //No product found with the specified id
                    return RedirectToAction("List");
                }

                //a vendor should have access only to his products
                //if (_workContext.CurrentVendor != null && Providre.VendorId != _workContext.CurrentVendor.Id)
                //    return RedirectToAction("List");

                else
                {
                    pattern_category.IsActive = true;
                    _repositoryTbl_PatternPricingPolicy.Update(pattern_category);
                }
                //activity log
                _customerActivityService.InsertActivity("ActivePatternPricingPolicyCategory", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
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
                        Tbl_PatternPricingPolicy pattern_category = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pattern_category != null)
                        {

                            pattern_category.IsActive = true;
                            pattern_category.DateUpdate = DateTime.Now;
                            pattern_category.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_PatternPricingPolicy.Update(pattern_category);
                        }
                    }
                }
                _customerActivityService.InsertActivity("ActivePatternPricingPolicyCategory", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));

            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }


        [HttpPost]
        public virtual IActionResult ActivePricingPolicy(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {


                Tbl_PatternPricingPolicy pp = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.Id == id).FirstOrDefault();

                if (pp == null)
                {
                    return Json(new { success = false, responseText = "رکوردی یافت نشد" });//, JsonRequestBehavior.AllowGet

                }
                else
                {
                    List<Tbl_PatternPricingPolicy> Checking_range = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.CategoryId == pp.CategoryId &&
                     p.IdParent == pp.IdParent).ToList();

                    if (Checking_range.Count > 0)
                    {

                        foreach (var item in Checking_range)
                        {
                            if (item.Id != pp.Id)
                            {
                                if (pp.MinCount >= item.MinCount && pp.MinCount <= item.MaxCount)
                                {
                                    return Json(new { success = false, responseText = "بازه های تعدادی همپوشانی دارند" });

                                }
                                if (pp.MinWeight >= item.MinWeight && pp.MinWeight <= item.MaxWeight)
                                {
                                    return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                                }
                                if (pp.MaxWeight >= item.MinWeight && pp.MaxWeight <= item.MaxWeight)
                                {
                                    return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                                }
                                if (pp.MaxCount >= item.MinCount && pp.MaxCount <= item.MaxCount)
                                {
                                    return Json(new { success = false, responseText = "بازه های تعدادی همپوشانی دارند" });

                                }
                            }
                        }
                    }

                    pp.IsActive = true;
                    pp.DateUpdate = DateTime.Now;
                    pp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_PatternPricingPolicy.Update(pp);
                }
                //activity log
                _customerActivityService.InsertActivity("ActivePatternPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                return Json(new { success = true, responseText = "فعال سازی با موفقیت  انجام گردید" });//, JsonRequestBehavior.AllowGet

            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = ex.ToString() });//, JsonRequestBehavior.AllowGet

            }

        }


        [HttpPost]
        public virtual IActionResult DisableSelectedPricingPolicy(ICollection<int> selectedIds)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();


            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_PatternPricingPolicy pp = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pp != null)
                        {

                            pp.IsActive = false;
                            pp.DateUpdate = DateTime.Now;
                            pp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_PatternPricingPolicy.Update(pp);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisablePatternPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
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

            var Model = new Tbl_PatternPricingPolicy();
            var categories = _paterPricingPolicyService.getActiveService();
            // var categories = SelectListHelper.GetCategoryList(_CategoryService, _cacheManager, true);
            if (categories.Count > 0)
            {
                Model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in categories)
                {

                    Model.ListCategory.Add(c);
                }
            }
            else
            {
                Model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }

            return View("/Plugins/Misc.ShippingSolutions/Views/PatternPricingPolicy/Create.cshtml", Model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_PatternPricingPolicy model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            //if (ModelState.IsValid)
            //{
            #region Check Duplicate
            Tbl_PatternPricingPolicy duplicate = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.CategoryId == model.CategoryId).FirstOrDefault();
            if (duplicate != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                return View("/Plugins/Misc.ShippingSolutions/Views/PatternPricingPolicy/Create.cshtml");

            }
            #endregion
            Tbl_PatternPricingPolicy NewCategory = new Tbl_PatternPricingPolicy();
            NewCategory.CategoryId = model.CategoryId;
            NewCategory.Name = model.Name;
            NewCategory.MinCount = model.MinCount;
            NewCategory.MaxCount = model.MaxCount;
            NewCategory.IsActive = true;
            NewCategory.IdParent = 0;
            NewCategory.DateInsert = DateTime.Now;
            NewCategory.IdUserInsert = _workContext.CurrentCustomer.Id;
            _repositoryTbl_PatternPricingPolicy.Insert(NewCategory);

            //activity log
            _customerActivityService.InsertActivity("AddNewPatternPricingPolicyCategory", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = NewCategory.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var pattern = _repositoryTbl_PatternPricingPolicy.GetById(id);
            if (pattern == null || pattern.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            pattern.IdParent = pattern.Id;
            var Model = new Tbl_PatternPricingPolicy();
            Model = pattern;
            //var categories = SelectListHelper.GetCategoryList(_CategoryService, _cacheManager, true);
            var categories = _paterPricingPolicyService.getActiveService();
            if (categories.Count > 0)
            {
                Model.ListCategory.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in categories)
                {

                    Model.ListCategory.Add(c);
                }
            }
            else
            {
                Model.ListCategory.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            return View("/Plugins/Misc.ShippingSolutions/Views/PatternPricingPolicy/Edit.cshtml", Model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_PatternPricingPolicy model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var patterncategoty = _repositoryTbl_PatternPricingPolicy.GetById(model.Id);
            if (patterncategoty == null || patterncategoty.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");

            //update
            if (patterncategoty.CategoryId != model.CategoryId)
            {
                //update all patterns
                List<Tbl_PatternPricingPolicy> All_patterns = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.CategoryId == patterncategoty.CategoryId && p.IdParent > 0).ToList();
                if (All_patterns.Count > 0)
                {
                    foreach (var item in All_patterns)
                    {
                        item.CategoryId = model.CategoryId;

                        item.DateUpdate = DateTime.Now;
                        item.IdUserUpdate = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_PatternPricingPolicy.Update(item);
                        //activity log
                        _customerActivityService.InsertActivity("EditAllSubParrentCategoryinPatternPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));

                    }

                }
                patterncategoty.CategoryId = model.CategoryId;
                patterncategoty.Name = model.Name;
                patterncategoty.MinCount = model.MinCount;
                patterncategoty.MaxCount = model.MaxCount;
                patterncategoty.DateUpdate = DateTime.Now;
                patterncategoty.IdUserUpdate = _workContext.CurrentCustomer.Id;
                _repositoryTbl_PatternPricingPolicy.Update(patterncategoty);
                //activity log
                _customerActivityService.InsertActivity("EditCategoryInPatternPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));

            }
            else
            {
                patterncategoty.DateUpdate = DateTime.Now;
                patterncategoty.Name = model.Name;
                patterncategoty.MinCount = model.MinCount;
                patterncategoty.MaxCount = model.MaxCount;
                patterncategoty.IdUserUpdate = _workContext.CurrentCustomer.Id;
                _repositoryTbl_PatternPricingPolicy.Update(patterncategoty);
                //activity log
                _customerActivityService.InsertActivity("EditPatternPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));

            }

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = patterncategoty.Id });
            }
            return RedirectToAction("List");


            //return View("/Plugins/Misc.ShippingSolutions/Views/ServiceTypes/Edit.cshtml", model);

        }

        [HttpPost]
        public virtual IActionResult AddPricingPolicy(vm_AddPaternPricingPolicy model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            try
            {
                var Checking_range = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.CategoryId == model.CategoryId &&
                   p.IdParent == model.IdParent).ToList();
                // Checking the range 
                if (Checking_range.Count > 0)
                {

                    foreach (var item in Checking_range)
                    {
                        if (item.Id != model.Id && item.IsActive)
                        {
                            //if (model.MinCount >= item.MinCount && model.MinCount <= item.MaxCount)
                            //{
                            //    return Json(new { success = false, responseText = "بازه های تعدادی همپوشانی دارند" });

                            //}
                            if (model.MinWeight >= item.MinWeight && model.MinWeight <= item.MaxWeight)
                            {
                                return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                            }
                            if (model.MaxWeight >= item.MinWeight && model.MaxWeight <= item.MaxWeight)
                            {
                                return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                            }
                            //if (model.MaxCount >= item.MinCount && model.MaxCount <= item.MaxCount)
                            //{
                            //    return Json(new { success = false, responseText = "بازه های تعدادی همپوشانی دارند" });

                            //}
                        }
                    }
                }

                if (model.Id == 0)
                {
                    //add

                    Tbl_PatternPricingPolicy temp = new Tbl_PatternPricingPolicy();
                    temp.CategoryId = model.CategoryId;
                    temp.DateInsert = DateTime.Now;
                    temp.IdParent = model.IdParent;
                    temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    temp.IsActive = true;
                    temp.Mablagh = model.Mablagh;
                    temp.MaxWeight = model.MaxWeight;
                    temp.MinWeight = model.MinWeight;
                    temp.Percent = model.Percent;
                    temp.Tashim = model.Tashim;
                    temp.PercentTashim = model.PercentTashim;
                    temp.MinCount = 0;
                    temp.MaxCount = 0;
                    _repositoryTbl_PatternPricingPolicy.Insert(temp);
                    //activity log
                    _customerActivityService.InsertActivity("AddNewPatternPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));
                    return Json(new { success = true, responseText = "عملیات اضافه کردن یک پیش نویس سیاست قیمت گذاری با موفقیت ثبت شد" });//, JsonRequestBehavior.AllowGet

                }
                else
                {
                    //update 
                    //Tbl_PricingPolicy temp = new Tbl_PricingPolicy();
                    //temp.Id = _PP.Id;
                    //temp.CountryId = _PP.CountryId;
                    //temp.DateUpdate = DateTime.Now;
                    //temp.Dealer_Customer_Id = _PP.Dealer_Customer_Id;
                    //temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    //temp.IsActive = true;
                    //temp.Mablagh = _PP.Mablagh;
                    //temp.MaxWeight = _PP.MaxWeight;
                    //temp.MinWeight = _PP.MinWeight;
                    //temp.Percent = _PP.Percent;
                    //temp.ProviderId = _PP.ProviderId;
                    //temp.Tashim = _PP.Tashim;
                    //temp.TypeUser = _PP.TypeUser;
                    //_repositoryTbl_PricingPolicy.Update(temp);
                    ////activity log
                    //_customerActivityService.InsertActivity("AddNewPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));
                    return Json(new { success = true, responseText = "عملیات ویرایش سیاست قیمت گذاری با موفقیت ثبت شد" });//, JsonRequestBehavior.AllowGet

                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() });

            }


        }


        #endregion
    }
}
