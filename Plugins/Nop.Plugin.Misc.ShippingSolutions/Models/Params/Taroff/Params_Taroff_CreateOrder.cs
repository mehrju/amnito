using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Params.Taroff
{
    /// <summary>
    /// مدل ورودی ثبت سفارش
    /// <para>توضیح بعضی از فیلد ها</para>
    /// <para>Code کد یکتای محلی </para>
    /// <para>DeliverTime زمان جمع اوری صب یا بعدازظهر</para>
    /// <para>CarrierId شناسه نوع حامل که جداگانه باید خوانده شود</para>
    /// <para>PaymentMethodId شناسه نوع پرداخت که باید جداگانه از سرویس خوانده شود</para>
    /// <para>CityId شناسه شهر مقصد که باید جداگانه خوانده شود</para>
    /// <para>TotalWeight وزن کل</para>
    /// <para>TotalPrice ارزش کلی محموله</para>
    /// <para>موارد اختیاری </para>
    /// <para>PostCode</para>
    /// <para>Code</para>
    /// <para>Note</para>
    /// <para>Email</para>
    /// </summary>
    public class Params_Taroff_CreateOrder
    {
        public string Token              { get; set; }
        public string FirstName          { get; set; }
        public string LastName           { get; set; }
        public string Address           { get; set; }
        public string PostCode          { get; set; }
        public string Code             { get; set; }
        public string DeliverTime      { get; set; }
        public string Note             { get; set; }
        public string Email           { get; set; }
        public string Mobile          { get; set; }
        public int    CarrierId       { get; set; }
        public int    PaymentMethodId { get; set; }
        public int    CityId          { get; set; }
        public string ProductTitles   { get; set; }
        public int    TotalWeight     { get; set; }
        public int    TotalPrice      { get; set; }





        public (bool, string) IsValidParams_Taroff_CreateOrder()
        {
            bool Status = true;
            string Message = "";
            if (string.IsNullOrEmpty((FirstName ?? "").Trim()))
            {
                Status = false;
                Message = "(Taroff_CreateOrder_FirstName) is null";
                
            }
            if (string.IsNullOrEmpty((LastName ?? "").Trim()))
            {
                Status = false;
                Message = "(Taroff_CreateOrder_LastName) is null";

            }
            if (string.IsNullOrEmpty((Address ?? "").Trim()))
            {
                Status = false;
                Message = "(Taroff_CreateOrder_Address) is null";

            }
            if (string.IsNullOrEmpty((DeliverTime ?? "").Trim()))
            {
                Status = false;
                Message = "(Taroff_CreateOrder_DeliverTime) is null";

            }
            else
            {
                if(DeliverTime!="" && DeliverTime != "")
                {
                    Status = false;
                    Message = "(DeliverTime) The value must be am  or pm";
                }
            }
            if (string.IsNullOrEmpty((Mobile ?? "").Trim()))
            {
                Status = false;
                Message = "(Taroff_CreateOrder_Mobile) is null";

            }
            if (string.IsNullOrEmpty((ProductTitles ?? "").Trim()))
            {
                Status = false;
                Message = "(Taroff_CreateOrder_ProductTitles) is null";

            }
            if (TotalPrice <= 0)
            {
                Status = false;
                Message = "(Taroff_CreateOrder_TotalPrice) The value must be greater than zero";
            }
            if (CityId <= 0)
            {
                Status = false;
                Message = "(Taroff_CreateOrder_CityId) The value must be greater than zero";
            }
            if (PaymentMethodId <= 0)
            {
                Status = false;
                Message = "(Taroff_CreateOrder_PaymentMethodId) The value must be greater than zero";
            }
            if (CarrierId <= 0)
            {
                Status = false;
                Message = "(Taroff_CreateOrder_CarrierId) The value must be greater than zero";
            }
            if (TotalWeight <= 0)
            {
                Status = false;
                Message = "(Taroff_CreateOrder_TotalWeight) The value must be greater than zero";
            }

            return (Status, Message);
        }
    }
}
