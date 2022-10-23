using System;
using Nop.Core;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Logger
{
    public static class PluginLog
    {
        public const int ReleaseVersion = 111;

        public static string SystemName => "FoxNetSoft.PaymentRules";

        public static string SystemName2 => "PaymentRules.FoxNetSoft.Plugin.Payments";

        public static string Folder => "~/Plugins/FoxNetSoft.PaymentRules/";

        public static string SystemName2All => "PluginCollection.FoxNetSoft.Plugin.Misc";

        public static string SystemName3 => "PaymentRulesSettings.SerialNumber";

        public static DateTime NopCommerceReleaseDate
        {
            get
            {
                switch (NopVersion.CurrentVersion)
                {
                    case "3.40":
                        return new DateTime(2014, 7, 17);
                    case "3.50":
                        return new DateTime(2014, 12, 8);
                    case "3.60":
                        return new DateTime(2015, 6, 15);
                    case "3.70":
                        return new DateTime(2015, 12, 7);
                    case "3.80":
                        return new DateTime(2016, 8, 3);
                    case "3.90":
                        return new DateTime(2017, 3, 15);
                    case "4.00":
                        return new DateTime(2017, 11, 9);
                    case "4.10":
                        return new DateTime(2018, 7, 27);
                    default:
                        return DateTime.UtcNow;
                }
            }
        }
    }
}