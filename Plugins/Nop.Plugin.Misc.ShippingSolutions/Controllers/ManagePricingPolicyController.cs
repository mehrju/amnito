using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Models;
using Nop.Plugin.Misc.ShippingSolutions.Models.Grid;
using Nop.Services.Catalog;
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

namespace Nop.Plugin.Misc.ShippingSolutions.Controllers
{
    [AdminAntiForgery(true)]
    public class ManagePricingPolicyController : BaseAdminController
    {
        private readonly IRepository<Tbl_ServicesProviders> _repositoryTbl_ServicesProviders;
        private readonly IRepository<Tbl_Dealer> _repositoryTbl_Dealer;
        private readonly IRepository<Tbl_PricingPolicy> _repositoryTbl_PricingPolicy;
        private readonly IRepository<Tbl_PatternPricingPolicy> _repositoryTbl_PatternPricingPolicy;
        private readonly ICategoryService _CategoryService;

        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;

        public ManagePricingPolicyController
            (ICategoryService CategoryService,
            IRepository<Tbl_PatternPricingPolicy> repositoryTbl_PatternPricingPolicy,
            IRepository<Tbl_ServicesProviders> repositoryTbl_ServicesProviders,
        IRepository<Tbl_Dealer> repositoryTbl_Dealer,
        IRepository<Tbl_PricingPolicy> repositoryTbl_PricingPolicy,
              IWorkContext workContext,
              IDbContext dbContext,
              IPermissionService permissionService,
              ICustomerService customerService,
              IStaticCacheManager cacheManager,
              ILocalizationService localizationService,
              ICustomerActivityService customerActivityService
            )
        {
            _CategoryService = CategoryService;
            _repositoryTbl_PatternPricingPolicy = repositoryTbl_PatternPricingPolicy;
            _repositoryTbl_ServicesProviders = repositoryTbl_ServicesProviders;
            _repositoryTbl_Dealer = repositoryTbl_Dealer;
            _repositoryTbl_PricingPolicy = repositoryTbl_PricingPolicy;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
        }
        public virtual IActionResult EditPricingPolicy(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var PricingPolicy = _repositoryTbl_PricingPolicy.GetById(id);
            if (PricingPolicy == null || PricingPolicy.IsActive == false)
            {
                //No category found with the specified id
                return RedirectToAction("List");
            }
            else
            {
                Tbl_PricingPolicy model = new Tbl_PricingPolicy();
                model.TypeUser = 2;
                model.Dealer_Customer_Id = PricingPolicy.Dealer_Customer_Id;
                return View("/Plugins/Misc.ShippingSolutions/Views/PricingPolicy/Edit.cshtml", model);

            }
        }
        [HttpPost]
        public virtual IActionResult AddPricingPolicy(Tbl_PricingPolicy _PP)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            try
            {
                var Checking_range = _repositoryTbl_PricingPolicy.Table.Where(p => p.CountryId == _PP.CountryId &&
                   p.Dealer_Customer_Id == _PP.Dealer_Customer_Id && p.ProviderId == _PP.ProviderId).ToList();
                // Checking the range 
                if (Checking_range.Count > 0)
                {
                    //float tester = (_PP.MinWeight + _PP.MaxWeight) / 2;
                    foreach (var item in Checking_range)
                    {
                        if (item.Id != _PP.Id && item.IsActive)
                        {

                            if (_PP.MinWeight >= item.MinWeight && _PP.MinWeight <= item.MaxWeight)
                            {
                                return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                            }
                            if (_PP.MaxWeight >= item.MinWeight && _PP.MaxWeight <= item.MaxWeight)
                            {
                                return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                            }
                        }
                    }
                }

                if (_PP.Id == 0)
                {
                    //add

                    Tbl_PricingPolicy temp = new Tbl_PricingPolicy();
                    temp.CountryId = _PP.CountryId;
                    temp.DateInsert = DateTime.Now;
                    temp.Dealer_Customer_Id = _PP.Dealer_Customer_Id;
                    temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                    temp.IsActive = true;
                    temp.Mablagh = _PP.Mablagh;
                    temp.MaxWeight = _PP.MaxWeight;
                    temp.MinWeight = _PP.MinWeight;
                    temp.Percent = _PP.Percent;
                    temp.ProviderId = _PP.ProviderId;
                    temp.Tashim = _PP.Tashim;
                    temp.PercentTashim = _PP.PercentTashim;
                    temp.TypeUser = _PP.TypeUser;
                    _repositoryTbl_PricingPolicy.Insert(temp);
                    //activity log
                    _customerActivityService.InsertActivity("AddNewPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));
                    return Json(new { success = true, responseText = "عملیات اضافه کردن یک سیاست قیمت گذاری با موفقیت ثبت شد" });//, JsonRequestBehavior.AllowGet

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

        [HttpPost]
        public virtual IActionResult PricingPoliciesList(DataSourceRequest command, Tbl_PricingPolicy _PP)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();

            var tbl_PricingPolicies = _repositoryTbl_PricingPolicy.Table.Where(p => p.CountryId == _PP.CountryId &&
                  p.Dealer_Customer_Id == _PP.Dealer_Customer_Id && p.ProviderId == _PP.ProviderId).ToList();



            var gridModel = new DataSourceResult();
            var Final_tbl_PricingPolicies = new List<Grid_PricingPolicy>();
            try
            {

                if (tbl_PricingPolicies.Count > 0)
                {

                    Final_tbl_PricingPolicies = (from q in tbl_PricingPolicies
                                                 select new Grid_PricingPolicy
                                                 {
                                                     Id = q.Id,
                                                     Grid_PricingPolicy_IsActive = q.IsActive,
                                                     Grid_PricingPolicy_MinWeight = q.MinWeight,
                                                     Grid_PricingPolicy_MaxWeight = q.MaxWeight,
                                                     Grid_PricingPolicy_Percent = q.Percent,
                                                     Grid_PricingPolicy_Mablagh = q.Mablagh,
                                                     Grid_PricingPolicy_Tashim = q.Tashim,
                                                     Grid_PricingPolicy_PercentTashim=q.PercentTashim,
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
                    Total = tbl_PricingPolicies.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

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
                        Tbl_PricingPolicy pp = _repositoryTbl_PricingPolicy.Table.Where(p => p.Id == item).FirstOrDefault();
                        if (pp != null)
                        {

                            pp.IsActive = false;
                            pp.DateUpdate = DateTime.Now;
                            pp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_PricingPolicy.Update(pp);
                        }
                    }
                }
                _customerActivityService.InsertActivity("DisablePricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDisable"));
            }
            catch (Exception ex)
            {
                ErrorNotification(ex.ToString());

            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual IActionResult Active(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            try
            {
                Tbl_PricingPolicy pp = _repositoryTbl_PricingPolicy.Table.Where(p => p.Id == id).FirstOrDefault();
                if (pp == null)
                {
                    return Json(new { success = false, responseText = "رکوردی یافت نشد" });//, JsonRequestBehavior.AllowGet

                }
                else
                {
                    var Checking_range = _repositoryTbl_PricingPolicy.Table.Where(p => p.CountryId == pp.CountryId &&
                  p.Dealer_Customer_Id == pp.Dealer_Customer_Id && p.ProviderId == pp.ProviderId).ToList();
                    // Checking the range 
                    if (Checking_range.Count > 0)
                    {
                        foreach (var item in Checking_range)
                        {
                            if (item.Id != pp.Id)
                            {
                                if (pp.MinWeight >= item.MinWeight && pp.MinWeight <= item.MaxWeight)
                                {
                                    return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                                }
                                if (pp.MaxWeight >= item.MinWeight && pp.MaxWeight <= item.MaxWeight)
                                {
                                    return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                                }
                            }
                        }
                    }

                    pp.IsActive = true;
                    pp.DateUpdate = DateTime.Now;
                    pp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_PricingPolicy.Update(pp);
                }
                //activity log
                _customerActivityService.InsertActivity("ActivePricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageActive"));
                return Json(new { success = true, responseText = "فعال سازی با موفقیت  انجام گردید" });//, JsonRequestBehavior.AllowGet

            }
            catch (Exception ex)
            {

                return Json(new { success = false, responseText = ex.ToString() });//, JsonRequestBehavior.AllowGet

            }

        }




        [HttpGet]
        public IActionResult GetCategoryPattern()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                return AccessDeniedView();

            var listDeps = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.IsActive == true && p.IdParent==0).Select(p => new
            {
                Value = p.Id,
                Text = p.Name + " از تعداد:" + p.MinCount.ToString() + " تا تعداد:" + p.MaxCount.ToString()
            }).ToList();//" دسته بندی:" + _CategoryService.GetCategoryById(p.CategoryId).Name +
            return Json(listDeps);

        }


        [HttpPost]
        public virtual IActionResult PricingPoliciesListByIdCategory(int Id)
        {
           
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();

            var Grid = (dynamic)null;
            var tbl_PatternPricingPolicies = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.IdParent == Id &&
                  p.IsActive ==true).Select(p=>new { 
                      id=p.Id,
                      min=p.MinWeight,
                      max=p.MaxWeight,
                      per=p.Percent,
                      mab=p.Mablagh,
                      tas=p.Tashim,
                      taspercent=p.PercentTashim
                  }).ToList();
            if (tbl_PatternPricingPolicies != null)
            {
                Grid = tbl_PatternPricingPolicies;
            }

            return Json(Grid);

        }









