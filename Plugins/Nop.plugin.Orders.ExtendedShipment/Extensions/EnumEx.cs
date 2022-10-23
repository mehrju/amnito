using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class EnumEx
    {
        public static string GetDisplayName(this Enum en)
        {
            return en.GetType().GetMember(en.ToString()).FirstOrDefault()?.GetCustomAttribute<DisplayAttribute>()?.Name;
        }

    }
}
