using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public interface IRewardPointServices
    {
        int GetRewardPointsCount(int customerId);
    }
}
