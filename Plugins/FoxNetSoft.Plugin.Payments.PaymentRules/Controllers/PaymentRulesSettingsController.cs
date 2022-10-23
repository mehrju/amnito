using System.IO;
using FoxNetSoft.Plugin.Payments.PaymentRules.Logger;
using FoxNetSoft.Plugin.Payments.PaymentRules.Models;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Controllers
{
    [Area("Admin")]
    [AuthorizeAdmin(false)]
    public class PaymentRulesSettingsController : BasePluginController
    {
        private readonly FNSLogger fnslogger_0;

        private readonly ILocalizationService FurmIkMtxL;

        private readonly IPermissionService ipermissionService_0;
        private readonly ISettingService isettingService_0;

        private readonly PaymentRulesSettings paymentRulesSettings_0;

        private readonly bool showDebugInfo;

        public PaymentRulesSettingsController(ISettingService settingService, PaymentRulesSettings pluginsettings,
            ILocalizationService localizationService, IPermissionService permissionService)
        {
            isettingService_0 = settingService;
            paymentRulesSettings_0 = pluginsettings;
            FurmIkMtxL = localizationService;
            ipermissionService_0 = permissionService;
            showDebugInfo = paymentRulesSettings_0.showDebugInfo;
            fnslogger_0 = new FNSLogger(showDebugInfo);
        }

        public IActionResult ClearLogFile()
        {
            if (!ipermissionService_0.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            fnslogger_0.ClearLogFile();
            return RedirectToAction("Configure", "PaymentRulesSettings", new { area = "Admin" });
        }

        public IActionResult Configure()
        {
            if (!ipermissionService_0.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var ILR = new InstallLocaleResources(string.Concat(PluginLog.Folder, "Resources/"), false);
            ILR.Update();
            var paymentRulesSettingsModel = new PaymentRulesSettingsModel
            {
                Enabled = paymentRulesSettings_0.Enabled,
                SerialNumber = paymentRulesSettings_0.SerialNumber,
                showDebugInfo = paymentRulesSettings_0.showDebugInfo,
                IsRegisted = true
            };
            method_2();
            return View(method_1("Configure"), paymentRulesSettingsModel);
        }

        [HttpPost]
        public IActionResult Configure(PaymentRulesSettingsModel model)
        {
            if (!ipermissionService_0.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            if (!ModelState.IsValid) return Configure();
            paymentRulesSettings_0.SerialNumber =
                string.IsNullOrEmpty(model.SerialNumber) ? "" : model.SerialNumber.Trim();
            paymentRulesSettings_0.showDebugInfo = model.showDebugInfo;
            paymentRulesSettings_0.Enabled = model.Enabled;
            isettingService_0.SaveSetting(paymentRulesSettings_0, 0);
            SuccessNotification(FurmIkMtxL.GetResource("Admin.Configuration.Updated"), true);
            return Configure();
        }

        public IActionResult GetLogFile()
        {
            if (!ipermissionService_0.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var logFilePath = fnslogger_0.GetLogFilePath();
            if (!System.IO.File.Exists(logFilePath))
                return RedirectToAction("Configure", "PaymentRulesSettings", new { area = "Admin" });
            var numArray = System.IO.File.ReadAllBytes(logFilePath);
            return File(numArray, "application/octet-stream", Path.GetFileName(logFilePath));
        }

        private void LogMsg(string string_0)
        {
            if (showDebugInfo) fnslogger_0.LogMessage(string_0);
        }

        private string method_1(string string_0)
        {
            return string.Concat(PluginLog.Folder, "Views/PaymentRulesSettings/", string_0, ".cshtml");
        }

        private void method_2()
        {
            //if (this.testDataPlugin_0.IsExpired)
            //{
            //	this.ErrorNotification(this.FurmIkMtxL.GetResource("Admin.FoxNetSoft.PaymentRules.IsExpired"), true);
            //	return;
            //}
            //if (!this.testDataPlugin_0.IsRegisted)
            //{
            //	this.ErrorNotification(this.FurmIkMtxL.GetResource("Admin.FoxNetSoft.PaymentRules.IsNoRegisted"), true);
            //}
        }
    }
}