        [HttpPost]
        public virtual IActionResult AddPricingPolicyAll(Tbl_PricingPolicy _PP, senditem List_PP)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            try
            {

                //check integer to lis_pp
                if (List_PP.Listitem.Count() == 0)
                {
                    return Json(new { success = false, responseText = "آیتمی در جدول قوانین وجود نداشت!@@!" });
                }
                else
                {
                    //check rang

                    foreach (var item in List_PP.Listitem)
                    {
                        foreach (var item2 in List_PP.Listitem)
                        {
                            if (item2 != item)
                            {
                                if (item.MinWeight >= item2.MinWeight && item.MinWeight <= item2.MaxWeight)
                                {
                                    return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                                }
                                if (item.MaxWeight >= item2.MinWeight && item.MaxWeight <= item2.MaxWeight)
                                {
                                    return Json(new { success = false, responseText = "بازه های وزنی همپوشانی دارند" });

                                }
                            }
                        }
                    }
                    //disable
                    var Disable_All_PP = _repositoryTbl_PricingPolicy.Table.Where(p => p.CountryId == _PP.CountryId &&
                       p.Dealer_Customer_Id == _PP.Dealer_Customer_Id && p.ProviderId == _PP.ProviderId).ToList();
                    foreach (var item in Disable_All_PP)
                    {
                        item.IsActive = false;
                        item.DateUpdate = DateTime.Now;
                        item.IdUserUpdate = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_PricingPolicy.Update(item);
                    }

                    if (_PP.Id == 0)
                    {
                        //add
                        foreach (var item in List_PP.Listitem)
                        {
                            Tbl_PricingPolicy temp = new Tbl_PricingPolicy();
                            temp.CountryId = _PP.CountryId;
                            temp.DateInsert = DateTime.Now;
                            temp.Dealer_Customer_Id = _PP.Dealer_Customer_Id;
                            temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                            temp.IsActive = true;
                            temp.Mablagh = item.Mablagh;
                            temp.MaxWeight = item.MaxWeight;
                            temp.MinWeight = item.MinWeight;
                            temp.Percent = item.Percent;
                            temp.ProviderId = _PP.ProviderId;
                            temp.Tashim = item.Tashim;
                            temp.PercentTashim = item.PercentTashim;
                            temp.TypeUser = _PP.TypeUser;
                            _repositoryTbl_PricingPolicy.Insert(temp);
                        }

                        //activity log
                        _customerActivityService.InsertActivity("AddNewPricingPolicy", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));
                        return Json(new { success = true, responseText = "عملیات اضافه کردن  سیاست قیمت گذاری با موفقیت ثبت شد" });//, JsonRequestBehavior.AllowGet

                    }
                    else
                    {
                        return Json(new { success = false, responseText ="---" });
                    }
                }
               

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() });

            }


        }





    }
}
