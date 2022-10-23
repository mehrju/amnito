using System.Collections.Generic;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
    public class DashboardModel
    {
        public int TotalOrdersCount { get; set; }

        public int RewardPointsBalanced { get; set; }

        public int CountTicket { get; set; }
        public string DepositCode { get; set; }
        public OrderModel LastOrder { get; set; }
    }

    public class DashboardInfoInput
    {

    }
    public class DashboardInfoResult
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLastName { get; set; }
        public string AffiliateId { get; set; }
        public string YourAffiliateId { get; set; }
        public long CustomerOrderSum { get; set; }
        public int AffiliatedCount { get; set; }
        public int UsedAffiliatedCount { get; set; }
        public int EarnedPrice { get; set; }
        public int Balance { get; set; }
        public string jsonsettings { get; set; }

    }

    public class DashboardSettingInfoResult
    {
        public bool ReciveSmsPersuit { get; set; }
        public bool ReciveOrderEmail { get; set; }
        public bool AccessToPrinter { get; set; }
        public bool AccessToPackage { get; set; }
        public bool UseLogo { get; set; }

    }

    public class DashboardAddressResult
    {
        public long Id { get; set; }
        public int AddressId { get; set; }
        public string Address1 { get; set; }
    }
    public class DashboardOrdersSearch
    {
        public string orderDateFrom { get; set; }
        public string orderDateTo { get; set; }
        public string reciverName { get; set; }
        public string senderName { get; set; }
        public int? reciverProvinceId { get; set; }
        public int? reciverStateId { get; set; }
        public int? senderProvinceId { get; set; }
        public int? senderStateId { get; set; }
        public int? weight { get; set; }
        public int? PaymentStatus { get; set; }
    }
    public class DashboardOrdersResult
    {
        public int? Id { get; set; }
        public string ServiceName { get; set; }
        public string Price { get; set; }
        public string Date { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
    }
    public class DashboardMessagesResult
    {
        public long Id { get; set; }
        public int? MessageId { get; set; }
        public string Subject { get; set; }
        public string ShamsiCreatedDate { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; }
        public int PriorityLevel { get; set; }
    }
    public class DashboardFactorRequestResult
    {
        public long Id { get; set; }
        public string ShamsiCreatedDate { get; set; }
        public string ShamsiDateFrom { get; set; }
        public string ShamsiDateTo { get; set; }
        public string ConfirmState { get; set; }
        public string SafeFileName { get; set; }

    }
    public class DashboardFactorRequestModel
    {
        public string DateFrom2 { get; set; }
        public string DateTo2 { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string EconomicCode { get; set; }
        public string ForiegnCode { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public string Operation { get; set; }
        public string ZipPostalCode { get; set; }


    }
    public class DashboardFactorRequestMRTModel
    {
        public string SenderNationalCode { get; set; }
        public string SenderFullName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderPostalCode { get; set; }
        public string SenderTel { get; set; }
        public int OrderId { get; set; }
        public int Id { get; set; }
        public int CodBmValue { get; set; }
        public int CodCost { get; set; }
        public string OrderDate { get; set; }
        public string EconomicCode { get; set; }

    }

    public class WalletIncomeMiniAdminInput
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Username { get; set; }
    }
    public class WalletIncomeForServicesMiniAdminResult
    {
        //شماره سفارش
        public int Id { get; set; }

        //مبلغ واریزی بابت خدمات نمایندگی
        public int Points { get; set; }

        //تاریخ سفارش
        public string ShamsiDate { get; set; }

        //نام کاربری
        public string Username { get; set; }

    }

    public class WalletIncomeDetailsMiniAdminResult
    {
        //[شماره سفارش]
        public int OrderId { get; set; }

        //[شماره آیتم سفارش]
        public int OrderItemId { get; set; }

        //[نوع سرویس]
        public string ProductName { get; set; }

        //[رده وزن]
        public string PropertyAttrValueName { get; set; }

        //[تعداد]
        public int Quantity { get; set; }

        //[تخفیف اعمال شده هر در واحد]
        public int DiscountPerUnit { get; set; }

        //[مجموع تخفیف آیتم سفارش]
        public int TotalDiscount { get; set; }

        //[مبلغ واریزی بابت تخفیف]
        public int Points { get; set; }

        //[تخفیف جاری هر در واحد]
        public int CurrentDiscount { get; set; }

        //[تاریخ سفارش]
        public string ShamsiDate { get; set; }
    }
}