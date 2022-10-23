using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services.Collecting
{
    public class CollectingService:ICollectingService
    {
        private readonly IDbContext _dbContext;
        public CollectingService(IDbContext dbContext)
        {
            _dbContext= dbContext;
        }
        
    }
}
