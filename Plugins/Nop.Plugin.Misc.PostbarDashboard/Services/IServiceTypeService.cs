using Nop.Plugin.Misc.PostbarDashboard.Models;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public interface IServiceTypeService
    {
        IList<ServiceTypeModel> GetServiceTypesByCurrentStoreId();
    }
}