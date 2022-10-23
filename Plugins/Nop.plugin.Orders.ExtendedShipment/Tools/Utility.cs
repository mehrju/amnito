using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Tools
{
    public static class Utility
    {
        public static List<EnumData> GetEnumList<TEnum>()
        {
            return Enum.GetValues(typeof(TEnum)).OfType<Enum>().OrderBy(p => Convert.ToInt32(p))
                .Select(p => new EnumData() { DisplayName = p.GetDisplayName(), Value = Convert.ToInt32(p) })
                .Where(p => !string.IsNullOrEmpty(p.DisplayName)).ToList();
        }
    }
}
