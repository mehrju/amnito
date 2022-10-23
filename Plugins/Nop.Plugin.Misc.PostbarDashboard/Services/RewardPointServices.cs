using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public class RewardPointServices : IRewardPointServices
    {
        private readonly IRepository<RewardPointsHistory> _rphRepository;

        public RewardPointServices(IRepository<RewardPointsHistory> rphRepository)
        {
            _rphRepository = rphRepository;
        }

        public int GetRewardPointsCount(int customerId)
        {
            var query = _rphRepository.Table;
            if (customerId > 0)
            {
                query = query.Where(rph => rph.CustomerId == customerId);
            }
            return query.Count();
        }
    }
}