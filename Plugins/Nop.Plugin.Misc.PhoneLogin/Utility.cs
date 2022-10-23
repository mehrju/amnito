using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PhoneLogin
{
    public static class Utility
    {
        public static bool CheckNumber(string strPhoneNumber)
        {
            var MatchPhoneNumberPattern = "(0|\\+98)?([ ]|-|[()]){0,2}9[0|1|2|3|4|5|6|7|8|9]([ ]|-|[()]){0,3}(?:[0-9]([ ]|-|[()]){0,2}){8}";
            return strPhoneNumber != null && Regex.IsMatch(strPhoneNumber, MatchPhoneNumberPattern);
        }
    }
}
