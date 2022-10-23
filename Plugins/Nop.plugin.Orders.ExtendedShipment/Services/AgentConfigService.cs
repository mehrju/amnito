using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class AgentConfigService : IAgentConfigService
    {
        private readonly IDbContext _dbContext;

        public AgentConfigService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SelectListItem> GetAgents()
        {
            return _dbContext.SqlQuery<SelectListItem>($@"
                          SELECT
	                          cast(C.Id as Nvarchar(15)) Value,
	                          ISNULL(TCI.FullName+N'-',N'')+C.Username Text
                          FROM
                          	dbo.Customer AS C
                          	INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
                          	LEFT JOIN dbo.Tb_CustomerInfo AS TCI ON C.Id= TCI.CustomerId
                          WHERE
                          	CCRM.CustomerRole_Id = 7
                            AND C.Active=1
                          ORDER BY ISNULL(TCI.FullName,C.Username)
				").ToList();
        }

        public List<SelectListItem> GetCollectors()
        {
            return _dbContext.SqlQuery<SelectListItem>($@"
                          SELECT
                          	  cast(C.Id as Nvarchar(15)) Value,
	                          ISNULL(TCI.FullName+N'-',N'')+C.Username Text
                          FROM
                          	dbo.Customer AS C
                          	INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
                          	LEFT JOIN dbo.Tb_CustomerInfo AS TCI ON C.Id= TCI.CustomerId
                          WHERE
                          	CCRM.CustomerRole_Id = 31
                            AND C.Active=1
                          ORDER BY ISNULL(TCI.FullName,C.Username)
				").ToList();
        }

        public void SaveAgentConfig(AgentConfigResultModel model)
        {
            string query = "IF NOT EXISTS( SELECT * FROM dbo.Tbl_AgentNearpostNode WHERE RepresentativeCustomerId = '" + model.RepresentativeCustomerId + "') INSERT INTO Tbl_AgentNearpostNode(AgentCustomerId, NearSateId, RepresentativeCustomerId) values('" + model.AgentCustomerId + "', '" + model.NearStateId + "', '" + model.RepresentativeCustomerId + "')";
            _dbContext.ExecuteSqlCommand(query);
        }
        public List<AgentConfigGridModel> GridAgentConfig(AgentConfigInputModel model, int PageSize, int PageIndex, out int count)
        {
            SqlParameter P_count = new SqlParameter() { ParameterName = "Count", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };

            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter() { ParameterName = "NearStateId", SqlDbType = SqlDbType.Int, Value = model.NearStateId, Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "NearCountryId", SqlDbType = SqlDbType.Int, Value = model.NearCountryId, Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "AgentCustomerId", SqlDbType = SqlDbType.Int, Value = model.AgentCustomerId, Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "RepresentativeCustomerId", SqlDbType = SqlDbType.Int, Value = model.RepresentativeCustomerId, Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "PageSize", SqlDbType = SqlDbType.Int, Value = PageSize, Direction = ParameterDirection.Input },
                new SqlParameter() { ParameterName = "PageIndex", SqlDbType = SqlDbType.Int, Value = PageIndex, Direction = ParameterDirection.Input },
                P_count
            };
            var allShipment = _dbContext.SqlQuery<AgentConfigGridModel>(@"EXECUTE [dbo].[Sp_GetAgentConfig] @NearStateId,@NearCountryId, @AgentCustomerId, @RepresentativeCustomerId, @PageSize, @PageIndex, @count OUTPUT", prms).ToList();

            count = (int)P_count.Value;
            return allShipment.ToList();
        }

        public void UpdateAgentConfig(AgentConfigResultModel model)
        {
            string query = $@"UPDATE Tbl_AgentNearpostNode 
                              SET AgentCustomerId = {model.AgentCustomerId},
                                  NearSateId = {model.NearStateId},
                                  RepresentativeCustomerId = {model.RepresentativeCustomerId}
                              WHERE
                                  Id = {model.Id}
                            ";
            _dbContext.ExecuteSqlCommand(query);
        }

        public List<ServiceDiscountGridModel> GridServiceDiscount(int CustomerId)
        {
            return _dbContext.SqlQuery<ServiceDiscountGridModel>($@"
                    SELECT 
                    	C.Name As ServiceName ,
                    	ISNULL(DiscountPercent,DiscountPrice) Discount 
                    FROM dbo.Category AS C
                    	INNER JOIN dbo.Tb_CategoryInfo AS TCI ON C.Id = tci.CategoryId
                    	INNER JOIN dbo.Tb_PrivatePostDiscount AS TPPD ON TPPD.ServiceId = C.Id
                    WHERE
                    	C.Published = 1 AND TPPD.IsActive = 1 AND TPPD.CustomerId = " + CustomerId)
                .ToList();
        }

        public List<SelectListItem> GetServices()
        {
            return _dbContext.SqlQuery<SelectListItem>($@"
                          SELECT
                            CAST(C.Id AS NVARCHAR(10)) Value,
                            C.Name AS Text
                          FROM
                          	dbo.Category AS C
                          	INNER JOIN dbo.Tb_CategoryInfo AS TCI ON C.Id = tci.CategoryId
                          WHERE
                          	C.Published = 1
				").ToList();
        }

        public void SaveServiceDiscount(ServiceDiscountModel model)
        {
            var ActiveDate =  model.ActiveDate.Value.ToShortDateString().Replace('/', '-');
            string query = $@"
                        IF EXISTS(SELECT * FROM dbo.Tb_PrivatePostDiscount WHERE CustomerId={model.CustomerId} AND ServiceId={model.ServiceId})            
	                    BEGIN            
	                    	UPDATE dbo.Tb_PrivatePostDiscount SET IsActive = 0,
	                    										   deActiveDate = {ActiveDate},
	                    										   deActiveCustomerId = {model.ActiveCustomerId}  
	                    					WHERE CustomerId={model.CustomerId} AND ServiceId={model.ServiceId}  
	                    End                                
                        BEGIN  
                        	INSERT INTO dbo.Tb_PrivatePostDiscount
                        	(
                        	    CustomerId,
                        	    ServiceId,
                        	    DiscountPercent,
                        	    DiscountPrice,
                        	    IsActive,
                        	    deActiveDate,
                        	    deActiveCustomerId,
                        	    ActiveDate,
                        	    ActiveCstomerId
                        	)
                        	VALUES
                        	(   
                                {model.CustomerId},         
                        	    {model.ServiceId},   
                        	    {model.DiscountPercent},
                        	    {model.DiscountPrice},
                        	    1,
                        	    NULL,
                        	    NULL,
                        	    {ActiveDate}, 
                        	    {model.ActiveCustomerId}          
                        	)  
                        END 
            ";
            _dbContext.ExecuteSqlCommand(query);
        }
    }
}
