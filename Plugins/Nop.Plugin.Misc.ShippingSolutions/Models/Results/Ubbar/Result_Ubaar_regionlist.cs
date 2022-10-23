using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Ubbar
{
    /// <summary> خروجی تابع لیست مناطق 
    /// <para> success_flag وضعیت تابع صفر خطا یک اوکی</para>
    /// <para>warning_messages لیست هشدارها</para>
    /// <para>region_list لیست مناطق</para>
    /// 
    /// </summary>
    public class Result_Ubaar_regionlist
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailRegionList DetailRegionList { get; set; }

    }
    public class DetailRegionList
    {
        public int success_flag { get; set; }
        public List<warning_messages> warning_messages { get; set; }
        public List<region_list> region_list { get; set; }
    }
    /// <summary>region_list
    /// <para>region_name    نام منطقه</para>
    /// <para>region_city    نام شهر</para>
    /// <para>region_state   نام استان</para>
    /// <para>lat           </para>
    /// <para>lng           </para>
    /// <para>id            کد شهر</para>
    /// <para>north_east_lat</para>
    /// <para>north_east_lng</para>
    /// <para>south_west_lat</para>
    /// <para>south_west_lng</para>
    /// 
    /// </summary>
    public class region_list
    {
        public string region_name { get; set; }
        public string region_city { get; set; }
        public string region_state { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string id { get; set; }
        public string north_east_lat { get; set; }
        public string north_east_lng { get; set; }
        public string south_west_lat { get; set; }
        public string south_west_lng { get; set; }
    }

    /// <summary>warning_messages
    /// <para>warning_code کد هشدار</para>
    /// <para>warning_msg متن هشدار</para>
    /// </summary>
    public class warning_messages
    {
        public string warning_code { get; set; }
        public string warning_msg { get; set; }
    }
    public class error_messages
    {
        public string error_code { get; set; }
        public string error_msg { get; set; }
    }
}
