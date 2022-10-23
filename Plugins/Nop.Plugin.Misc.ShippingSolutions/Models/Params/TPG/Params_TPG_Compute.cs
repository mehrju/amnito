using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.TPG
{
    /// <summary>
    /// مدل ورودی استعلام قیمت
    /// شناسه شهر مبدا و مقصد طبق اکسل ارسالی
    /// باتوحه به داکیومنت همه موارد اجباری هستند
    /// <para>باتوجه به داکیومنت یک سری از فیلدها موراد ثابت فعلا باید بفرستیم که در ادامه نام میبرم</para>
    /// <para>cn=0</para>
    /// <para>service=6</para>
    /// <para>currency=1</para>
    /// <para>userCost =0</para>
    /// <para>priority=false</para>
    /// <para>km=0</para>
    /// <para>isPas=false</para>
    /// <para>contact=-1</para>
    /// <para>2 فیلد زیر از تنظمیات تکمل خواهد شد</para>
    /// <para>ContractCode</para>
    /// <para>ContractId</para>
    /// <para>دیگر پارامترها که باید تکمیل گردد</para>
    /// <para>weight وزن </para>
    /// <para>source شناسه مبدا</para>
    /// <para>destination شناسه مقصد</para>
    /// <para>hasInsurance وضعیت بیمه</para>
    /// <para>Length طول</para>
    /// <para>Width عرض</para>
    /// <para>Height ارتفاع</para>
    /// <para>name نام کالا</para>
    /// </summary>
    public class Params_TPG_Compute
    {
        public Params_TPG_Compute()
        {
            cn = 0;
           // service = 6; 6= عادی , 
           // 7 = COD
            currency = 1;
            userCost = 0;
            isPas = false;
            km = 0;
            priority = false;
            contact = -1;
        }
        public int Client { get; set; }
        public int cn { get; set; }
        public string weight { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public string destinationarea { get; set; }
        public int service { get; set; }
        public int currency { get; set; }
        public bool hasInsurance { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int userCost { get; set; }
        public string ContractCode { get; set; }
        public int ContractId { get; set; }
        public string name { get; set; }
        public int contact { get; set; }
        public bool isPas { get; set; }
        public int km { get; set; }
        public bool priority { get; set; }

    }
}
