using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public static class Common
    {
        public static DateTime PersianToDateTime(string persianDate, char betweenChar = '/')
        {
            if (persianDate == null || persianDate == "null" || persianDate.Length < 8)
                return DateTime.MinValue;
            PersianCalendar pc = new PersianCalendar();
            var dateParts = persianDate.Split(betweenChar);
            if (dateParts.Length < 3)
                return DateTime.MinValue;
            return pc.ToDateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]), Convert.ToInt32(dateParts[2]), 0, 0, 0, 0);
        }


        public static string DateTimeToPersian(DateTime date, char betweenChar)
        {
            PersianCalendar pc = new PersianCalendar();
            return $"{pc.GetYear(date)}{betweenChar}{pc.GetMonth(date).ToString().PadLeft(2, '0')}{betweenChar}{pc.GetDayOfMonth(date).ToString().PadLeft(2, '0')}";
        }
        

    }
}
