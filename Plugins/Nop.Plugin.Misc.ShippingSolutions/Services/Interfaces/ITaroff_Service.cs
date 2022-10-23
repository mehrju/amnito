using Nop.Plugin.Misc.ShippingSolutions.Models.Params.Taroff;
using Nop.Plugin.Misc.ShippingSolutions.Models.Results.Taroff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    /// <summary>
    /// سرویس تعارف
    /// <para>GetProvinces متد دریافت لیست استان هاو مناطق پستی تهران</para>
    /// <para>GetCity متد دریافت لیست شهرهای یک استان</para>
    /// <para>GetListPaymentMethods متد دریافت لیست روش های پرداخت</para>
    /// <para>GetListCarriers متد دریافت لیست روش  های حمل(پیشتاز و...)</para>
    /// <para>CreateOrder متد ثبت سفارش</para>
    /// <para>GetStateOrder متد پیگیری سفارش</para>
    /// <para>SetStateReady متد نهایی کردن سفارش یااعلام امادگی</para>
    /// <para>SetStateCancel متد کنسل کردن سفارش</para>
    /// <para>تنظیمات و.... اعم از توکن ذخیره شود</para>
    /// 
    /// </summary>
    public interface ITaroff_Service
    {
        Result_Taroff_GetProvinces GetProvinces();
        Result_Taroff_GetCity GetCity(Params_Taroff_GetCity param);
        Result_Taroff_GetListPaymentMethods GetListPaymentMethods();
        Result_Taroff_GetListCarriers GetListCarriers();
       Task< Result_Taroff_CreateOrder> CreateOrder(Params_Taroff_CreateOrder param);
        Result_Taroff_GetStateOrder GetStateOrder(Params_Taroff_GetStateOrder param);
        Result_Taroff_SetStateReady SetStateReady(Params_Taroff_SetStateReady param);
        Result_Taroff_SetStateCancel SetStateCancel(Params_Taroff_SetStateCancel param);
    }
}
