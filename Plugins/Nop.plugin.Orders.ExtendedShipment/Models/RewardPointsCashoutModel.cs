using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
	public class ChargeWalletHistoryResult
	{
		public long Id { get; set; }
		public int? orderId { get; set; }
		public string ChargeWalletTypeName { get; set; }
		public int? Point { get; set; }
		public string xShamsiDate { get; set; }
        public string xShamsiPayDate { get; set; }
        public string xShamsiRequestPayDate { get; set; }
        public string Description { get; set; }

	}
	public class RewardPointsCashoutModel
	{
		public int? Id { get; set; }
		public decimal TotalValue { get; set; }
		public string Message { get; set; }
		public decimal? RewardPointBalance { get; set; }
		public string FactorNumber { get; set; }
        public string Sheba { get; set; }
        public string CustomerName { get; set; }
    }
    public class ExcelImportModel
    {
        public string Transfer_Amount { get; set; }
        public string Sheba { get; set; }
        public string Description { get; set; }
        public string Factor_Number { get; set; }
    }
    public class RewardPointsCashoutInputModel
	{
		public RewardPointsCashoutInputModel()
		{

		}
		[DisplayName("تاریخ از")]
		[UIHint("DateNullable")]
		public DateTime? dateFrom { get; set; }
		[DisplayName("تاریخ تا")]
		[UIHint("DateNullable")]
		public DateTime? dateTo { get; set; }
		[DisplayName("نوع تراکنش")]
		public string chargeWalletType { get; set; }
		[DisplayName("مشتری")]
		public string customers { get; set; }
		[DisplayName("پرداخت شده")]
		public bool? paid { get; set; }
        public string date1 { get; set; }
        public string date2 { get; set; }


    }

	public class FactorRequestModel
	{
		public int? requestFactorId { get; set; }
		[DisplayName("مشتری")]
		public string customers { get; set; }
		[DisplayName("تاریخ از")]
		[UIHint("DateNullable")]
		public string dateFrom { get; set; }
		[DisplayName("تاریخ تا")]
		[UIHint("DateNullable")]
		public string dateTo { get; set; }

	}
	public class AdminSendMessageModel
	{
		public string customers { get; set; }
		public string roles { get; set; }
		public string message { get; set; }
		public string subject { get; set; }

	}
	public class AdminMessageModelResult
	{
		public string customerName { get; set; }
		public string state { get; set; }
		public string message { get; set; }
		public string subject { get; set; }
		public string shamsiDate { get; set; }
	}
	public class AdminSearchMessageModel
    {
		[DisplayName("مشتری")]
		public string customers2 { get; set; }
		[DisplayName("نقش ها")]
		public string roles2 { get; set; }
		[DisplayName("تاریخ از")]
		[UIHint("DateNullable")]
		public string dateFrom { get; set; }
		[DisplayName("تاریخ تا")]
		[UIHint("DateNullable")]
		public string dateTo { get; set; }
	}
	public class FactorRequestResult
	{
		public long Id { get; set; }
		public string ShamsiCreatedDate { get; set; }
		public string ShamsiDateFrom { get; set; }
		public string ShamsiDateTo { get; set; }
		public string CustomerName { get; set; }
		public string ConfirmState { get; set; }
		public string SafeFileName { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Address { get; set; }
		public string ZipPostalCode { get; set; }
		public string ForiegnCode { get; set; }
		public string NationalCode { get; set; }
		public string EconomicCode { get; set; }

	}
}
