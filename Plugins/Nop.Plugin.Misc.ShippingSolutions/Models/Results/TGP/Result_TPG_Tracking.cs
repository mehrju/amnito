using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Models.Results.TGP
{
    /// <summary>
    /// مدل خروجی متد پیگیری بارنامه 
    ///<para>CodeStatus کد خطای برگشتی</para>
    ///<para>Message متن برگشتی</para>
    /// <para>RootObject آبجکت خروجی</para>
    /// <para> RequestId کد پیگیری درخواست</para>
    /// </summary>
    public class Result_TPG_Tracking
    {
        public bool Status { get; set; }
        public int CodeStatus { get; set; }
        public string Message { get; set; }
        public string RequestId { get; set; }
        public RootObject RootObject { get; set; }
    }

    public class ReceiptInfo
    {
        public int ReceiptId { get; set; }
        public int SourceContactId { get; set; }
        public object SourceRegionId { get; set; }
        public int destinationregionid { get; set; }
        public int CrossingRegionId { get; set; }
        public object DestinationContactId { get; set; }
        public object ReceiptOperatorId { get; set; }
        public object AgentId { get; set; }
        public object ZoneId { get; set; }
        public object RegistrantId { get; set; }
        public object FareType { get; set; }
        public int ConsignmentNumber { get; set; }
        public object RefrenceId { get; set; }
        public object Status { get; set; }
        public object OrderId { get; set; }
        public object ServiceType { get; set; }
        public object PaymentType { get; set; }
        public object CashType { get; set; }
        public object TransportType { get; set; }
        public object TotalCost { get; set; }
        public object CalculatedCost { get; set; }
        public object ManualCost { get; set; }
        public object RegisterDate { get; set; }
        public object TotalFreightValue { get; set; }
        public object DocumentId { get; set; }
        public object ReferenceNumber { get; set; }
        public object RegistrantIp { get; set; }
        public object AppName { get; set; }
        public object Summery { get; set; }
        public object Note { get; set; }
        public object SysStartTime { get; set; }
        public object SysEndTime { get; set; }
        public int ItemCount { get; set; }
        public object CrossingPrefix { get; set; }
        public object SourcePrefix { get; set; }
        public object DestinationPrefix { get; set; }
        public object TransportTypeName { get; set; }
        public string SoureTownName { get; set; }
        public string SourceTownNameFa { get; set; }
        public string DestinationTownNameFa { get; set; }
        public object CrossingPrefixTownNameFa { get; set; }
        public object DestinationTownName { get; set; }
        public object ServiceName { get; set; }
        public object FareTypeName { get; set; }
        public object CashTypeName { get; set; }
        public string StatusName { get; set; }
        public DateTime DataReceiveDate { get; set; }
        public DateTime DataSentDate { get; set; }
        public object SourecContactNumber { get; set; }
        public object SourecContactAddress { get; set; }
        public object SourecContactName { get; set; }
        public object ReferenceName { get; set; }
        public string SourceTownNameEn { get; set; }
        public string DestinationTownNameEn { get; set; }
        public int SrcCountryId { get; set; }
        public int DstCountryId { get; set; }
        public string SoureCountryNameFA { get; set; }
        public string DestinationCountryNameFA { get; set; }
        public string SoureCountryNameEn { get; set; }
        public string DestinationCountryNameEn { get; set; }
        public object ReferenceDescription { get; set; }
        public object ContractId { get; set; }
        public object ProblemName { get; set; }
        public object PhysicalScan { get; set; }
        public object FreightTransportName { get; set; }
        public object FreightName { get; set; }
        public object ShipmentNo { get; set; }
        public object TurnName { get; set; }
    }

    public class Detail
    {
        public int ReceiptDetailsId { get; set; }
        public object ReceiptPartNo { get; set; }
        public object PartPathId { get; set; }
        public object RealWeight { get; set; }
        public object ManualWeigth { get; set; }
        public object Width { get; set; }
        public object Height { get; set; }
        public object Length { get; set; }
        public object Value { get; set; }
        public object TransportCost { get; set; }
        public object TransportType { get; set; }
        public object DocumentId { get; set; }
        public object ReceiptId { get; set; }
        public object IsDangerous { get; set; }
        public bool IsDocument { get; set; }
        public object SysStartTime { get; set; }
        public object SysEndTime { get; set; }
        public object ConsigmentTypeName { get; set; }
        public object ConsigmentType { get; set; }
    }

    public class ExtraCost
    {
        public int ReceiptExtraCostId { get; set; }
        public int ExtraCostId { get; set; }
        public int ExtraCostAmount { get; set; }
        public int ExtraCostReceiptId { get; set; }
        public int DocumentID { get; set; }
        public object ExtraCostDescription { get; set; }
        public int CostTypeId { get; set; }
        public object CostTypeName { get; set; }
        public object CostTypeNameFa { get; set; }
    }

    public class Sender
    {
        public int ContactId { get; set; }
        public object ContactNumber { get; set; }
        public object ContactType { get; set; }
        public object Code { get; set; }
        public object ComapnyName { get; set; }
        public object Name { get; set; }
        public object Family { get; set; }
        public object FatherName { get; set; }
        public object CertNo { get; set; }
        public object IdentityCode { get; set; }
        public object CompanyIdentityCode { get; set; }
        public object PostalCode { get; set; }
        public object Address { get; set; }
        public object ZoneId { get; set; }
        public object GeoId { get; set; }
        public object Fax { get; set; }
        public object Email { get; set; }
        public object Mobile { get; set; }
        public object VAT { get; set; }
        public object Telex { get; set; }
        public object Sign { get; set; }
        public object CertAttach { get; set; }
        public object IdentityAttach { get; set; }
        public object RefrenceContactId { get; set; }
        public int UserId { get; set; }
        public object ContractId { get; set; }
        public object GeoLat { get; set; }
        public object GeoLen { get; set; }
        public object ContactPriority { get; set; }
        public object CountryCode { get; set; }
        public object CityCode { get; set; }
        public int ContactMode { get; set; }
        public object TownId { get; set; }
        public object ContractCode { get; set; }
    }

    public class Receiver
    {
        public int ContactId { get; set; }
        public object ContactNumber { get; set; }
        public object ContactType { get; set; }
        public object Code { get; set; }
        public object ComapnyName { get; set; }
        public object Name { get; set; }
        public object Family { get; set; }
        public object FatherName { get; set; }
        public object CertNo { get; set; }
        public object IdentityCode { get; set; }
        public object CompanyIdentityCode { get; set; }
        public object PostalCode { get; set; }
        public object Address { get; set; }
        public object ZoneId { get; set; }
        public object GeoId { get; set; }
        public object Fax { get; set; }
        public object Email { get; set; }
        public object Mobile { get; set; }
        public object VAT { get; set; }
        public object Telex { get; set; }
        public object Sign { get; set; }
        public object CertAttach { get; set; }
        public object IdentityAttach { get; set; }
        public object RefrenceContactId { get; set; }
        public int UserId { get; set; }
        public object ContractId { get; set; }
        public object GeoLat { get; set; }
        public object GeoLen { get; set; }
        public object ContactPriority { get; set; }
        public object CountryCode { get; set; }
        public object CityCode { get; set; }
        public int ContactMode { get; set; }
        public object TownId { get; set; }
        public object ContractCode { get; set; }
    }

    public class CODInfo
    {
        public int CODEntryID { get; set; }
        public int ConsNumber { get; set; }
        public int CODCost { get; set; }
        public object DepositDate { get; set; }
        public object TrackingCode { get; set; }
        public object BankID { get; set; }
        public object IsDeposit { get; set; }
        public object EntryDate { get; set; }
        public object OperatorIP { get; set; }
        public int OperatorID { get; set; }
        public object CODStatus { get; set; }
        public object PayType { get; set; }
        public object Description { get; set; }
        public object ReceiptID { get; set; }
    }

    public class ContractInfo
    {
        public object ClientId { get; set; }
        public object ContractCode { get; set; }
        public object ContractEndDate { get; set; }
        public int ContractId { get; set; }
        public object ContractPayment { get; set; }
        public object ContractStatus { get; set; }
        public int ContractType { get; set; }
        public object ExactSettleDay { get; set; }
        public object IsDraft { get; set; }
        public object IsFormal { get; set; }
        public object SettleInterval { get; set; }
        public object PriceType { get; set; }
        public object AuthorityName { get; set; }
        public object ContractStartDate { get; set; }
        public object ContractTitle { get; set; }
        public object TarrifDescription { get; set; }
        public object TarrifName { get; set; }
    }

    public class RootObject
    {
        public ReceiptInfo ReceiptInfo { get; set; }
        public List<Detail> Details { get; set; }
        public List<ExtraCost> ExtraCosts { get; set; }
        public List<History> History { get; set; }
        public Sender Sender { get; set; }
        public Receiver Receiver { get; set; }
        public CODInfo CODInfo { get; set; }
        public object Sign { get; set; }
        public ContractInfo ContractInfo { get; set; }
        public object SignLink { get; set; }
        public object PickupSign { get; set; }
        public object PickupSignLink { get; set; }
        public object DeliverySign { get; set; }
        public object DeliverySignLink { get; set; }
        public object DocumentFileName { get; set; }
        public string PickupDocumentFileName { get; set; }
        public string DeliveryDocumentFileName { get; set; }
    }


    public class History
    {
        public int StatusLogId { get; set; }
        public object StatusReceiptId { get; set; }
        public object StatusDetailId { get; set; }
        public int Status { get; set; }
        public DateTime StatusDate { get; set; }
        public object DocumentID { get; set; }
        public int? TriggerManId { get; set; }
        public object DeliverManId { get; set; }
        public object TriggerLocationId { get; set; }
        public object StatusNote { get; set; }
        public object TriggerManIP { get; set; }
        public DateTime RecordDate { get; set; }
        public string TransfereeName { get; set; }
        public int OperatorId { get; set; }
        public int? DeliveryManId { get; set; }
        public object OperatorIP { get; set; }
        public int StatusId { get; set; }
        public string StatusPrefix { get; set; }
        public string StatusDescription { get; set; }
        public string OperatorName { get; set; }
        public object TriggerManName { get; set; }
        public object DeliveryManName { get; set; }
        public int IsInternal { get; set; }
        public string StatusDescriptionEN { get; set; }
        public object TriggerManCode { get; set; }
        public object DeliveryManCode { get; set; }
        public object HubName { get; set; }
    }
}
