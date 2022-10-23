using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox
{
    /// <summary>
    /// مدل خروچی لیست سفارشات
    /// <para> Status وضعیت متد</para>
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    ///<para>DetailResult_Snappbox_Get_Order_List  مدل خروجی متد </para>
    /// </summary>
    public class Result_Snappbox_Get_Order_List
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public DetailResult_Snappbox_Get_Order_List DetailResult_Snappbox_Get_Order_List { get; set; }
    }

    public class DetailResult_Snappbox_Get_Order_List
    {
        public List<OrderList> orderList { get; set; }
        public int totalPageCount { get; set; }
        public int pageNumber { get; set; }
        public int totalCount { get; set; }
    }



    public class TerminalResult_Snappbox_Get_Order_List
    {
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string unit { get; set; }
        public string address { get; set; }
        public string plate { get; set; }
        public string comment { get; set; }
        public string contactName { get; set; }
        public string contactPhoneNumber { get; set; }
        public int sequenceNumber { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public string paymentType { get; set; }
        public double cashOnPickup { get; set; }
        public double cashOnDelivery { get; set; }
        public List<object> pickedUpItems { get; set; }
        public List<object> droppedOffItems { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool? isPackageReturnPickup { get; set; }
    }

    public class OrderList
    {
        public int customerId { get; set; }
        public string customerName { get; set; }
        public string customerPhonenumber { get; set; }
        public string customerRefId { get; set; }
        public string customerEmail { get; set; }
        public string status { get; set; }
        public string cancelledBy { get; set; }
        public string cancelledAt { get; set; }
        public double bikerDeliveryFare { get; set; }
        public double bikerDeliveryFareAfterCommission { get; set; }
        public double customerDeliveryFare { get; set; }
        public bool isForceAllocated { get; set; }
        public string deliveryFarePaymentType { get; set; }
        public int customerWalletId { get; set; }
        public int sequenceNumberDeliveryCollection { get; set; }
        public bool isCancellable { get; set; }
        public bool isDeniable { get; set; }
        public int orderConfigId { get; set; }
        public string city { get; set; }
        public string deliveryCategory { get; set; }
        public bool isReturn { get; set; }
        public List<object> allotments { get; set; }
        public List<TerminalResult_Snappbox_Get_Order_List> terminals { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool isOrderRevert { get; set; }
        public int waitingTime { get; set; }
        public int allocationTtl { get; set; }
        public List<object> items { get; set; }
        public List<object> feedback { get; set; }
        public List<object> orderTakeIts { get; set; }
        public int id { get; set; }
    }


}
