using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Nop.Plugin.Misc.ShippingSolutions.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageDealerCustomerServiceProviderLimitatonController : BaseAdminController
    {
        private readonly IRepository<Tbl_Dealer_Customer_ServiceProvider> _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider;
        private readonly IWorkContext _workContext;
        private readonly IDbContext _dbContext;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IStaticCacheManager _cacheManager;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;


        public ManageDealerCustomerServiceProviderLimitatonController
                  (
           IRepository<Tbl_ServicesProviders> repositoryTbl_ServicesProviders,

              IRepository<Tbl_Dealer_Customer_ServiceProvider> repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider,
              IWorkContext workContext,
              IDbContext dbContext,
              IPermissionService permissionService,
              ICustomerService customerService,
              IStaticCacheManager cacheManager,
              ILocalizationService localizationService,
              ICustomerActivityService customerActivityService


          )
        {
            _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider = repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider;
            _workContext = workContext;
            _dbContext = dbContext;
            _permissionService = permissionService;
            _customerService = customerService;
            _cacheManager = cacheManager;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
        }

        public virtual IActionResult Editlimitation(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var DealerCustomer = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.GetById(id);
            if (DealerCustomer == null || DealerCustomer.IsActive == false)
                //No category found with the specified id
                return RedirectToAction("List");
            var model = new Tbl_Dealer_Customer_ServiceProvider();
            model = DealerCustomer;

            #region ListStateApplyPricingPolicy
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "انتخاب کنید", Value = "0", Selected = true });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تخفیف", Value = "1" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "کیف پول", Value = "2" });
            model.ListStateApplyPricingPolicy.Add(new SelectListItem { Text = "تسهیم", Value = "3" });
            #endregion
            return View("/Plugins/Misc.ShippingSolutions/Views/DealerCustomerServiceProvider/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual IActionResult Edit(Tbl_Dealer_Customer_ServiceProvider model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel))
                return AccessDeniedView();

            var DealerCustomer = _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.GetById(model.Id);
            
                if (DealerCustomer == null || DealerCustomer.IsActive == false)
                {
                    if (DealerCustomer.TypeUser == 1)
                    {
                        //Customer
                        return RedirectToAction("Edit", new RouteValueDictionary(
                            new { controller = "ManageCustomerServiceProvider", action = "Edit", id = DealerCustomer.CustomerId }));

                    }
                    else
                    {
                        //delear
                        return RedirectToAction("Edit", new RouteValueDictionary(
                          new { controller = "ManageDealer", action = "Edit", id = DealerCustomer.DealerId }));

                    }
                }

            //update
            DealerCustomer.StateMonth_Day = model.StateMonth_Day;
            DealerCustomer.MaxCountpackage = model.MaxCountpackage;
            DealerCustomer.MaxWeight = model.MaxWeight;
            DealerCustomer.DateUpdate = DateTime.Now;
            DealerCustomer.IdUserUpdate = _workContext.CurrentCustomer.Id;
            _repositoryTbl_Dealer_Tbl_Dealer_Customer_ServiceProvider.Update(DealerCustomer);

            
            //activity log
            _customerActivityService.InsertActivity("EditDealerCustomerLimitation", _localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));

            SuccessNotification(_localizationService.GetResource("Nop.Plugin.Misc.ShippingSolutions.MessageUpdate"));
            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();
                return RedirectToAction("Editlimitation", new { id = DealerCustomer.Id });
            }
            else
            {
                if (DealerCustomer.TypeUser == 1)
                {
                    //Customer
                    return RedirectToAction("Edit", new RouteValueDictionary(
                        new { controller = "ManageCustomerServiceProvider", action = "Edit", id = DealerCustomer.CustomerId }));

                }
                else
                {
                    //delear
                    return RedirectToAction("Edit", new RouteValueDictionary(
                      new { controller = "ManageDealer", action = "Edit", id = DealerCustomer.DealerId }));

                }
            }
          
        }

    }
}
