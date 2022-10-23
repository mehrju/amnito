using System;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IChargeWalletFailService
    {
        void InsertFailedLog(Exception ex, object detail, string message);
    }
}