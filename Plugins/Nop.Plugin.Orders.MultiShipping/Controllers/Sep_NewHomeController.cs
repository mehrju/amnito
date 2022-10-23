using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.plugin.Orders.ExtendedShipment.Services;
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
    public class Sep_NewHomeController : BasePublicController
    {
        #region fileds
        private readonly INewCheckout _newCheckout;
        private readonly IWorkContext _workContext;
        private readonly ISepService _sepService;
        private readonly I_IndexPageService _I_IndexPageService;
        private readonly ISecurityService _securityService;
        // private readonly IRepository<Tbl_ServiceProviderDashboard> _repositoryTbl_ServiceProviderDashboard;
        #endregion

        public Sep_NewHomeController(
            //  IRepository<Tbl_ServiceProviderDashboard> repositoryTbl_ServiceProviderDashboard,
            ISepService sepService
           , INewCheckout newCheckout
           , IWorkContext workContext
           , I_IndexPageService I_IndexPageService
            , ISecurityService securityService
          )
        {
            _securityService = securityService;
            _sepService = sepService;
            _workContext = workContext;
            _newCheckout = newCheckout;
            // _repositoryTbl_ServiceProviderDashboard = repositoryTbl_ServiceProviderDashboard;
            _I_IndexPageService = I_IndexPageService;

        }
        [HttpPost, HttpGet]
        public IActionResult MainIndex()
        {
            return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/MainIndex.cshtml");
        }
        [HttpPost, HttpGet]
        public IActionResult Index()
        {
            try
            {

                //var encryptedMobileNo = HttpContext.Request.Query["msisdn"].ToString();

                //HttpContext.Session.SetString("ComeFrom", "Sep");
                //_newCheckout.Log("Input from Sep", encryptedMobileNo ?? "");
                //if (string.IsNullOrEmpty(encryptedMobileNo))
                //{
                //    _newCheckout.Log("اطلاعات پست شده از sep نا معتبر می باشد", encryptedMobileNo ?? "فیلد اطلاعاتی خالی می باشد");
                //    return Content("در حال حاضر این سرویس در دسترس نمی باشد");

                //}
                //string Mobile = _securityService.DecryptSepInput(encryptedMobileNo);
                ////if (Mobile != "09139064053")
                ////    return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد");
                //if (string.IsNullOrEmpty(Mobile))
                //{
                //    _newCheckout.Log(" نام کاربری ارسال از 724 به درستی رمز گشایی نشده یا خالی مباشد", "");
                //    return Content("شماره تلفن همراه ارسال نامعتبر می باشد");
                //}

                //string msg = "";
                //var customer = _sepService.AuthenticatSepUser(out msg, Mobile);
                //if (customer == null)
                //{
                //    _newCheckout.Log(msg, "");
                //    return Content(msg);
                //}

                //HttpContext.Session.SetString("ApConfigValue", apStartupModel.config);
                ////درخواست اطلاعات کاربر
                NewCheckoutViewModel model = new NewCheckoutViewModel()
                {
                    IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                    IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                };

                if (!_sepService.IsValidCustomer(_workContext.CurrentCustomer))
                {
                    _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر sep", "");
                    return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد. مجددا سعی کنید");
                }
                //_newCheckout.Log("session for AP", HttpContext.Session.GetString("ApConfigValue"));

                try
                {
                    model.vmServiceProvider_Index = _I_IndexPageService.GetServiceProvider();
                }
                catch (Exception ex)
                {
                    LogException(ex);
                }
                return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/Index.cshtml", model);

            }
            catch (Exception ex)
            {
                LogException(ex);
                return Content("در حال حاضر امکان درسترسی به سرویس مورد نظر وجود ندارد");
            }
            
        }
        [HttpPost, HttpGet]
        public IActionResult login()
        {
            try
            {
                var encryptedMobileNo = HttpContext.Request.Query["msisdn"].ToString();

                HttpContext.Session.SetString("ComeFrom", "Sep");
                _newCheckout.Log("Input from Sep", encryptedMobileNo ?? "");
                if (string.IsNullOrEmpty(encryptedMobileNo))
                {
                    _newCheckout.Log("اطلاعات پست شده از sep نا معتبر می باشد", encryptedMobileNo ?? "فیلد اطلاعاتی خالی می باشد");
                    return Json(new {success=false,message="در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید" });

                }
                string Mobile = _securityService.DecryptSepInput(encryptedMobileNo);
                //if (Mobile != "09139064053")
                //    return Content("در حال حاضر امکان دسترسی به این سرویس برای شما مقدور نمی باشد");
                if (string.IsNullOrEmpty(Mobile))
                {
                    _newCheckout.Log(" نام کاربری ارسال از 724 به درستی رمز گشایی نشده یا خالی مباشد", "");
                    return Json(new { success = false, message = "در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید" });
                }

                string msg = "";
                var customer = _sepService.AuthenticatSepUser(out msg, Mobile);
                if (customer == null)
                {
                    _newCheckout.Log(msg, "");
                    return Json(new { success = false, message = "در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید" });
                }

                //HttpContext.Session.SetString("ApConfigValue", apStartupModel.config);
                ////درخواست اطلاعات کاربر
                NewCheckoutViewModel model = new NewCheckoutViewModel()
                {
                    IsAgent = _workContext.CurrentCustomer.IsInCustomerRole("mini-Administrators"),
                    IsInCodRole = _workContext.CurrentCustomer.IsInCustomerRole("COD"),
                };

                if (!_sepService.IsValidCustomer(customer))
                {
                    _newCheckout.Log("خطا در زمان اعتبار سنجی کاربر sep", "");
                    return Json(new { success = false, message = "در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید" });
                }
               
                return Json(new { success = true, message = "" });

            }
            catch (Exception ex)
            {
                LogException(ex);
                return Json(new { success = false, message = "در حال حاضر امکان اعتبار سنجی وجود ندارد، لطفا مجددا سعی کنید" });
            }

        }
        [HttpPost, HttpGet]
        public IActionResult _Index()
        {
            // return Content("تا اطلاع ثانوی دسترسی به این قسمت امکان پذیر نمی باشد");
            HttpContext.Session.SetString("ComeFrom", "Sep");
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
            return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/Index.cshtml", model);
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
            return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/Sep/Index.cshtml", model);
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
