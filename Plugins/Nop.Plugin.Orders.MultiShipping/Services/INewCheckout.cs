using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Plugin.Orders.MultiShipping.Model;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Orders;
using Nop.Web.Models.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Services
{
    public interface INewCheckout
    {
        CheckoutPaymentMethodModel getPaymentMethodForSafeBuy(Order order, Customer customer);
        bool RequestToApplyDiscount(int orderId, string CopunCode,bool IsForInternational=false);
        ShipmentDimention getDimentionByName(string name);
        List<InvoiceDetails> Sh_GetFactorModel(List<int> OrderIds);
        CategoryInfoModel getCategoryInfo(Order order);
        bool isInvalidSernder(int senderCountry, int senderState);
        int CalcHagheMaghar(Address BillingAddress, int serviceId, int totalWeight, int customerId);
        int getRelatedOrder(int ParentOrderId);
        bool CanPayForOrder(int orderId, out string msg);
        bool canAddValueToForginRequest(int parentOrderId);
        int getForginAddtionalValue(int orderId);
        void InsertRelatedOrder(int orderId1, int OrderId2, int relatedOrderPrice);
        (decimal Lat, decimal Lon) GetLatLong(int? CountryId, int? StateProvinceId);
        void RestartStopwatch(System.Diagnostics.Stopwatch watch, string logNote, ref long Total);
        CheckoutPaymentMethodModel getPaymentMethod(Order order);
        int CalcHagheSabet(int customerId, int ServiceId, int OrderId = 0);
        bool _SendDataToPost(Order order, out string error);
        List<SelectListItem> getUbbarStateByCountryId(int countryId);
        List<SelectListItem> getUbbarVechileOption(string TruckTypeName);
        List<SelectListItem> getUbbarTruckType();
        List<SelectListItem> getForginCountry();
        SubMarketModel GetStoreAndSubMarket(string host, string Path);
        Address ProcessAddress(Address address, Customer customer, string type);
        Address ProcessAddress(NewCheckoutModel model, Customer customer, string type);
        CheckoutPaymentMethodModel getPaymentMethodForChargeWallet();
        PlaceOrderResult ProccessWalletOrder(int Amount, string paymentMethodName, int CustomerId);
        void LogException(Exception exception);
        long getTimeSpan();
        List<CustomAddressModel> FetchAddress(int CustomerId, int CountryId, int StateId, string AddressValue);
        bool IsValidServiceForCustomer(int serviceId);
        List<SelectListItem> getWeightItem();
        List<SelectListItem> getInsuranceItems();
        List<SelectListItem> getKartonItems();
        List<SelectListItem> getCountryList();
        List<SelectListItem> getStateByCountryId(int countryId);
        Task<List<_ServiceInfo>> GetServiceInfo(int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int AproximateValue
            , int customerId
            , int height = 0
            , int length = 0
            , int width = 0
            , byte consType = 0
            , string content = ""
            , DateTime? dispach_date = null
            , string PackingOption = ""
            , string vechileType = ""
            , string VechileOption = ""
            , int receiver_ForeginCountry = 0
            , string receiver_ForeginCountryNameEn = ""
            , bool FromAPi = false
            , bool? IsCod = null
            , int serviceId = 0
            , bool IsFromAp = false
            , bool ShowPrivatePost = true
            , bool ShowDistributer = true
            , CustomAddressModel SenderAddress = null
            , CustomAddressModel ReciverAddress = null
            , bool IsFromUi=false
            , bool IsFromSep = false
            );

         Task<List<_ServiceInfo>> GetServiceInfoAnonymouse(int senderCountry
            , int senderState
            , int receiverCountry
            , int receiverState
            , int weightItem
            , int AproximateValue
            //, int customerId
            , int height = 0
            , int length = 0
            , int width = 0
            , byte consType = 0
            , string content = ""
            , DateTime? dispach_date = null
            , string PackingOption = ""
            , string vechileType = ""
            , string VechileOption = ""
            , int receiver_ForeginCountry = 0
            , string receiver_ForeginCountryNameEn = null
            , bool FromAPi = false//حق مقر شمرده نمیشود API در 
            , bool? IsCod = null
            , int serviceId = 0
            , bool IsFromAp = false
            , bool ShowPrivatePost = true
            , bool ShowDistributer = true
            , CustomAddressModel SenderAddress = null
            , CustomAddressModel ReciverAddress = null
            , bool IsFromUi = false
            , bool IsFromSep = false
            );
        PlaceOrderResult ProccessOrder(List<NewCheckoutModel> orderList, int CustomerId);
        BillAndPaymentModel GetFactorModel(List<int> OrderIds, bool safebuy = false);
        void InsertOrderNote(string note, int orderId);
        void Log(string header, string Message);
        int CalcCODPriceApi(Core.Domain.Catalog.Product product
           , int weight
           , string attributesXml
           , string userName
           , int countryId
           , int stateId
           , int postType
           , out string error);
        CheckoutPaymentMethodModel getPaymentMethodForSaleCarton();
        PlaceOrderResult ProccessCartonOrder(CartonSaleModel _model);

        CheckoutPaymentMethodModel GetPaymentMethodsForCODRequest();
        NewCheckout_Sp_Output CheckoutBySp(NewCheckout_Sp_Input model, OrderRegistrationMethod RegistrationMethod, int BulkOrderId,
            bool IsFromAp, bool isFromSep,bool IsForInternational=false);
    }
}
