using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    public class PaterPricingPolicyService: IPaterPricingPolicyService
    {
        private readonly IDbContext _dbContext;
        public PaterPricingPolicyService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<SelectListItem> getActiveService()
        {
            string query = $@"select
	                        cast(C.Id as varchar(10)) Value,
	                        C.Name as Text
                        from
	                        Category C
	                        inner join Tb_CategoryInfo CI on C.Id=CI.CategoryId";
           return _dbContext.SqlQuery<SelectListItem>(query, new object[0]).ToList();
        }
    }
}
