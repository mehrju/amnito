using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
//using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Services;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class Ap_NewHomeController : BasePublicController
    {
        #region fileds
        private readonly INewCheckout _newCheckout;
        private readonly IWorkContext _workContext;
        private readonly IApService _apService;
        private readonly I_IndexPageService _I_IndexPageService;
       // private readonly IRepository<Tbl_ServiceProviderDashboard> _repositoryTbl_ServiceProviderDashboard;
        #endregion

        public Ap_NewHomeController(
          //  IRepository<Tbl_ServiceProviderDashboard> repositoryTbl_ServiceProviderDashboard,
            IApService apService
          , INewCheckout newCheckout
          , IWorkContext workContext
, I_IndexPageService I_IndexPageService
          )
        {
            _apService = apService;
            _workContext = workContext;
            _newCheckout = newCheckout;
           // _repositoryTbl_ServiceProviderDashboard = repositoryTbl_ServiceProviderDashboard;
            _I_IndexPageService = I_IndexPageService;

        }
        [HttpPost, HttpGet]
        public IActionResult Index(string startupData)
        {
            //return Content("تا اطلاع ثانوی دسترسی به این قسمت امکان پذیر نمی باشد");
            HttpContext.Session.SetString("ComeFrom", "Ap");
            //_newCheckout.Log("Input from Ap", startupData ?? "");
            if (string.IsNullOrEmpty(startupData))
            {
                _newCheckout.Log("اطلاعات پست شده از آپ نا معتبر می باشد", startupData ?? "فیلد اطلاعاتی خالی می باشد");
                return Content("در حال حاضر این سرویس در دسترس نمی باشد");

            }
            var apStartupModel = JsonConvert.DeserializeObject<ApStartupModel>(startupData);
            if (apStartupModel == null)
            {
                _newCheckout.Log("اطلاعات پست شده از آپ نا معتبر می باشد", startupData ?? "فیلد اطلاعاتی خالی می باشد");
                return Content("در حال حاضر این سرویس در دسترس نمی باشد");
            }
            if (string.IsNullOrEmpty(apStartupModel.authToken))
            {
                _newCheckout.Log("اطلاعات پست شده از آپ نا معتبر می باشد", startupData ?? "فیلد اطلاعاتی خالی می باشد");
                return Content("در حال حاضر این سرویس در دسترس نمی باشد");
            }
            string msg = "";
            var customer = _apService.AuthenticatApUser(out msg, apStartupModel.authToken);
            if (customer == null)
            {
                _newCheckout.Log(msg, "");
                return Content(msg);
            }

            HttpContext.Session.SetString("ApConfigValue", apStartupModel.config);
            //درخواست اطلاعات کاربر
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                ApStartupConfig = apStartupModel.config
            };

            if (!_apService.IsValidCustomer(customer))
            {
                _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر آپ", "");
                return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
            }
            _newCheckout.Log("session for AP", HttpContext.Session.GetString("ApConfigValue"));
            
            try
            {
                model.vmServiceProvider_Index = _I_IndexPageService.GetServiceProvider();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Index.cshtml", model);
        }
        [HttpPost, HttpGet]
        public IActionResult _Index()
        {
           // return Content("تا اطلاع ثانوی دسترسی به این قسمت امکان پذیر نمی باشد");
            HttpContext.Session.SetString("ComeFrom", "Ap");
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                ApStartupConfig = ""
            };
            try
            {
                model.vmServiceProvider_Index = _I_IndexPageService.GetServiceProvider();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Index.cshtml", model);
        }
        [HttpPost, HttpGet]
        public IActionResult _NewIndex()
        {

            if (!IsValidCustomer())
                return RedirectToRoute("Login", new { returnUrl = Url.RouteUrl("_SepIPGStartup") });
            //return Content("تا اطلاع ثانوی دسترسی به این قسمت امکان پذیر نمی باشد");
            HttpContext.Session.SetString("ComeFrom", "Ap");
            NewCheckoutViewModel model = new NewCheckoutViewModel()
            {
                IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                ApStartupConfig = ""
            };
            try
            {
                model.vmServiceProvider_Index = _I_IndexPageService.GetServiceProvider();
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/Index.cshtml", model);
        }
        public bool IsValidCustomer()
        {
            var customer = _workContext.CurrentCustomer;
            if (customer == null || !customer.IsRegistered() || customer.IsGuest() || customer.Username == null)
                return false;
            return true;
        }
    }
}
