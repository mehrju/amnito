using Nop.Core;
using Nop.Data;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly IStoreContext _storeContext;
        private readonly IDbContext _dbContext;

        public ServiceTypeService(IStoreContext storeContext,IDbContext dbContext)
        {
            _storeContext = storeContext;
            _dbContext = dbContext;
        }

        public IList<ServiceTypeModel> GetServiceTypesByCurrentStoreId()
        {
            var storeId = _storeContext.CurrentStore.Id;
            string query = $@"
                SELECT
                    C.Id,
                    C.Name
                FROM
                    dbo.Category AS C
                    INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = C.Id
                WHERE
                    c.Published = 1
                    AND c.Deleted = 0
	                AND ({storeId} = 5 OR ({storeId} = 3 AND C.Id IN (654,655,667,670,660,661,662,690,691,692,693,694,695,696,697,698)))";

            return _dbContext.SqlQuery<ServiceTypeModel>(query).ToList();
        }
    }
}
