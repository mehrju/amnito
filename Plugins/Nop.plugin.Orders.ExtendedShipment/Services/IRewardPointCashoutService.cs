using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
	public interface IRewardPointCashoutService
	{
		List<ChargeWalletHistoryResult> GetCustomerWalletHistory(DateTime? DateFrom, DateTime? DateTo,long? UserId, string chargeWalletType, bool? Paid, int PageSize, int PageIndex, out int count);
		List<RewardPointsCashoutModel> getRewardPointCashout(DateTime? DateFrom, DateTime? DateTo,string customers, string chargeWalletType, string excelfilename, int PageSize, int PageIndex, out int count);

		string saveRewardPointCashout(string jsonTableExcel);

		List<SelectListItem> getchargeWalletTypes();
		List<SelectListItem> getcustomers(string searchtext);
        string RequestRewardPointCashout(string chargeWalletIds, bool? CashoutTypeId, string ShebaToInsert, long UserId);
        string SaveSettingCashout(string jsonSettings, long UserId);
		T[] execSpNormal<T>(string SpName, object obj);
		T[] execSpNormal<T>(string SpName, object obj, object objectOut);
		T[] execSp<T>(string SpName, object obj);
		T[] execSp<T>(string SpName, object obj, out string jsonout);
	}
}
