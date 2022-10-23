using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Models.Grid;
using Nop.Plugin.Misc.ShippingSolutions.Models.Search;
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
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageProductPatternPricingController : BaseAdminController
    {
        private readonly IRepository<Tbl_Product_PatternPricing> _repositoryTbl_Product_PatternPricing;
        private readonly IRepository<Tbl_PatternPricingPolicy> _repositoryTbl_PatternPricingPolicy;


        private readonly ICategoryService _CategoryService;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IProductService _productService;

        public ManageProductPatternPricingController
            (
            ICategoryService CategoryService,
            IRepository<Tbl_PatternPricingPolicy> repositoryTbl_PatternPricingPolicy,
                IRepository<Tbl_Product_PatternPricing> repositoryTbl_Product_PatternPricing,
                IProductService productService,
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
            _repositoryTbl_Product_PatternPricing = repositoryTbl_Product_PatternPricing;
            _productService = productService;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
        }






        #region index&list
        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }
        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Model = new Search_ProductPatternPricing();
            var patterns = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.IsActive && p.IdParent == 0).ToList();
            if (patterns.Count > 0)
            {
                Model.ListPatternPricing.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var item in patterns)
                {
                    Model.ListPatternPricing.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() });

                }

            }
            #region ListStateApplyPricingPolicy
            Model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            Model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            Model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            Model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/ProductPatternPricing/List.cshtml", Model);
        }
        [HttpPost]
        public virtual IActionResult ProductList(DataSourceRequest command, Search_ProductPatternPricing model)
        {
            String ErrrorMassege = "";
            //we use own own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();
            var gridModel = new DataSourceResult();
            var Final_Product = new List<Grid_ProductPatternPricing>();

            List<Tbl_Product_PatternPricing> tbl_productpp = _repositoryTbl_Product_PatternPricing.Table.GroupBy(p => p.IdProduct).Select(g => g.OrderBy(c => c.Id).FirstOrDefault()).ToList();

            try
            {
                if (tbl_productpp.Count > 0)
                {
                    if (model.Search_ProductPatternPricing_ActiveSearch == true)
                    {
                        if (!string.IsNullOrEmpty(model.Search_ProductPatternPricing_ProductName))
                        {
                            var products = _productService.SearchProducts(
                                          pageIndex: command.Page - 1,
                                          pageSize: command.PageSize,
                                           keywords: model.Search_ProductPatternPricing_ProductName,
                                          showHidden: true,
                                          overridePublished: true
                                      );
                            if (products.Count > 0)
                            {

                                tbl_productpp = (from p in tbl_productpp
                                                 join c in products on p.IdProduct equals c.Id
                                                 select p).ToList();
                            }

                        }
                        if (model.Search_ProductPatternPricing_IdPatternPricing > 0)
                        {
                            var allproduct = _repositoryTbl_Product_PatternPricing.Table.ToList();

                            tbl_productpp = (from p in tbl_productpp
                                             join c in allproduct on p.Id equals c.Id
                                             where c.IdPatternPricing == model.Search_ProductPatternPricing_IdPatternPricing
                                             select p).ToList();
                        }


                    }

                    foreach (var item in tbl_productpp)
                    {
                        Grid_ProductPatternPricing temp = new Grid_ProductPatternPricing();
                        temp.Id = item.Id;
                        temp.Grid_ProductPatternPricing_ProductName = _productService.GetProductById(item.IdProduct).Name;
                        temp.Grid_ProductPatternPricing_IsActive = item.IsActive;
                        temp.Grid_ProductPatternPricing_PatternNames = GetNamePattern(item.IdProduct);
                        temp.Grid_ProductPatternPricing_StateClaculateMonth = item.StateClaculateMonth;
                        temp.Grid_ProductPatternPricing_StateApplyPricingPolicy = item.StateApplyPricingPolicy == 1 ? "تخفیف" : item.StateApplyPricingPolicy == 2 ? "کیف پول" : item.StateApplyPricingPolicy == 3 ? "تسهیم" : "-";
                        //temp.Grid_ProductPatternPricing_Price = item.Price;
                        Final_Product.Add(temp);

                    }
                }
                else
                {
                    ErrrorMassege = "No Data Exist";
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
                    Data = Final_Product,
                    Total = Final_Product.Count,
                    Errors = ErrrorMassege
                };

            }

            return Json(gridModel);
        }
        public string GetNamePattern(int Idppp)
        {
            string s = "";
            if (Idppp > 0)
            {
                var ppps = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.IdProduct == Idppp && p.IsActive).ToList();

                if (ppps.Count > 0)
                {
                    foreach (var item in ppps)
                    {
                        s += _repositoryTbl_PatternPricingPolicy.GetById(item.IdPatternPricing).Name + " ";
                    }
                }
            }
            return s;
        }


        [HttpPost]
        public virtual IActionResult PatternPricingList(DataSourceRequest command, int Id)
        {
            String ErrrorMassege = "";
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();


            List<Tbl_Product_PatternPricing> tbl_Product_Patterns = new List<Tbl_Product_PatternPricing>();
            var gridModel = new DataSourceResult();
            var Final_Pattern = new List<Grid_PatternPricingPolicy>();
            try
            {
                if (Id > 0)
                {

                    var idproduct = _repositoryTbl_Product_PatternPricing.GetById(Id).IdProduct;
                    tbl_Product_Patterns = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.IdProduct == idproduct).ToList();

                    if (tbl_Product_Patterns.Count > 0)
                    {
                        foreach (var item in tbl_Product_Patterns)
                        {
                            var pat = _repositoryTbl_PatternPricingPolicy.GetById(item.IdPatternPricing);
                            Grid_PatternPricingPolicy temp = new Grid_PatternPricingPolicy();
                            temp.Id = item.Id;
                            temp.Grid_PatternPricingPolicy_Category_Name = _CategoryService.GetCategoryById(pat.CategoryId).Name;
                            temp.Grid_PatternPricingPolicy_IsActive = item.IsActive;
                            temp.Grid_PatternPricingPolicy_UserInsert = _customerService.GetCustomerById(item.IdUserInsert).GetFullName();
                            temp.Grid_PatternPricingPolicy_UserUpdate = item.IdUserUpdate == null ? "" : _customerService.GetCustomerById((int)item.IdUserUpdate).GetFullName();
                            temp.Grid_PatternPricingPolicy_DateInsert = item.DateInsert;
                            temp.Grid_PatternPricingPolicy_DateUpdate = item.DateUpdate;
                            temp.Grid_PatternPricingPolicy_MaxCount = pat.MaxCount;
                            temp.Grid_PatternPricingPolicy_MinCount = pat.MinCount;
                            temp.Grid_PatternPricingPolicy_Name = pat.Name;
                            Final_Pattern.Add(temp);

                        }

                    }
                    else
                    {
                        ErrrorMassege = "No Exsit Data";
                    }

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
                    Data = Final_Pattern,
                    Total = Final_Pattern.Count,
                    Errors = ErrrorMassege,
                };

            }
            return Json(gridModel);

        }
        #endregion

        #region Diables & active 
        [HttpPost]
        public virtual IActionResult DisableProduct(ICollection<int> selectedIds)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_Product_PatternPricing temp = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.Id == item).FirstOrDefault();

                        if (temp != null)
                        {
                            temp.IsActive = false;
                            temp.DateUpdate = DateTime.Now;
                            temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Product_PatternPricing.Update(temp);
                        }
                    }
                }
                        

                //activity log
                _customerActivityService.InsertActivity("DisableProductPatternPricing", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActiveProduct(ICollection<int> selectedIds)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                if (selectedIds != null)
                {
                    foreach (var item in selectedIds)
                    {
                        Tbl_Product_PatternPricing temp = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.Id == item).FirstOrDefault();

                        if (temp != null)
                        {
                            temp.IsActive = true;
                            temp.DateUpdate = DateTime.Now;
                            temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                            _repositoryTbl_Product_PatternPricing.Update(temp);

                        }
                    }
                }

                //activity log
                _customerActivityService.InsertActivity("ActiveProductPatternPricing", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }
        //
        [HttpPost]
        public virtual IActionResult DisablePatternPricing(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Product_PatternPricing temp = _repositoryTbl_Product_PatternPricing.GetById(id);

                if (temp != null)
                {
                    temp.IsActive = false;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Product_PatternPricing.Update(temp);
                    
                }

                //activity log
                _customerActivityService.InsertActivity("DisableProductPatternPricing", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
            }
            catch (Exception ex)
            {

                ErrorNotification(ex.ToString());
            }
            return Json(new { Result = true });

        }

        [HttpPost]
        public virtual IActionResult ActivePatternPricing(int id)
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            try
            {
                Tbl_Product_PatternPricing temp = _repositoryTbl_Product_PatternPricing.GetById(id);

                if (temp != null)
                {
                    temp.IsActive = true;
                    temp.DateUpdate = DateTime.Now;
                    temp.IdUserUpdate = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_Product_PatternPricing.Update(temp);

                }

                //activity log
                _customerActivityService.InsertActivity("ActiveProductPatternPricing", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
                SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageDelete"));
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
            var model = new Tbl_Product_PatternPricing();
            //model.ListPatternPricing.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });

            #region list product
            var products = _productService.SearchProducts(
                                         showHidden: true,
                                         overridePublished: true
                                     );
            if (products.Count > 0)
            {
                if (products.Count > 0)
                {
                    model.ListProduct.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                    foreach (var c in products)
                    {

                        model.ListProduct.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                    }
                }
                else
                {
                    model.ListProduct.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
            }


            #endregion
            #region  get list patterns
            var patterns = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.IsActive && p.IdParent==0).ToList();
            if (patterns.Count > 0)
            {
                model.ListPatternPricing.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in patterns)
                {

                    model.ListPatternPricing.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                }
            }
            else
            {
                model.ListPatternPricing.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            #region ListStateApplyPricingPolicy
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/ProductPatternPricing/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Create(Tbl_Product_PatternPricing model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();


            var DuplicateProduct = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.IdProduct == model.IdProduct).FirstOrDefault();
            if (DuplicateProduct != null)
            {
                ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                return RedirectToAction("List");
            }

            if (model._IdPatternPricing.Count() > 0)
            {
                foreach (var item in model._IdPatternPricing)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_Product_PatternPricing Duplicate = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.IdProduct == model.IdProduct && p.IdPatternPricing == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_Product_PatternPricing.Update(Duplicate);
                            }
                            //ErrorNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Duplicate"));
                            //return RedirectToAction("Edit", new { id = Provider.Id });

                        }
                        Tbl_Product_PatternPricing temp = new Tbl_Product_PatternPricing();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.StateApplyPricingPolicy = model.StateApplyPricingPolicy;
                        temp.IsActive = true;
                        temp.IdProduct = model.IdProduct;
                        temp.StateClaculateMonth = model.StateClaculateMonth;
                        //temp.Price = model.Price;
                        temp.IdPatternPricing = item;
                        _repositoryTbl_Product_PatternPricing.Insert(temp);

                    }

                }

            }


            //activity log
            _customerActivityService.InsertActivity("AddNewProductPatternPricing", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), model.IdProduct.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = model.Id });
            }
            return RedirectToAction("List");
            //}


            //return View("/Plugins/Misc.ShippingSolutions/Views/TransportationTypes/Create.cshtml",model);
        }

        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var product = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.Id == id).FirstOrDefault();

            if (product == null || product.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = new Tbl_Product_PatternPricing();
            model.Id = product.Id;
            model.IdProduct = product.IdProduct;
            model.StateApplyPricingPolicy = product.StateApplyPricingPolicy;
            model.NameProduct = _productService.GetProductById(model.IdProduct).Name;
            model.StateClaculateMonth = product.StateClaculateMonth;
            //model.Price = product.Price;
            #region list product
            var products = _productService.SearchProducts(
                                         showHidden: true,
                                         overridePublished: true
                                     );
            if (products.Count > 0)
            {
                if (products.Count > 0)
                {
                    model.ListProduct.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                    foreach (var c in products)
                    {

                        model.ListProduct.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                    }
                }
                else
                {
                    model.ListProduct.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
                }
            }


            #endregion
            #region  get list patterns
            var patterns = _repositoryTbl_PatternPricingPolicy.Table.Where(p => p.IsActive == true).ToList();
            if (patterns.Count > 0)
            {
                model.ListPatternPricing.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
                foreach (var c in patterns)
                {

                    model.ListPatternPricing.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
                }
            }
            else
            {
                model.ListPatternPricing.Add(new SelectListItem { Text = "داده ای وجود ندارد", Value = "0" });
            }
            #endregion
            #region ListStateApplyPricingPolicy
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion

            return View("/Plugins/Misc.ShippingSolutions/Views/ProductPatternPricing/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Product_PatternPricing model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var Product = _repositoryTbl_Product_PatternPricing.GetById(model.Id);
            if (Product == null || Product.IsActive == false)
                return RedirectToAction("List");

            var Patterns = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.IdProduct == Product.IdProduct).ToList();

            if (Product.IdProduct != model.IdProduct)
            {
                foreach (var item in Patterns)
                {
                    item.IdProduct = model.IdProduct;
                    item.DateUpdate = DateTime.Now;
                    item.IdUserUpdate = _workContext.CurrentCustomer.Id;
                }
            }
            if (Product.StateApplyPricingPolicy != model.StateApplyPricingPolicy)
            {
                foreach (var item in Patterns)
                {
                    item.StateApplyPricingPolicy = model.StateApplyPricingPolicy;
                    item.DateUpdate = DateTime.Now;
                    item.IdUserUpdate = _workContext.CurrentCustomer.Id;
                }
            }
            if (Product.StateClaculateMonth != model.StateClaculateMonth)
            {
                foreach (var item in Patterns)
                {
                    item.StateClaculateMonth = model.StateClaculateMonth;
                    item.DateUpdate = DateTime.Now;
                    item.IdUserUpdate = _workContext.CurrentCustomer.Id;
                }
            }
            //if (Product.Price != model.Price)
            //{
            //    foreach (var item in Patterns)
            //    {
            //        item.Price = model.Price;
            //        item.DateUpdate = DateTime.Now;
            //        item.IdUserUpdate = _workContext.CurrentCustomer.Id;
            //    }
            //}
            _repositoryTbl_Product_PatternPricing.Update(Patterns);

            if (model._IdPatternPricing.Count() > 0)
            {
                foreach (var item in model._IdPatternPricing)
                {
                    if (item > 0)
                    {  //check duplicate
                        Tbl_Product_PatternPricing Duplicate = _repositoryTbl_Product_PatternPricing.Table.Where(p => p.IdProduct == model.IdProduct && p.IdPatternPricing == item).FirstOrDefault();
                        if (Duplicate != null)
                        {
                            if (Duplicate.IsActive == false)
                            {
                                Duplicate.IsActive = true;
                                Duplicate.DateUpdate = DateTime.Now;
                                Duplicate.IdUserUpdate = _workContext.CurrentCustomer.Id;
                                _repositoryTbl_Product_PatternPricing.Update(Duplicate);
                            }
                            
                        }
                        Tbl_Product_PatternPricing temp = new Tbl_Product_PatternPricing();
                        temp.DateInsert = DateTime.Now;
                        temp.IdUserInsert = _workContext.CurrentCustomer.Id;
                        temp.StateApplyPricingPolicy = model.StateApplyPricingPolicy;
                        temp.IsActive = true;
                        temp.IdProduct = model.IdProduct;
                        temp.StateClaculateMonth = model.StateClaculateMonth;
                        //temp.Price = model.Price;
                        temp.IdPatternPricing = item;
                        _repositoryTbl_Product_PatternPricing.Insert(temp);

                    }

                }

            }
            //activity log
            _customerActivityService.InsertActivity("updateProductPatternPricing", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), model.IdProduct.ToString());

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = Product.Id });
            }
            return RedirectToAction("List");
        }


        #endregion

    }
}
