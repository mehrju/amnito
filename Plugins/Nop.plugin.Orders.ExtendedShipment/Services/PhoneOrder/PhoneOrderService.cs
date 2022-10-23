using Nop.Core;
using Nop.Core.Data;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Domain.PhoneOrder;
using Nop.plugin.Orders.ExtendedShipment.Enums;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Models.PhoneOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services.PhoneOrder
{
    public class PhoneOrderService : IPhoneOrderService
    {
        private readonly IRepository<Tbl_PhoneOrder> _repository_PhoneOrder;
        private readonly IDbContext _dbContext;
        private readonly IRepository<ShipmentTrackingModel> _repository_ShipmentTracking;

        public PhoneOrderService(IRepository<Tbl_PhoneOrder> repository_PhoneOrder,
            IDbContext dbContext,
            IRepository<ShipmentTrackingModel> repository_ShipmentTracking)
        {
            _repository_PhoneOrder = repository_PhoneOrder;
            _dbContext = dbContext;
            _repository_ShipmentTracking = repository_ShipmentTracking;
        }

        public int? CanRegisterPhoneOrder(int stateId)
        {
            string query = $@"SELECT
	TOP(1) C.Id COllectorId
FROM
	dbo.Customer AS C
	INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
	INNER JOIN dbo.Tb_UserStates AS TUS ON TUS.CustomerId = C.Id
WHERE
	CCRM.CustomerRole_Id = 31
	AND TUS.StateId = {stateId}
	AND C.Active= 1";
            return _dbContext.SqlQuery<int?>(query).FirstOrDefault();

        }

        public void InsertPhoneOrder(PhoneOrderModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }


            var entity = new Tbl_PhoneOrder()
            {
                Address = model.Address,
                CityId = model.CityId,
                FirstName = model.FirstName,
                IsCarRequired = model.IsCarRequired,
                LastName = model.LastName,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                PhoneNumber = model.PhoneNumber,
                PostalCode = model.PostalCode,
                TellNumber = model.TellNumber,
                CreateDate = DateTime.Now,
                ServiceId = model.ServiceId,
                HasLocation = model.HasLocation
            };
            _repository_PhoneOrder.Insert(entity);

            _repository_ShipmentTracking.Insert(new ShipmentTrackingModel()
            {
                ShipmentEventId = (int)OrderShipmentStatusEnum.Registration,
                LastShipmentEventDate = DateTime.Now,
            });
            model.Id = entity.Id;
        }


        public PagedList<PhoneOrderModel> GetPagedPhoneOrders(string fromDateStr,string toDateStr, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var fromDate = fromDateStr.FromPersianToGregorianDate();
            var toDate = toDateStr.FromPersianToGregorianDate();
            return new PagedList<PhoneOrderModel>(_dbContext.SqlQuery<PhoneOrderModel>($@"SELECT tpo.*,sp.Name CityName,sp.CountryId,c.Name StateName,CAST(CASE WHEN tpoo.Id IS NOT NULL THEN 1 ELSE 0 END AS BIT) HasOrder FROM dbo.Tbl_PhoneOrder AS tpo
INNER JOIN dbo.StateProvince AS sp ON sp.Id = tpo.CityId
INNER JOIN dbo.Country AS c ON c.Id = sp.CountryId
LEFT JOIN dbo.Tbl_PhoneOrder_Order AS tpoo ON tpoo.PhoneOrderId = tpo.Id
WHERE {((fromDateStr == null || fromDateStr == "null") ? "1=1" : $"tpo.CreateDate >= '{fromDate.ToDateString()}'")} AND
{((toDateStr == null || toDateStr == "null") ? "1=1" : $"tpo.CreateDate <= '{toDate.ToDateString()}'")}
ORDER BY tpo.CreateDate DESC
OFFSET ({pageIndex} * {pageSize}) ROWS
FETCH NEXT {pageSize} ROWS ONLY").ToList(), pageIndex, pageSize);
        }


        public Tbl_PhoneOrder GetPhoneOrderById(int id)
        {
            return _repository_PhoneOrder.GetById(id);
        }


        public void UpdatePhoneOrder(Tbl_PhoneOrder entity)
        {
            _repository_PhoneOrder.Update(entity);
        }


        public List<PostService> ListOfService()
        {
            string query = @"SELECT
	                            DISTINCT C.Name AS ServiceName
	                            ,C.Id AS ServiceId
                            FROM
	                            dbo.Category AS C
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.CategoryId = C.Id
	                            INNER JOIN dbo.StoreMapping AS SM ON sm.EntityId = C.Id AND sm.EntityName ='Category'
	                            INNER JOIN dbo.Tb_CategoryInfo AS TCI ON TCI.CategoryId = C.Id
                            WHERE	
	                             C.Deleted = 0
	                            AND C.Published = 1
	                            AND SM.StoreId = 5
                            ORDER BY C.Name";
            return _dbContext.SqlQuery<PostService>(query, new object[0]).AsEnumerable<PostService>().ToList();
        }


    }
}
