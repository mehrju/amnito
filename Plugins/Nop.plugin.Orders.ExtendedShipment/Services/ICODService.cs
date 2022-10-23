using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.plugin.Orders.ExtendedShipment.CODServiceRefrence;
using Nop.plugin.Orders.ExtendedShipment.Models;
using static Nop.plugin.Orders.ExtendedShipment.Services.CodService;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface ICodService
    {
        List<ShopInfo> GetShopList();
        ShopResisterResult RegisterGatewayShop(Shop model);
        string GetPrice(int Weight, int Price, string Shcode, int State, int City, int Tip, int Cod, int Showtype);
        string NewOrder2(string EncodedOrderDetailes, string Ordertip);
        bool ShipmentChangeStatus(int Status, string trackingNumber, out string result);
        string ChangeStatus(string trackingNumber, int Status);
        string GetStatus(string trackingNumber);
        List<SelectListItem> getCODEventList();
        /// <summary>
        ///  تغییر وضعیت مرسوله پس کرایه
        /// </summary>
        /// <param name="Status">
        /// 1= معلق در فروشگاه
        /// 2= آماده قبول
        /// </param>
        /// <param name="trackingNUmber"></param>
        /// <returns></returns>
        bool ChangeStatus(int Status, string trackingNumber, out string result);
        /// <summary>
        /// گرفتن آخرین وضعیت به همراه تاریخ آن
        /// </summary>
        /// <param name="trackingNumber"></param>
        /// <returns></returns>
        ShipmentTrackingModel GetStatus(string trackingNumber, out string result);
        /// <summary>
        /// دریافت کد های واریزی در یک بازه زمانی
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        List<CodShipmentFinancialModel> getBilling(string startDate, string endDate);
        /// <summary>
        ///  گرفتن ریز واریزی ها براساس کد واریزی
        /// </summary>
        /// <param name="settleCode">کد واریز</param>
        /// <param name="tip">نوع جزئیات عددي ما بین 1 الی 6 </param>
        /// <param name="PageNo">صفحه مرسولاتی که قصد مشاهده دارید از 1 الی ...</param>
        List<CodShipmentFinancialModel> getBillingDetailes(string settleCode, DateTime VarizDate, WebService1SoapClient CODSoapClient = null);

        bool ChargeMoneyBagForCodGood(int shipmentId, int price, out string Msg);
        List<string> GetTrackingNumbersByUniqueReferenceNo(string UniqueReferenceNo);
    }
}
