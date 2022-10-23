using Nop.Core.Data;
using Nop.Core.Domain.Directory;
using Nop.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    public class CheckRegionDesVijePost : ICheckRegionDesVijePost
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<Tbl_RelationOstanCityVijePost> _repositoryTbl_RelationOstanCityVijePost;
        public readonly IRepository<StateProvince> _repositoryStateProvince;
        private readonly ICategoryService _categoryService;
        public CheckRegionDesVijePost(IDbContext dbContext
            , IRepository<Tbl_RelationOstanCityVijePost> repositoryTbl_RelationOstanCityVijePost
            , IRepository<StateProvince> repositoryStateProvince
            , ICategoryService categoryService)
        {
            _categoryService = categoryService;
            _dbContext = dbContext;
            _repositoryTbl_RelationOstanCityVijePost = repositoryTbl_RelationOstanCityVijePost;
            _repositoryStateProvince = repositoryStateProvince;
        }

        public bool CheckValidSourceDistination(int SenderCountrId, int SenderStateId, int ReciverCountrId, int ReciverStateId)
        {
            if (_repositoryTbl_RelationOstanCityVijePost.Table.Any(p => p.IdCountryDes == SenderCountrId && p.IdCity == SenderStateId))
            {
                return _repositoryTbl_RelationOstanCityVijePost.Table.Where(p => p.IdCountryRegion == SenderCountrId && p.IdCity == ReciverStateId).Any();
            }
            return false;
        }

        public void fill()
        {
            List<Tbl_RelationOstanCityVijePost> df = _repositoryTbl_RelationOstanCityVijePost.Table.ToList();
            foreach (var item in df)
            {
                var StateProvince = _repositoryStateProvince.Table.Where(p => p.CountryId == item.IdCountryDes && p.Name == item.Name).FirstOrDefault();
                if (StateProvince != null)
                {
                    item.IdCity = StateProvince.Id;
                    _repositoryTbl_RelationOstanCityVijePost.Update(item);
                }
            }
        }
        public bool CheckValidSourceForInternationalPost(int SenderCountrId, int SenderStateId)
        {
            if (SenderCountrId == 1)
                return true;
            var cat = _categoryService.GetCategoryById(662);
            if (cat.Published)
                return _repositoryTbl_RelationOstanCityVijePost.Table.Any(p => p.IdCountryDes == SenderCountrId && p.IdCity == SenderStateId);
            var cat2 = _categoryService.GetCategoryById(714);
            if (cat2.Published)
            {
                string query = $@"IF EXISTS(
                            SELECT
	                            TOP(1) 1
                            FROM
	                            dbo.Tb_Dts_StateProvince AS TDSP
                            WHERE
	                            TDSP.StateId = {SenderStateId})
                            BEGIN
	                            SELECT CAST(1 AS BIT)
                            END
                            ELSE
                            BEGIN
                                SELECT CAST(0 AS BIT)
                            END";
                return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
            }
            else
            {
                return true;
            }
        }


    }
}
