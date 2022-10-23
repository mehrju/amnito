using System.Data.Entity;
using Nop.Core.Infrastructure;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Data
{
    public class EfStartUpTask : IStartupTask
    {
        public int Order => 1;

        public void Execute()
        {
            Database.SetInitializer<PaymentRulesObjectContext>(null);
        }
    }
}