using Nop.Core.Data;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Models.distOrCollect;
using Nop.plugin.Orders.ExtendedShipment.Models.Tozico.Input;
using Nop.plugin.Orders.ExtendedShipment.Services.Tozico;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class CollectorService : ICollectorService
    {
        private readonly IRepository<ShipmentAppointmentModel> _ShipmentAppointmentRepository;
        private readonly IDbContext _dbContext;
        private readonly ITozicoService _tozicoService;

        public CollectorService(IRepository<ShipmentAppointmentModel> ShipmentAppointmentRepository,
            IDbContext dbContext, ITozicoService tozicoService)
        {
            _tozicoService = tozicoService;
            _ShipmentAppointmentRepository = ShipmentAppointmentRepository;
            _dbContext = dbContext;
        }
        #region Collectors
        public bool ChooseCollector(int shipmentId, int CollectorId, string desc)
        {
            var SAR = _ShipmentAppointmentRepository.Table.OrderByDescending(p => p.Id)
                .FirstOrDefault(p => p.ShipmentId == shipmentId);
            if (SAR == null)
            {
                SAR = new ShipmentAppointmentModel()
                {
                    ShipmentId = shipmentId
                };
                _ShipmentAppointmentRepository.Insert(SAR);
            }

            string query = $@" INSERT INTO dbo.Tb_Collectors
                            (
	                            ShipmentAppointmentId
	                            , CustomerId
	                            , AssignDate
	                            , [Desc]
                            )
                            VALUES
                            (	{SAR.Id} 
	                            , {CollectorId}
	                            , GETDATE()
	                            , N'{desc}' 
                            )
                            SELECT CAST(SCOPE_IDENTITY() AS INT) Id ";
            int CollectId = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            if (CollectId > 0)
                return true;
            return false;
        }
        public LastCollector getLastCollector(int shipmentId)
        {
            string query = $@"SELECT
	                            NOC.Name CollectorName,
	                            dbo.GregorianToJalali(Tc.AssignDate,'yyyy/MM/dd HH:mm:ss') AS AssignDate
                            FROM
	                            dbo.ShipmentAppointment AS SA
	                            INNER JOIN dbo.Tb_Collectors AS TC ON TC.ShipmentAppointmentId = SA.Id
	                            INNER JOIN dbo.Customer AS C ON Tc.CustomerId = C.Id
	                            INNER JOIN dbo.[Name Of Customer] AS NOC ON C.Id = NOC.EntityId
                            WHERE
	                            sa.ShipmentId = {shipmentId}";
            return _dbContext.SqlQuery<LastCollector>(query, new object[0]).FirstOrDefault();
        }
        public List<CollectorList> GetAllCollectorList()
        {
            string query = $@"SELECT 
	                            CASE WHEN (tci.FullName ='' OR TCI.FullName IS NULL) THEN C.Username ELSE tci.FullName END CustomerName,
	                            C.Id as CustomerId,
	                            ISNULL(Cn.Id,-1) AS CountryId,
	                            ISNULL(Sp.Id,-1) AS StateProvince
                            FROM
	                            dbo.Customer AS C
	                            INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id 
	                            INNER JOIN dbo.CustomerRole AS CR ON CR.Id = CCRM.CustomerRole_Id
	                            INNER JOIN dbo.Tb_UserStates AS TUS ON TUS.CustomerId = C.Id
	                            INNER JOIN dbo.StateProvince AS SP ON SP.Id = TUS.StateId
	                            INNER JOIN dbo.Country AS Cn ON Cn.Id = SP.CountryId
	                            LEFT JOIN dbo.Tb_CustomerInfo AS TCI ON TCI.CustomerId = C.Id
                            WHERE
	                            Cr.SystemName LIKE N'%CollerctorAgent%'
	                            AND C.Deleted = 0
	                            AND C.Active = 1
                                ORDER BY Cn.Name,sp.Name";

            return _dbContext.SqlQuery<CollectorList>(query, new object[0]).ToList();
        }
        public int assignToCollector(int shipmentId)
        {
            try
            {
                string query = "EXEC dbo.Sp_AssignToCollector @shipmentId ";
                SqlParameter P_shipmentId = new SqlParameter()
                {
                    ParameterName = "shipmentId",
                    SqlDbType = SqlDbType.Int,
                    Value = (object)shipmentId
                };

                SqlParameter[] prms = new SqlParameter[]{
                    P_shipmentId
                };
                var AssignResult = _dbContext.SqlQuery<AssignToCollectorResult>(query, prms).FirstOrDefault();
                if (AssignResult != null && AssignResult.AgentId > 0)
                {
                    //Send To Tozie Co
                    //TozicoCustomer shipment = new TozicoCustomer() {
                    //    Branch = AssignResult.AgentId,

                    //};
                    //_tozicoService.AddOrUpdateCustomers()
                }
                return AssignResult.CollectorId.GetValueOrDefault(0);
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                common.Log("خطا در زمان انتصاب جمع آور به محموله شماره " + shipmentId, "");
                return 0;
            }
        }
        public void assignToDistributer(Order order)
        {
            foreach (var item in order.Shipments)
            {
                try
                {
                    string query = "EXEC dbo.Sp_AssignToDistributer @shipmentId ";
                    SqlParameter P_shipmentId = new SqlParameter()
                    {
                        ParameterName = "shipmentId",
                        SqlDbType = SqlDbType.Int,
                        Value = (object)item.Id
                    };

                    SqlParameter[] prms = new SqlParameter[]{
                        P_shipmentId
                    };
                    var AssignResult = _dbContext.SqlQuery<AssignToDistributerResult>(query, prms).FirstOrDefault();
                    if (AssignResult != null)
                    {
                        if (AssignResult.DistributerId == 0)
                        {
                            common.InsertOrderNote(AssignResult.Message + " => ShipmentId = " + item.Id, order.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    common.LogException(ex);
                    common.Log("خطا در زمان انتصاب توزیع کننده به محموله شماره " + item.Id, "");
                }
            }

        }
        public bool HasCollectorInState(int StateId)
        {
            string query = $@"IF EXISTS(
                            SELECT
	                            TOP(1) 1
                            FROM
	                            dbo.Customer AS C
	                            INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id
	                            INNER JOIN dbo.CustomerRole AS CR ON CR.Id = CCRM.CustomerRole_Id
	                            INNER JOIN dbo.Tb_UserStates AS TUS ON TUS.CustomerId = C.Id
                            WHERE
	                            C.Active = 1
	                            AND C.Deleted = 0
	                            AND Cr.SystemName =N''
	                            AND TUS.StateId = {StateId})
                            BEGIN
                                SELECT CAST(1 AS BIT) HasCollector
                            END
                            ELSE
                            BEGIN
                                SELECT CAST(0 AS BIT) HasCollector
                            END";
            return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }
        public List<CollectorList> GetBikers()
        {
            string query = $@"SELECT 
	                            CASE WHEN (tci.FullName ='' OR TCI.FullName IS NULL) THEN C.Username ELSE tci.FullName END CustomerName,
	                            C.Id as CustomerId,
	                            ISNULL(Cn.Id,-1) AS CountryId,
	                            ISNULL(Sp.Id,-1) AS StateProvince
                            FROM
	                            dbo.Customer AS C
	                            INNER JOIN dbo.Customer_CustomerRole_Mapping AS CCRM ON CCRM.Customer_Id = C.Id 
	                            INNER JOIN dbo.CustomerRole AS CR ON CR.Id = CCRM.CustomerRole_Id
	                            INNER JOIN dbo.Tb_UserStates AS TUS ON TUS.CustomerId = C.Id
	                            INNER JOIN dbo.StateProvince AS SP ON SP.Id = TUS.StateId
	                            INNER JOIN dbo.Country AS Cn ON Cn.Id = SP.CountryId
	                            LEFT JOIN dbo.Tb_CustomerInfo AS TCI ON TCI.CustomerId = C.Id
                            WHERE
	                            Cr.SystemName LIKE N'%CollectorBiker%'
	                            AND C.Deleted = 0
	                            AND C.Active = 1
                                ORDER BY Cn.Name,sp.Name";

            return _dbContext.SqlQuery<CollectorList>(query, new object[0]).ToList();
        }

        public class CollectorList
        {
            public string CustomerName { get; set; }
            public int CustomerId { get; set; }
            public int CountryId { get; set; }
            public int StateProvince { get; set; }
        }
        public class LastCollector
        {
            public string CollectorName { get; set; }
            public string AssignDate { get; set; }
        }
        public class AssignToCollectorResult
        {
            public int? CollectorId { get; set; }
            public int? AgentId { get; set; }
        }
        public class AssignToDistributerResult
        {
            public int DistributerId { get; set; }
            public string Message { get; set; }
        }
        #endregion

        public bool SaveCollectingPrices(PriceResponse pricesData, int orderId)
        {
            return true;
        }
        public PriceResponse CalculatePrice(int SenderCountry, int SenderState, string Address, int CustomerId, int ServiceId, int orderId,
            ServicesType _servicesType)
        {
            List<ShipmentWithVolume> _allShipment = new List<ShipmentWithVolume>();
            var _oldShipment = getShipmentAndSize(SenderCountry, SenderState, Address, CustomerId, ServiceId, null);
            var _newShipment = getShipmentAndSize(SenderCountry, SenderState, Address, CustomerId, ServiceId, orderId);
            if (_oldShipment != null && _oldShipment.Any())
            {
                _allShipment.AddRange(_oldShipment);
            }
            if (_newShipment != null && _newShipment.Any())
            {
                _allShipment.AddRange(_newShipment);
            }

            List<CreatePriceDetail> Lst_parcels = _allShipment.Select(p => new CreatePriceDetail()
            {
                buyingPrice = 0,
                sellingPrice = 0,
                sizeOfBox = null,
                collectionPrice = p.collectionPrice,
                distributionPrice = p.distributionPrice,
                isCanceled = p.isCanceled,
                isNew = p.isNew,
                height = p.Height,
                length = p.Length,
                width = p.Width,
                ShipmentId = p.ShipmentId.ToString(),
                DestinationCityTypeId = p.ReceiverCityType
            }).ToList();
            CreatePrice _createPrice = new CreatePrice()
            {
                commentCollection = "",
                commentDistribution = "",
                basketId = orderId.ToString(),
                parcels = Lst_parcels,
                courierId = getCurier(orderId),
                cityTypeId = Lst_parcels.First().DestinationCityTypeId,
                serviceId = (int)_servicesType,

            };

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var client = new RestClient("https://parcelprice.postex.ir/api/basketapi");
            //var client = new RestClient("http://localhost:5000/api/Gateway");
            var request = new RestRequest("/CreatePriceResponseCollection", Method.POST);
            string jsonToSend = Newtonsoft.Json.JsonConvert.SerializeObject(_createPrice);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            //request.AddBody(_createPrice);
            PriceResponse reuslt = new PriceResponse();
            try
            {
                var reposnce = client.Execute(request);
                if (reposnce.StatusCode == HttpStatusCode.OK)
                {
                    if (!string.IsNullOrEmpty(reposnce.Content))
                    {
                        reuslt = Newtonsoft.Json.JsonConvert.DeserializeObject<PriceResponse>(reposnce.Content);
                        return reuslt;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                return null;
            }



            PriceResponse responce = new PriceResponse();
            return responce;
        }
        private int getCurier(int orderId)
        {
            string query = $@"SELECT
	                            C.Name ServiceName,
	                            C.Id ServiceId,
	                            CASE 
	                            WHEN TCI.CategoryId IN (718) THEN 1
	                            WHEN TCI.CategoryId IN (733) THEN 2
	                            WHEN TCI.CategoryId IN (712,713,714,715) THEN 3
	                            WHEN TCI.CategoryId IN (730) THEN 4
	                            WHEN TCI.CategoryId IN (670,723) THEN 6
	                            WHEN TCI.CategoryId IN (702) THEN 8
	                            WHEN TCI.IsForeign =1 THEN 7 
	                            ELSE 0 END Courier

                            FROM 
	                            dbo.[Order] AS O
	                            INNER JOIN dbo.OrderItem AS OI ON OI.OrderId = O.Id
	                            INNER JOIN dbo.Product AS P ON P.Id = OI.ProductId
	                            INNER JOIN dbo.Product_Category_Mapping AS PCM ON PCM.ProductId = P.Id
	                            INNER JOIN dbo.Category AS C ON C.Id = PCM.CategoryId
	                            INNER JOIN 	dbo.Tb_CategoryInfo AS TCI ON C.Id = TCI.CategoryId
                            WHERE
	                            O.id = {orderId}";
            var data = _dbContext.SqlQuery<getCourierModel>(query, new object[0]).FirstOrDefault();
            if (data == null)
                return 0;
            return data.Courier;
        }
        public List<ShipmentWithVolume> getShipmentAndSize(int SenderCountry, int SenderState, string Address, int CustomerId, int ServiceId, int? orderId)
        {
            #region PrevItem
            string query = $@"EXEC dbo.Sp_GetShipmentAndSize @Int_CountryId,@Int_StateId,@Nvc_Address,@Int_CustomerId,@OrderId";

            SqlParameter P_CountryId = new SqlParameter()
            {
                ParameterName = "Int_CountryId",
                SqlDbType = SqlDbType.Int,
                Value = (object)SenderCountry
            };
            SqlParameter P_SernderState = new SqlParameter()
            {
                ParameterName = "Int_StateId",
                SqlDbType = SqlDbType.Int,
                Value = (object)SenderState
            };
            SqlParameter P_Address = new SqlParameter()
            {
                ParameterName = "Nvc_Address",
                SqlDbType = SqlDbType.NVarChar,
                Value = (object)Address
            };
            SqlParameter P_CustomerId = new SqlParameter()
            {
                ParameterName = "Int_CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = (object)CustomerId
            };
            SqlParameter P_orderId = new SqlParameter()
            {
                ParameterName = "OrderId",
                SqlDbType = SqlDbType.Int,
                Value = (orderId.HasValue ? (object)orderId.Value : (object)DBNull.Value)
            };
            SqlParameter[] prms = new SqlParameter[]{
                P_CountryId,
                P_SernderState,
                P_Address,
                P_CustomerId,
                P_orderId
            };
            return _dbContext.SqlQuery<ShipmentWithVolume>(query, prms).ToList();
            #endregion
        }
        public int getCollectingPriceInfo(int Volume, int SenderStateId)
        {

            string query = "EXEC dbo.Sp_GetDistribtionPrice @Volume,@ReciverStateId ,@Price OUTPUT";
            SqlParameter P_Price = new SqlParameter() { ParameterName = "Price", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            SqlParameter[] prms = new SqlParameter[] {
                    new SqlParameter() { ParameterName = "Volume", SqlDbType = SqlDbType.Int, Value = Volume },
                    new SqlParameter() { ParameterName = "ReciverStateId", SqlDbType = SqlDbType.Int, Value = SenderStateId},
                    P_Price
                };
            var data = _dbContext.SqlQuery<int?>(query, prms).FirstOrDefault().GetValueOrDefault(0);
            return (int)P_Price.Value;
        }
        public BoxSizePricingInfo GetByVolume(int volume, int CityId)
        {
            string query = $@"SELECT TOP(1)
                                TICDR.SizeName,
                                TICDR.TehranTotalPrice,
                                TICDR.G8TotalPrice,
                                TICDR.OtherCItyTotalPrice,
                                TICDR.Volume
                            FROM 
	                            dbo.Tb_InCityDistributionRules AS TICDR
                            WHERE
	                            TICDR.Volume - {volume} >= 0
                            ORDER BY (TICDR.Volume - {volume})";
            return _dbContext.SqlQuery<BoxSizePricingInfo>(query, new object[0]).FirstOrDefault();
        }
        public BoxSizePricingInfo GetMaxBox(int CityId)
        {
            String SubQuery = "";
            if (new int[] { 4, 579, 580, 581, 582, 583, 584, 585 }.Contains(CityId))//تهران
            {
                SubQuery = "SELECT TICDR.TehranTotalPrice TotalPrice ";
            }
            else if (new int[] { 74, 175, 207, 259, 343, 409, 526, 555 }.Contains(CityId))// G8
            {
                SubQuery = "SELECT TICDR.G8TotalPrice TotalPrice ";
            }
            else
            {
                SubQuery = "SELECT TICDR.OtherCItyTotalPrice TotalPrice ";
            }
            string query = $@"SELECT TOP(1)
                                TICDR.SizeName,
                                {SubQuery}
                                TICDR.Volume
                            FROM 
	                            dbo.Tb_InCityDistributionRules AS TICDR
                            ORDER BY TICDR.Volume DESC";
            return _dbContext.SqlQuery<BoxSizePricingInfo>(query, new object[0]).FirstOrDefault();
        }
        public void SaveCollectingPrices(PriceResponse collectingInfo)
        {
            try
            {
                List<int> InsertedIds = new List<int>();
                if (collectingInfo != null && collectingInfo.BoxSizes != null && collectingInfo.BoxSizes.Any())
                {
                    foreach (var item in collectingInfo.BoxSizes.Where(p => p.IsNew))
                    {
                        string query = $@"	INSERT INTO dbo.Tb_ShipmentCollectingPrice
	                        (
		                        shipmetId,
		                        CollectingPrice,
		                        DistributionPrice
	                        )
	                        VALUES
	                        (   {item.ShipmentId},    -- shipmetId - int
		                        {item.CollectionPrice}, -- CollectingPrice - int
		                        {item.DistributionPrice}  -- DistributionPrice - int
	                        ) Select SELECT CAST(SCOPE_IDENTITY() AS INT) InsertedId";
                        int Id = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
                        if (Id > 0)
                        {
                            InsertedIds.Add(Id);
                        }
                    }
                    if (collectingInfo.BoxSizes.Where(p => p.IsNew).Count() != InsertedIds.Count)
                    {
                        common.Log("ایراد در درج  نتایج سرویس توزیع و جمع آوری", Newtonsoft.Json.JsonConvert.SerializeObject(collectingInfo));
                    }
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
            }
        }
    }
    #region Model
    public class getCourierModel
    {
        public string ServiceName { get; set; }
        public int ServiceId { get; set; }
        public int Courier { get; set; }
    }
    public enum CityType
    {
        [Description("تهران")] Tehran = 1,
        [Description(" کلانشهرها-جی 8")] G8 = 2,
        [Description("مرکز استان")] StateCenter = 3,
        [Description("شهرستان ها")] SmallCities = 4
    }
    public enum ServicesType
    {
        [Description("جمع آوری و توزیع")] DistributionAndCollectionPaykhub = 1,
        [Description("جمع آوری ")] CollectionForMiddleMileDelivery = 2,
        [Description("توزیع")] Distribution = 3
    }
    public enum Couriers
    {
        [Description("امنیتو")] Paykhub = 1,
        [Description("کالارسان")] Kalaresan = 2,
        [Description("چاپار")] Chappar = 3,
        [Description("ماهکس")] Mahex = 4,
        [Description("آرامکس")] Aramex = 5,
        [Description("پست")] Post = 6,
        [Description("بین المللی")] International = 7,
        [Description("یارباکس")] yarbox = 8
    }
    public class ShipmentWithVolume
    {
        public int ShipmentId { get; set; }
        public string _Address { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Length { get; set; }
        public int collectionPrice { get; set; }
        public int distributionPrice { get; set; }
        public int SenderCityType { get; set; }
        public int ReceiverCityType { get; set; }
        public bool isCanceled { get; set; }
        public bool isNew { get; set; }
        public bool NeedCollector { get; set; }
        public bool NeedDistributer { get; set; }
    }
    public class CreatePriceDetail
    {
        public int? sizeOfBox { get; set; }
        public int height { get; set; }
        public int width { get; set; }
        public int length { get; set; }
        public int buyingPrice { get; set; }
        public int sellingPrice { get; set; }
        public int collectionPrice { get; set; }
        public int distributionPrice { get; set; }
        public int DestinationCityTypeId { get; set; }
        public string ShipmentId { get; set; }
        public bool isNew { get; set; }
        public bool isCanceled { get; set; }
        public bool NeedsCollection { get; set; }
        public bool NeedsDistribution { get; set; }
    }
    public class CreatePrice
    {
        public List<CreatePriceDetail> parcels { get; set; }
        public int cityTypeId { get; set; }
        public int serviceId { get; set; }
        public int courierId { get; set; }
        public string commentCollection { get; set; }
        public string commentDistribution { get; set; }
        public string basketId { get; set; }
    }
    public class PriceResponse
    {
        public List<BoxPrice> BoxSizes { get; set; }
        public decimal CollectionPrice { get; set; }
        public decimal DistributionPrice { get; set; }
        public string CommentCollection { get; set; }
        public string CommentDistribution { get; set; }
        public decimal WalletCollection { get; set; }
        public decimal WalletDistribution { get; set; }
        public string BasketId { get; set; }
    }
    public class BoxPrice
    {
        public string SizeOfBox { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Length { get; set; }
        public decimal CollectionPrice { get; set; }
        public decimal DistributionPrice { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public CityType DestinationCityTypeId { get; set; }
        public string ShipmentId { get; set; }
        public bool IsNew { get; set; }
        public bool IsCanceled { get; set; }
    }
    #endregion
}
