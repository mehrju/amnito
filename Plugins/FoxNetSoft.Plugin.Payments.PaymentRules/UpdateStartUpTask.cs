using FoxNetSoft.Plugin.Payments.PaymentRules.Data;
using FoxNetSoft.Plugin.Payments.PaymentRules.Logger;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using System;

namespace FoxNetSoft.Plugin.Payments.PaymentRules
{
    public class UpdateStartUpTask : IStartupTask
    {
        public int Order
        {
            get
            {
                return 100;
            }
        }

        public UpdateStartUpTask()
        {
        }

        public void Execute()
        {
            PluginDescriptor pluginDescriptorBySystemName = EngineContext.Current.Resolve<IPluginFinder>().GetPluginDescriptorBySystemName(PluginLog.SystemName, LoadPluginsMode.InstalledOnly);
            if (pluginDescriptorBySystemName == null || !pluginDescriptorBySystemName.Installed)
            {
                return;
            }
            ISettingService settingService = EngineContext.Current.Resolve<ISettingService>();
            PaymentRulesObjectContext paymentRulesObjectContext = EngineContext.Current.Resolve<PaymentRulesObjectContext>();
            PaymentRulesSettings paymentRulesSetting = EngineContext.Current.Resolve<PaymentRulesSettings>();
            if (paymentRulesSetting.Version < 111)
            {
                paymentRulesSetting.Version = 111;
                settingService.SaveSetting<PaymentRulesSettings>(paymentRulesSetting, 0);
                paymentRulesObjectContext.UpdateInstallationScript(paymentRulesSetting.Version);
            }
        }
    }
}