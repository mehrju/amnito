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
    public class ManageSalesPartnersPercentController : BaseAdminController
    {
        private readonly IRepository<Tbl_SalesPartnersPercent> _repositoryTbl_SalesPartnersPercent;


        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;

        public ManageSalesPartnersPercentController
            (
              IRepository<Tbl_SalesPartnersPercent> repositoryTbl_SalesPartnersPercent,

              IWorkContext workContext,
              IDbContext dbContext,
              IPermissionService permissionService,
              ICustomerService customerService,
              IStaticCacheManager cacheManager,
              ILocalizationService localizationService,
              ICustomerActivityService customerActivityService
            )
        {
            _repositoryTbl_SalesPartnersPercent = repositoryTbl_SalesPartnersPercent;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
        }
        public virtual IActionResult Index()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            return View("/Plugins/Misc.ShippingSolutions/Views/SalesPartnersPercent/SalesPartnersPercent.Info.cshtml");
        }


        [HttpPost]
        public virtual IActionResult Plan()
        {

            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedKendoGridJson();

            var Grid = (dynamic)null;
            var tbl_SalesPartnersPercent = _repositoryTbl_SalesPartnersPercent.Table.Select(p => new
            {
                id = p.Id,
                ofday = p.OfDay,
                upday = p.UpDay,
                name = p.Name,
                percent = p._Percent
            }).ToList();
            if (tbl_SalesPartnersPercent != null)
            {
                Grid = tbl_SalesPartnersPercent;
            }
            return Json(Grid);

        }


        [HttpPost]
        [AdminAntiForgery(true)]
        public virtual IActionResult EditPlan(senditem_SalesPartnersPercent List_PP)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            try
            {

                if (List_PP.Listitem_SalesPartnersPercent.Count() == 0)
                {
                    return Json(new { success = false, responseText = "آیتمی در جدول قوانین وجود نداشت!@@!" });
                }
                else
                {

                    foreach (var item in List_PP.Listitem_SalesPartnersPercent)
                    {
                        Tbl_SalesPartnersPercent p = _repositoryTbl_SalesPartnersPercent.GetById(item.id);
                        p.Name = item.name;
                        p.OfDay = item.ofday;
                        p.UpDay = item.upday;
                        p._Percent = item.percent;
                        p.DateUpdate = DateTime.Now;
                        p.IdUserUpdate = _workContext.CurrentCustomer.Id;
                        _repositoryTbl_SalesPartnersPercent.Update(p);
                        _customerActivityService.InsertActivity("UpdateSalesPartnersPercent", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"), p.Id.ToString());
                    }
                    return Json(new { success = true, responseText = "عملیات اضافه کردن  سیاست قیمت گذاری با موفقیت ثبت شد" });//, JsonRequestBehavior.AllowGet

                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() });

            }


        }



        [HttpPost]
        public virtual IActionResult AddSalesPartnersPercent(Tbl_SalesPartnersPercent _PP)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();
            try
            {
                
               

                if (_PP.Id == 0)
                {
                    //add

                    Tbl_SalesPartnersPercent p = new Tbl_SalesPartnersPercent();
                    p.Name = _PP.Name;
                    p.OfDay = _PP.OfDay;
                    p.UpDay = _PP.UpDay;
                    p._Percent = _PP._Percent;
                    p.DateInsert = DateTime.Now;
                    p.IdUserInsert = _workContext.CurrentCustomer.Id;
                    _repositoryTbl_SalesPartnersPercent.Insert(p);
                    //activity log
                    _customerActivityService.InsertActivity("SalesPartnersPercent", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.Create"));
                    return Json(new { success = true, responseText = "عملیات اضافه کردن یک سیاست قیمت گذاری با موفقیت ثبت شد" });//, JsonRequestBehavior.AllowGet

                }
                else
                {
                    
                    return Json(new { success = false, responseText = "عملیات ویرایش سیاست قیمت گذاری با موفقیت ثبت نشد" });

                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.ToString() });

            }


        }

    }
}
