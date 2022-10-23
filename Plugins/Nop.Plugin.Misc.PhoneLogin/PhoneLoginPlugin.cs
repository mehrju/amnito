using System;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.PhoneLogin
{
    /// <summary>
    /// Represents the Mobile SMS provider
    /// </summary>
    public class PhoneLoginPlugin : BasePlugin, IMiscPlugin
    {
        private readonly PhoneLoginSettings _phoneLoginSettings;
        private readonly ILogger _logger;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly CustomerSettings _customerSettings;
        private readonly IWebHelper _webHelper;

        public PhoneLoginPlugin(PhoneLoginSettings phoneLoginSettings,
            ILogger logger,
            ISettingService settingService,
            IStoreContext storeContext,
            CustomerSettings customerSettings,
            IWebHelper webHelper)
        {
            this._phoneLoginSettings = phoneLoginSettings;
            this._logger = logger;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._customerSettings = customerSettings;
            this._webHelper = webHelper;
        }

        /// <summary>
        /// Sends SMS
        /// </summary>
        /// <param name="text">SMS text</param>
        /// <returns>Result</returns>
        public bool SendSms(string text, string number)
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PhoneLogin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.PhoneLogin.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new PhoneLoginSettings
            {
               // Email = "youremail@vtext.com",
            };
            _settingService.SaveSetting(settings);

            var newCustomerSettings = _settingService.LoadSetting<CustomerSettings>();
            newCustomerSettings.UsernamesEnabled = true;
            newCustomerSettings.CheckUsernameAvailabilityEnabled = false;
            newCustomerSettings.AllowUsersToChangeUsernames = false;
            _settingService.SaveSetting(newCustomerSettings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.TestFailed", "ارسال پیام انجام نشد");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.TestSuccess", "پیام ارسال شد");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCode", "کد فعال سازی");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeWrong", "کد فعال سازی صحیح نمی باشد");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeNotEmpty", "کد فعال سازی را وارد کنید");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeTitle", "تائید شماره موبایل");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.RecoveryPassword", "بازیابی کلمه عبور");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeOk", "تائید");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeResend", "ارسال مجدد");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.MobileNumber", "شماره موبایل");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Cancel", "لغو");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.RequiredMobileNumber", "شماره موبایل را وارد کنید");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.MobileNumber.NotFound", "شماره موبایل پیدا نشد");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Password.Sent", "کلمه عبور ارسال شد.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.MobileNumber.Wrong", "شماره موبایل صحیح نمی باشد");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Enabled", "فعال سازی");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Enabled.Hint", "فعال سازی");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.LineNumber", "شماره خط ارسال");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.LineNumber.Hint", "شماره خط ارسال");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.UserName", "نام کاربری پیامک");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.UserName.Required", "شماره موبایل مورد نیاز است");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.UserName.Hint", "نام کاربری پیامک");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Password", "رمز وب سرویس");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Password.Required", "کلمه عبور مورد نیاز است");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Password.Hint", "رمز وب سرویس");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.ApiKey", "API Key");

            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.RequiredLineNumber", "شماره خط ارسال اجباریست");
            this.AddOrUpdatePluginLocaleResource("Plugins.Misc.PhoneLogin.RequiredApiKey" , "API Key اجباریست");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<PhoneLoginSettings>();

            //locales
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.TestFailed");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.TestSuccess");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCode");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeWrong");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeNotEmpty");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeTitle");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.RecoveryPassword");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeOk");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.ConfirmationCodeResend");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.MobileNumber");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Cancel");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.RequiredMobileNumber");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.MobileNumber.NotFound");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Password.Sent");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Password.MobileNumber.Wrong");

            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Enabled");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Enabled.Hint");

            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.LineNumber");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.LineNumber.Hint");

            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.UserName");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.UserName.Required");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.UserName.Hint");

            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Password");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Password.Required");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.Password.Hint");

            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.Fields.ApiKey");

            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.RequiredLineNumber");
            this.DeletePluginLocaleResource("Plugins.Misc.PhoneLogin.RequiredApiKey");

            base.Uninstall();
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/PhoneLogin/Configure";
        }
    }
}
