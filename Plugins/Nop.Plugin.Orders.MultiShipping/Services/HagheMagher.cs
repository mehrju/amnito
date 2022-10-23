using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Web.Models.ShoppingCart;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public class HagheMaghar : IHagheMaghar
    {
        private readonly IRepository<HagheMagharModel> _repositoryHagheMaghar;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IDbContext _dbContext;
        private readonly IAddressService _addressService;
        private readonly ICustomerService _customerService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        public HagheMaghar(IRepository<HagheMagharModel> repositoryHagheMaghar,
            ISettingService settingService
            , IDbContext dbContext
            , IAddressService addressService
            , ICustomerService customerService
            , IStoreContext storeContext
            , IExtendedShipmentService extendedShipmentService)
        {
            _extendedShipmentService = extendedShipmentService;
            _storeContext = storeContext;
            _customerService = customerService;
            _settingService = settingService;
            _repositoryHagheMaghar = repositoryHagheMaghar;
            _dbContext = dbContext;
            _addressService = addressService;
        }
        public void Insert(int orderItemId, int HagheMagharPrice,int ShipmentHagheMaghar)
        {
            var model = new HagheMagharModel()
            {
                HagheMagharPrice = HagheMagharPrice,
                OrderItemId = orderItemId,
                ShipmentHagheMaghr = ShipmentHagheMaghar
            };
            _repositoryHagheMaghar.Insert(model);
        }
        public int getHaghMaghrInOrder(int customerId, OrderTotalsModel model)
        {
            return 0;
            //if (string.IsNullOrEmpty(model.SelectedShippingMethod))
            //    return 0;
            //if (customerId == 0)
            //    return 0;
            //var customer = _customerService.GetCustomerById(customerId);

            //if (customer.BillingAddress == null)
            //    return 0;

            //int HagheMagharPrice = 0;
            //var setting =
            //    _settingService.GetSetting("NopMaster.Wallet_ProductId", _storeContext.CurrentStore.Id, false);
            //int productId = setting == null ? 0 : int.Parse(setting.Value);
            //if (productId != 0)
            //    if (customer.ShoppingCartItems.Any(p => p.ProductId == productId))
            //        return 0;
            //var product = customer.ShoppingCartItems.Select(p => p.Product).Distinct().First();
            //var catInfo = _extendedShipmentService.GetCategoryInfo(product);
            //if (catInfo == null)
            //{
            //    _extendedShipmentService.Log($"حق مقر برای سرویس {product.Name} تعریف نشده", "");
            //    return 0;
            //}
            //if (!catInfo.HasHagheMaghar)
            //    return 0;

            ////if (customer.ShoppingCartItems.Any(p => p.Product.Name.Contains("پس کرایه") || p.Product.Name.ToLower().Contains("cod")))
            ////    return 0;
            //var address = customer.BillingAddress;
            //SqlParameter[] prms = new SqlParameter[] {
            //    new SqlParameter() { ParameterName = "@Int_CountryId", SqlDbType = SqlDbType.Int,Value = address.CountryId },
            //    new SqlParameter() { ParameterName = "@Int_StateId", SqlDbType = SqlDbType.Int,Value = address.StateProvinceId },
            //    new SqlParameter() { ParameterName = "@Nvc_Address", SqlDbType = SqlDbType.NVarChar,Value = address.Address1 },
            //    new SqlParameter() { ParameterName = "@Nvc_FirstName", SqlDbType = SqlDbType.NVarChar,Value = address.FirstName },
            //    new SqlParameter() { ParameterName = "@Nvc_LastName", SqlDbType = SqlDbType.NVarChar,Value = address.LastName },
            //    new SqlParameter() { ParameterName = "@vc_PhoneNumber", SqlDbType = SqlDbType.NVarChar,Value = address.PhoneNumber },
            //    new SqlParameter() { ParameterName = "Int_CustomerId", SqlDbType = SqlDbType.Int, Value = customerId },
            //    new SqlParameter() { ParameterName = "ServiceId", SqlDbType = SqlDbType.Int, Value = catInfo.CategoryId },
            //    new SqlParameter() { ParameterName = "InComeTotalWeight", SqlDbType = SqlDbType.Int, Value = catInfo.CategoryId },
            //};
            //int? price = _dbContext.SqlQuery<int>(@"EXECUTE [dbo].[Sp_CheckBillingAddressForHagheMaghar] @Int_CountryId,@Int_StateId,@Nvc_Address,@Nvc_FirstName,@Nvc_LastName,@vc_PhoneNumber,@Int_CustomerId,@ServiceId", prms).FirstOrDefault();
            //return price.GetValueOrDefault(0);
        }
        public int getInsertedHagheMaghar(Order order)
        {
            var orderItems = order.OrderItems.Select(p => p.Id).ToList();
            return ((int?)_repositoryHagheMaghar.Table.SingleOrDefault(p => orderItems.Contains(p.OrderItemId))?.HagheMagharPrice).GetValueOrDefault(0);
        }
        public int getHagheMagharByCountryId(int countryId)
        {
            string query= @"SELECT
				            TCHM.HagheMagharPrice
			            FROM
				            dbo.Tb_CountryHagheMaghar AS TCHM
			            WHERE
				            TCHM.CountryId = " + countryId;
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
    }
}
