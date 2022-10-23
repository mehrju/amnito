using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IUserStatesService
    {
        bool Insert(int customerId, List<int> States, int countryId);
        List<int> getUserStates(int CustomerId);
        int GetIdUser(int CountryId);
    }
}
