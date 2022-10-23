using Nop.plugin.Orders.ExtendedShipment.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class DateTimeEx
    {
        public static string ToPersianDate(this DateTime date,char betweenChar = '/')
        {
            return Common.DateTimeToPersian(date, betweenChar);
        } 

        public static string ToDateString(this DateTime date)
        {
            return $"{date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second}";
        }
    }
}
