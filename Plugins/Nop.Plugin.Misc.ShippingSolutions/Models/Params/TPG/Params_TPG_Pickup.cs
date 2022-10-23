using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.TPG
{/// <summary>
/// مدل ورودی متد PICKUP
/// <para>فقط موارد ذیل تکمیل گردد</para>
/// <para>Desc توضیحات اختیاری</para>
/// <para>Lat اختیاری</para>
/// <para>Longt اختیاری</para>
/// <para>Name اجباری</para>
/// <para>Family اجباری</para>
/// <para>CompanyName اجباری</para>
/// <para>SubregionId شهر میدا اختیاری  </para>
/// <para>Address ادرس</para>
/// <para>Weight اجباری گرم</para>
/// <para>DstId شناسه مقصد</para>
/// <para>SrcId شناسه مبدا</para>
/// <para>DstAddress ادرس مقصد</para>
/// <para>SenderName نام فرسنده</para>
/// <para>Length اختیاری</para>
/// <para>Heightاختیاری</para>
/// <para>Width اختیاری</para>
/// 
/// </summary>
    public class Params_TPG_Pickup
    {
        public Params_TPG_Pickup()
        {
            appid = 1;
            UserIP = GetLocalIPAddress();
            Client = 0;
            ContactId = -1;
            CoordDate = DateTime.Now;
            PackingCost = 0;
            InsuranceCost = 0;
            ManualPrice = 0;
            PostalCode = "0";
            ConsigmentType = 0;
            NeedPacking = 0;
            InsuranceType = 0;
            FreightValue = 0;
            ServiceId = 6;
            VehicleType = 1;
            Count = 1;
            HSCID = 3;
            PickupManId = 0;
            UseCopy = false;
            PayType = 1;
            BeingSchedule = false;
            InterVal = 0;
            InterValType = 0;
            StartDateTime = DateTime.Now;
            EndDateTime = DateTime.Now;
            OccureDateTime = DateTime.Now;
            Saturday = false;
            Sunday = false;
            Monday = false;
            Tuesday = false;
            Wednesday = false;
            Thursday = false;
            Friday = false;
            ScheduleId = 0;
            Area = 0;
            DestArea = 0;
            Plaque = 1;
            IsPostPaid = false;
            resultCode = 0;
            resultID = 0;
            resultCN = 0;
            resultCost = 0;
            BuildReceipt = true;
            TaxiPrice = 0;
            Identity = true;
            DisableEnforce = false;
            CODCost = 0;
        }
        public int appid { get; set; }
        public int UserId { get; set; }
        public string UserIP { get; set; }
        public int Client { get; set; }
        public int ContactId { get; set; }
        public int ContractId { get; set; }
        public DateTime CoordDate { get; set; }
        public string Desc { get; set; }
        public double Lat { get; set; }
        public double Longt { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string CompanyName { get; set; }
        public int PackingCost { get; set; }
        public int InsuranceCost { get; set; }
        public int ManualPrice { get; set; }
        public string PostalCode { get; set; }
        public int SubregionId { get; set; }
        public string Address { get; set; }
        public int ConsigmentType { get; set; }
        public int NeedPacking { get; set; }
        public int Weight { get; set; }
        public int Length { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int InsuranceType { get; set; }
        public int FreightValue { get; set; }
        public int DstId { get; set; }
        public int SrcId { get; set; }
        public int ServiceId { get; set; }
        public string DstAddress { get; set; }
        public int VehicleType { get; set; }
        public int Count { get; set; }
        public int HSCID { get; set; }
        public int PickupManId { get; set; }
        public bool UseCopy { get; set; }
        public int PayType { get; set; }
        public bool BeingSchedule { get; set; }
        public int InterVal { get; set; }
        public int InterValType { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime OccureDateTime { get; set; }
        public string SenderName { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public int ScheduleId { get; set; }
        public int Area { get; set; }
        public int DestArea { get; set; }
        public int DestContactId { get; set; }
        public int Plaque { get; set; }
        public bool IsPostPaid { get; set; }
        public int resultCode { get; set; }
        public int resultID { get; set; }
        public int resultCN { get; set; }
        public double resultCost { get; set; }
        public bool BuildReceipt { get; set; }
        public int TaxiPrice { get; set; }
        public bool Identity { get; set; }
        public bool DisableEnforce { get; set; }
        public int CODCost { get; set; }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
