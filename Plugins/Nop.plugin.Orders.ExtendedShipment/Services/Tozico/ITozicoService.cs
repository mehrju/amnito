using Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input;
using Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services.Tozico
{
    public interface ITozicoService
    {
        //Task<OptimizationRequestOutput> OptimizationRequest(OptimizationRequestInput input);
        //Task<T> SendResuest<T>(HttpMethod httpMethod, string route, object postParams);
        TozicoResult AddOrUpdateBranches(List<Branch> branches);
        TozicoResult AddOrUpdateVehicles(List<Vehicle> vehicles);
        TozicoResult AddOrUpdateCustomers(List<TozicoCustomer> customers);
        TozicoTokenResult getLoginToken(int customerId);
    }
}
