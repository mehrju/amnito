using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IContractService
    {
        ContractItems GetLastContract(int CustomerId);
        int saveContract(ContractItems item, out string _error);
    }
}
