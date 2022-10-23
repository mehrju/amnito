using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.Snappbox
{
    /// <summary>
    /// مدل خروجی متد جزییات و پیگیری سفارش
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>OrderDetails آبجکت خروجی</para>
    /// </summary>
    public class Result_Snappbox_Get_Order_Detail
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public OrderDetails_Get_order_Detail orderDetails { get; set; }
    }

    public class Terminal
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
        public string arrivedPickupAt { get; set; }
        public string paymentType { get; set; }
        public double cashOnPickup { get; set; }
        public double cashOnDelivery { get; set; }
        public List<object> pickedUpItems { get; set; }
        public List<object> droppedOffItems { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool isPackageReturnPickup { get; set; }
    }

    public class Order
    {
        public int customerId { get; set; }
        public string customerName { get; set; }
        public string status { get; set; }
        public double bikerDeliveryFare { get; set; }
        public double bikerDeliveryFareAfterCommission { get; set; }
        public double customerDeliveryFare { get; set; }
        public bool isForceAllocated { get; set; }
        public bool isCancellable { get; set; }
        public bool isDeniable { get; set; }
        public string deliveryCategory { get; set; }
        public bool isReturn { get; set; }
        public List<object> items { get; set; }
        public List<object> allotments { get; set; }
        public List<Terminal> terminals { get; set; }
        public string createdAt { get; set; }
        public string supportNumber { get; set; }
        public int waitingTime { get; set; }
        public bool batchable { get; set; }
        public bool cancellable { get; set; }
        public bool forceAllocated { get; set; }
        public bool deniable { get; set; }
        public bool @return { get; set; }
    }

    public class Location
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }

    public class Allotment
    {
        public int id { get; set; }
        public int bikerId { get; set; }
        public string bikerName { get; set; }
        public string bikerPhoneNumber { get; set; }
        public string vehiclePlateNumber { get; set; }
        public string bikerImageUrl { get; set; }
        public string status { get; set; }
        public bool current { get; set; }
        public string acceptedAt { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public Order order { get; set; }
        public Location location { get; set; }
    }

    public class Terminal2
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
        public string arrivedPickupAt { get; set; }
        public string paymentType { get; set; }
        public double cashOnPickup { get; set; }
        public double cashOnDelivery { get; set; }
        public List<object> pickedUpItems { get; set; }
        public List<object> droppedOffItems { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public bool isPackageReturnPickup { get; set; }
    }

    public class OrderDetails
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
        public List<Allotment> allotments { get; set; }
        public List<Terminal2> terminals { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string supportNumber { get; set; }
        public int waitingTime { get; set; }
        public int allocationTtl { get; set; }
        public List<object> items { get; set; }
        public List<object> feedback { get; set; }
        public List<object> orderTakeIts { get; set; }
        public int id { get; set; }
    }
    public class OrderDetails_Get_order_Detail
    {
        public OrderDetails orderDetails { get; set; }
    }
}
