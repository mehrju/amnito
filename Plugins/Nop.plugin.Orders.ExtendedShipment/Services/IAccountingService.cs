using Nop.Core.Domain.Shipping;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IAccountingService
    {
        void calcAffilateCharge(Order order);
        void InsertChargeWallethistory(ChargeWalletHistoryModel model);
        void RecoverAgentAmountMonyForShipment(Shipment shipment);
        bool HasChargeWallethistory(int ChargeWalleTypeId, int orderId);
        bool IsChargedWalletForAgentAMountRule(int orderItem, int ChargeWalletType);
        void refoundAffilateCharge(Order order);
        int ChargeWalletForAgentSaleAmount(Order order, int price, string inputMsg, out string Msg);
        List<SettlementListInfo> SettlementList(int FileId
           , string UserName
           , int OrderId
           , DateTime? DepositDateFrom
           , DateTime? DepositDateTo
           , DateTime? SettlementDateFrom
           , DateTime? SettlementDateTo
           , int PageIndex
           , int PageSize
            , ref int PageCount
           );
    }
}
