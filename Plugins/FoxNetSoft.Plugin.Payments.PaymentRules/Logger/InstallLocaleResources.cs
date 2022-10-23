using System;
using System.IO;
using System.Text;
using System.Xml;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Localization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Logger
{
    public class InstallLocaleResources
    {
        private readonly string filepath;
        private readonly FNSLogger fnslogger_0;
        private readonly bool showDebugInfo;

        public InstallLocaleResources(string filepath, bool showDebugInfo = false)
        {
            this.filepath = filepath;
            this.showDebugInfo = showDebugInfo;
            fnslogger_0 = new FNSLogger(showDebugInfo);
        }

        public void LogMessage(string message)
        {
            if (!showDebugInfo)
                return;
            fnslogger_0.LogMessage(message);
        }

        private void method_0(
            ILocalizationService ilocalizationService_0,
            ILanguageService ilanguageService_0,
            string string_1,
            string string_2,
            bool bool_1 = true)
        {
            if (ilocalizationService_0 == null)
                throw new ArgumentNullException("localizationService");
            if (ilanguageService_0 == null)
                throw new ArgumentNullException("languageService");
            if (string.IsNullOrWhiteSpace(string_1) || string.IsNullOrWhiteSpace(string_2))
                return;
            foreach (var current in ilanguageService_0.GetAllLanguages(true, 0, true))
            {
                var stringResourceByName =
                    ilocalizationService_0.GetLocaleStringResourceByName(string_1, current.Id, false);
                if (stringResourceByName == null)
                {
                    var localeStringResource1 = new LocaleStringResource();
                    localeStringResource1.LanguageId = current.Id;
                    localeStringResource1.ResourceName = string_1;
                    localeStringResource1.ResourceValue = string_2;
                    var localeStringResource2 = localeStringResource1;
                    ilocalizationService_0.InsertLocaleStringResource(localeStringResource2);
                }
                else if (bool_1)
                {
                    stringResourceByName.ResourceValue = string_2;
                    ilocalizationService_0.UpdateLocaleStringResource(stringResourceByName);
                }
            }
        }

        private void method_1(bool bool_1 = true)
        {
            var ilanguageService_0 = EngineContext.Current.Resolve<ILanguageService>();
            EngineContext.Current.Resolve<IRepository<Language>>();
            var ilocalizationService_0 = EngineContext.Current.Resolve<ILocalizationService>();
            if (ilocalizationService_0 == null)
                throw new ArgumentNullException("localizationService");
            if (ilanguageService_0 == null)
                throw new ArgumentNullException("languageService");
            foreach (var enumerateFile in Directory.EnumerateFiles(CommonHelper.MapPath(filepath), "*.xml",
                SearchOption.TopDirectoryOnly))
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("InstallLocaleResources. Install. filePath=" + enumerateFile);
                var string_1 = "";
                var str = "";
                using (var xmlTextReader = new XmlTextReader(enumerateFile))
                {
                    while (xmlTextReader.Read())
                        switch (xmlTextReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if (xmlTextReader.Name == "LocaleResource")
                                {
                                    var attribute = xmlTextReader.GetAttribute("Name");
                                    if (!string.IsNullOrWhiteSpace(attribute)) string_1 = attribute.Trim();
                                }

                                continue;
                            case XmlNodeType.Text:
                            case XmlNodeType.CDATA:
                                if (!string.IsNullOrWhiteSpace(xmlTextReader.Value))
                                {
                                    var string_2 = xmlTextReader.Value.Trim();
                                    method_0(ilocalizationService_0, ilanguageService_0, string_1, string_2, bool_1);
                                    stringBuilder.AppendLine(string.Format("    ResourceName={0}, ResourceValue={1}",
                                        string_1, string_2));
                                }

                                string_1 = "";
                                str = "";
                                continue;
                            case XmlNodeType.EndElement:
                                string_1 = "";
                                str = "";
                                continue;
                            default:
                                continue;
                        }
                }

                LogMessage(stringBuilder.ToString());
            }
        }

        public void Install()
        {
            method_1(true);
        }

        public void Update()
        {
            method_1(false);
        }

        public void UnInstall(string languageResourceMask)
        {
        }
    }
}