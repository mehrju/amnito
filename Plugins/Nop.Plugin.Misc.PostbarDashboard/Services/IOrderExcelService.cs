using Nop.Plugin.Misc.PostbarDashboard.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public interface IOrderExcelService
    {
        void GetOrdersExcel(IList<OrderModel> orders, OrderSearchCondition searchCondition, Stream stream);
        void GetOrderBillDetailExcel(IList<OrderBillDetail> orderBillDetails, OrderSearchCondition searchCondition, Stream stream);
        void GetOrderBarcodeExcel(IList<OrderTrackingBarcode> orders, OrderSearchCondition searchCondition, Stream stream);
        void WalletIncomeForServicesMiniAdminExcel(IList<WalletIncomeForServicesMiniAdminResult> walletIncomes, WalletIncomeMiniAdminInput searchCondition, Stream stream);
        void WalletIncomeDetailsMiniAdminExcel(IList<WalletIncomeDetailsMiniAdminResult> walletIncomes, WalletIncomeMiniAdminInput searchCondition, Stream stream);
        void ReportWithTrackingCodeMiniAdminExcel(IList<WalletIncomeMiniAdminInput> walletIncomes, OrderSearchCondition searchCondition, Stream stream);
        void GetWalletExcel(IList<WalletRecord> Walletresords, Stream stream);
    }
}
