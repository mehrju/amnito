using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Services.Localization;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nop.Plugin.Misc.PostbarDashboard.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IRepository<Order> _orderRepository;
        private readonly IRepository<OrderItem> _orderItemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<RewardPointsHistory> _rewardPointsHistory;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IPaymentService _paymentService;
        private readonly IDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderServices(
            IHttpContextAccessor httpContextAccessor,
            IRepository<Order> orderRepository,
            IRepository<OrderItem> orderItemRepository,
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<Category> categoryRepository,
            IRepository<RewardPointsHistory> rewardPointsHistory,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IPaymentService paymentService,
            IDbContext dbContext)
        {
            this._orderRepository = orderRepository;
            this._orderItemRepository = orderItemRepository; ;
            this._productRepository = productRepository;
            this._productCategoryRepository = productCategoryRepository;
            this._categoryRepository = categoryRepository;
            this._rewardPointsHistory = rewardPointsHistory;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._paymentService = paymentService;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCustomerOrderCount(int customerId)
        {
            var query = _orderRepository.Table;
            if (customerId > 0)
            {
                query = query.Where(o => o.CustomerId == customerId && !o.Deleted);
            }
            return query.Count();
        }

        public OrderModel GetCustomerLastOrder(int customerId)
        {
            var query = from order in _orderRepository.Table
                        join orderItem in _orderItemRepository.Table on order.Id equals orderItem.OrderId
                        join product in _productRepository.Table on orderItem.ProductId equals product.Id
                        join productCategory in _productCategoryRepository.Table on product.Id equals productCategory.ProductId
                        join category in _categoryRepository.Table on productCategory.CategoryId equals category.Id
                        where !order.Deleted && order.CustomerId == customerId
                        orderby order.CreatedOnUtc descending
                        select new
                        {
                            CategoryName = category.Name,
                            OrderTotal = order.OrderTotal,
                            OrderDate = order.CreatedOnUtc,
                            OrderId = order.Id,
                            OrderStatus = order.OrderStatusId,
                            PaymentStatus = order.PaymentMethodSystemName
                        };
            var result = query.FirstOrDefault();
            if (result != null)
            {
                return new OrderModel
                {
                    CategoryName = result.CategoryName,
                    OrderTotal = Convert.ToInt32(result.OrderTotal),
                    OrderDate = gerogorianDateToPersian(result.OrderDate),
                    OrderId = result.OrderId,
                    OrderStatus = ((OrderStatus)result.OrderStatus).GetLocalizedEnum(_localizationService, _workContext),
                    PaymentStatus = _paymentService.LoadPaymentMethodBySystemName(result.PaymentStatus) != null ?
                                _paymentService.LoadPaymentMethodBySystemName(result.PaymentStatus).PluginDescriptor.FriendlyName : result.PaymentStatus
                };
            }
            return null;
        }

        private string gerogorianDateToPersian(DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            return $"{persianCalendar.GetYear(dateTime)}/{persianCalendar.GetMonth(dateTime).ToString("00")}/{persianCalendar.GetDayOfMonth(dateTime).ToString("00")}";
        }
        public string getSubMarketFromUrl()
        {
            string host = _httpContextAccessor.HttpContext.Request.Host.Host.ToLower();
            string path = _httpContextAccessor.HttpContext.Request.Path.HasValue ? _httpContextAccessor.HttpContext.Request.Path.Value : "";
            if (host.Contains("postbar") || host.Contains("postbaar"))
                return "PostBar";
            else if (path.ToLower().Contains("/ap/"))
                return "Ap";
            else if (_httpContextAccessor.HttpContext.Request.Host.Host.ToLower().Contains("shipito"))
                return "Shipito";
            return "Shipito";
        }


        public IPagedList<Order> SearchOrders(int storeId = 0,
           int vendorId = 0, int customerId = 0,
           int productId = 0, int affiliateId = 0, int warehouseId = 0,
           int billingCountryId = 0, string paymentMethodSystemName = null,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
           List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
           string billingEmail = null, string billingLastName = "",
           string orderNotes = null, int pageIndex = 0, int pageSize = int.MaxValue
            , int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, string SenderName = null, int OrderIdFrom = 0, int OrderIdTo = 0, bool IsOrderOutDate = false, int orderState = 0, List<int> serviceTypes = null)
        {
            var query = _orderRepository.Table;
            if (storeId > 0)
                query = query.Where(o => o.StoreId == storeId);
            if (OrderIdFrom > 0)
            {
                query = query.Where(p => p.Id >= OrderIdFrom);
            }
            if (OrderIdTo > 0)
            {
                query = query.Where(p => p.Id <= OrderIdTo);
            }
            if (vendorId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.VendorId == vendorId));
            }
            if (customerId > 0)
                query = query.Where(o => o.CustomerId == customerId);
            if (productId > 0)
            {
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem => orderItem.Product.Id == productId));
            }
            if (warehouseId > 0)
            {
                var manageStockInventoryMethodId = (int)ManageInventoryMethod.ManageStock;
                query = query
                    .Where(o => o.OrderItems
                    .Any(orderItem =>
                        //"Use multiple warehouses" enabled
                        //we search in each warehouse
                        (orderItem.Product.ManageInventoryMethodId == manageStockInventoryMethodId &&
                        orderItem.Product.UseMultipleWarehouses &&
                        orderItem.Product.ProductWarehouseInventory.Any(pwi => pwi.WarehouseId == warehouseId))
                        ||
                        //"Use multiple warehouses" disabled
                        //we use standard "warehouse" property
                        ((orderItem.Product.ManageInventoryMethodId != manageStockInventoryMethodId ||
                        !orderItem.Product.UseMultipleWarehouses) &&
                        orderItem.Product.WarehouseId == warehouseId))
                        );
            }
            if (billingCountryId > 0)
                query = query.Where(o => o.BillingAddress != null && o.BillingAddress.CountryId == billingCountryId);
            if (!string.IsNullOrEmpty(paymentMethodSystemName))
                query = query.Where(o => o.PaymentMethodSystemName == paymentMethodSystemName);
            if (affiliateId > 0)
                query = query.Where(o => o.AffiliateId == affiliateId);
            if (createdFromUtc.HasValue)
                query = query.Where(o => createdFromUtc.Value <= o.CreatedOnUtc);
            if (createdToUtc.HasValue)
                query = query.Where(o => createdToUtc.Value >= o.CreatedOnUtc);
            if (osIds != null && osIds.Any())
                query = query.Where(o => osIds.Contains(o.OrderStatusId));
            if (psIds != null && psIds.Any())
                query = query.Where(o => psIds.Contains(o.PaymentStatusId));
            if (ssIds != null && ssIds.Any())
                query = query.Where(o => ssIds.Contains(o.ShippingStatusId));
            if (!string.IsNullOrEmpty(billingEmail))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.Email) && o.BillingAddress.Email.Contains(billingEmail));
            if (!string.IsNullOrEmpty(billingLastName))
                query = query.Where(o => o.BillingAddress != null && !string.IsNullOrEmpty(o.BillingAddress.LastName) && o.BillingAddress.LastName.Contains(billingLastName));
            if (!string.IsNullOrEmpty(orderNotes))
                query = query.Where(o => o.OrderNotes.Any(on => on.Note.Contains(orderNotes)));

            if (SenderCountryId > 0)
            {
                query = query.Where(p => p.BillingAddress != null && p.BillingAddress.CountryId == SenderCountryId);
            }
            if (SenderStateProvinceId > 0)
            {
                query = query.Where(p => p.BillingAddress != null && p.BillingAddress.StateProvinceId.HasValue && p.BillingAddress.StateProvinceId == SenderStateProvinceId);
            }
            if (ReciverCountryId > 0)
            {
                query = query.Where(p => p.ShippingAddress != null && p.ShippingAddress.CountryId == ReciverCountryId);
            }
            if (ReciverStateProvinceId > 0)
            {
                query = query.Where(p => p.ShippingAddress != null && p.ShippingAddress.StateProvinceId == ReciverStateProvinceId);
            }
            if (!string.IsNullOrEmpty(ReciverName))
            {
                query = query.Where(p => p.ShippingAddress != null
                && (!string.IsNullOrEmpty(p.ShippingAddress.FirstName) || !string.IsNullOrEmpty(p.ShippingAddress.LastName))
                && ((p.ShippingAddress.FirstName ?? string.Empty) + " " + (p.ShippingAddress.LastName ?? string.Empty)).Contains(ReciverName));
            }
            if (!string.IsNullOrEmpty(SenderName))
            {
                query = query.Where(p => p.BillingAddress != null
                && (!string.IsNullOrEmpty(p.BillingAddress.FirstName) || !string.IsNullOrEmpty(p.BillingAddress.LastName))
                && ((p.BillingAddress.FirstName ?? string.Empty) + " " + (p.BillingAddress.LastName ?? string.Empty)).Contains(SenderName));
            }

            if (serviceTypes != null && serviceTypes.Any())
            {
                query = query.Where(p => p.OrderItems.Any(q => serviceTypes.Contains(q.Product.ProductCategories.FirstOrDefault().CategoryId)));
            }

            query = query.Where(o => !o.Deleted);
            query = query.OrderByDescending(o => o.CreatedOnUtc);

            //database layer paging
            var total = query.Count();

            query = query.Skip(pageIndex * pageSize).Take(pageSize);

            return new PagedList<Order>(query.ToList(), pageIndex, pageSize, total);
        }

        public IList<OrderBillDetail> SearchOrderBillDetail(int pageIndex = 0, int pageSize = int.MaxValue,
            int customerId = 0, int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, int OrderIdFrom = 0, int OrderIdTo = 0, int PaymentStatus = 0, int OrderStatus = 0,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> ServiceTypes = null)
        {
            var prms = new List<SqlParameter>();

            prms.Add(new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = customerId
            });
            prms.Add(new SqlParameter()
            {
                ParameterName = "PageIndex",
                SqlDbType = SqlDbType.Int,
                Value = pageIndex
            });
            prms.Add(new SqlParameter()
            {
                ParameterName = "PageSize",
                SqlDbType = SqlDbType.Int,
                Value = pageSize
            });


            //if (OrderId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderIdFrom",
                SqlDbType = SqlDbType.Int,
                Value = OrderIdFrom
            });
            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderIdTo",
                SqlDbType = SqlDbType.Int,
                Value = OrderIdTo
            });
            //}
            //if(OrderStatus != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderStatus",
                SqlDbType = SqlDbType.Int,
                Value = OrderStatus
            });
            //}
            //if(PaymentStatus != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "PayStatus",
                SqlDbType = SqlDbType.Int,
                Value = PaymentStatus
            });
            //}
            //if (!string.IsNullOrEmpty(ReciverName))
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverName",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(ReciverName) ? DBNull.Value : (object)ReciverName
            });
            //}
            //if (ReciverCountryId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverProvinceId",
                SqlDbType = SqlDbType.Int,
                Value = ReciverCountryId
            });
            //}
            //if (ReciverStateProvinceId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverCityId",
                SqlDbType = SqlDbType.Int,
                Value = ReciverStateProvinceId
            });
            //}
            //if (SenderCountryId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "SenderProvinceId",
                SqlDbType = SqlDbType.Int,
                Value = SenderCountryId
            });
            //}
            //if (SenderStateProvinceId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "SenderCityId",
                SqlDbType = SqlDbType.Int,
                Value = SenderStateProvinceId
            });
            //}


            //if (createdFromUtc.HasValue)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "FromDate",
                SqlDbType = SqlDbType.Date,
                Value = (object)createdFromUtc ?? DBNull.Value
            });
            //}
            //if (createdToUtc.HasValue)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "ToDate",
                SqlDbType = SqlDbType.Date,
                Value = (object)createdToUtc ?? DBNull.Value
            });

            prms.Add(new SqlParameter()
            {
                ParameterName = "ServiceTypes",
                SqlDbType = SqlDbType.NVarChar,
                Value = (ServiceTypes != null && ServiceTypes.Any()) ? (object)string.Join(",", ServiceTypes) : DBNull.Value
            });
            //}

            string Query = $@"EXECUTE [dbo].[Sp_OrderBillDetails_Customer] {string.Join(", ", prms.Select(p => "@" + p.ParameterName))}";

            return _dbContext.SqlQuery<OrderBillDetail>(Query, prms.ToArray()).ToList();
        }


        public IList<OrderTrackingBarcode> SearchOrderBarcode(int pageIndex = 0, int pageSize = int.MaxValue,
            int customerId = 0, int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
           string ReciverName = null, int OrderIdFrom = 0, int OrderIdTo = 0, int PaymentStatus = 0, int OrderStatus = 0,
           DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> ServiceTypes = null)
        {
            var prms = new List<SqlParameter>();

            prms.Add(new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = customerId
            });
            prms.Add(new SqlParameter()
            {
                ParameterName = "PageIndex",
                SqlDbType = SqlDbType.Int,
                Value = pageIndex
            });
            prms.Add(new SqlParameter()
            {
                ParameterName = "PageSize",
                SqlDbType = SqlDbType.Int,
                Value = pageSize
            });


            //if (OrderId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderIdFrom",
                SqlDbType = SqlDbType.Int,
                Value = OrderIdFrom
            });

            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderIdTo",
                SqlDbType = SqlDbType.Int,
                Value = OrderIdTo
            });
            //}
            //if(OrderStatus != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderStatus",
                SqlDbType = SqlDbType.Int,
                Value = OrderStatus
            });
            //}
            //if(PaymentStatus != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "PayStatus",
                SqlDbType = SqlDbType.Int,
                Value = PaymentStatus
            });
            //}
            //if (!string.IsNullOrEmpty(ReciverName))
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverName",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(ReciverName) ? DBNull.Value : (object)ReciverName
            });
            //}
            //if (ReciverCountryId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverProvinceId",
                SqlDbType = SqlDbType.Int,
                Value = ReciverCountryId
            });
            //}
            //if (ReciverStateProvinceId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverCityId",
                SqlDbType = SqlDbType.Int,
                Value = ReciverStateProvinceId
            });
            //}
            //if (SenderCountryId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "SenderProvinceId",
                SqlDbType = SqlDbType.Int,
                Value = SenderCountryId
            });
            //}
            //if (SenderStateProvinceId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "SenderCityId",
                SqlDbType = SqlDbType.Int,
                Value = SenderStateProvinceId
            });
            //}


            //if (createdFromUtc.HasValue)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "FromDate",
                SqlDbType = SqlDbType.Date,
                Value = (object)createdFromUtc ?? DBNull.Value
            });
            //}
            //if (createdToUtc.HasValue)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "ToDate",
                SqlDbType = SqlDbType.Date,
                Value = (object)createdToUtc ?? DBNull.Value
            });

            prms.Add(new SqlParameter()
            {
                ParameterName = "ServiceTypes",
                SqlDbType = SqlDbType.NVarChar,
                Value = (ServiceTypes != null && ServiceTypes.Any()) ? (object)string.Join(",", ServiceTypes) : DBNull.Value
            });

            //}

            string Query = $@"EXECUTE [dbo].[Sp_OrderTrackingBarcode] {string.Join(", ", prms.Select(p => "@" + p.ParameterName))}";

            return _dbContext.SqlQuery<OrderTrackingBarcode>(Query, prms.ToArray()).ToList();
        }

        public IList<SearchedOrder> SearchOrders(int pageIndex = 0, int pageSize = int.MaxValue,
          int customerId = 0, int SenderStateProvinceId = 0, int SenderCountryId = 0, int ReciverCountryId = 0, int ReciverStateProvinceId = 0,
         string ReciverName = null, int OrderIdFrom = 0, int OrderIdTo = 0, int PaymentStatus = 0, int OrderStatus = 0,
         DateTime? createdFromUtc = null, DateTime? createdToUtc = null, List<int> ServiceTypes = null)
        {
            var prms = new List<SqlParameter>();

            prms.Add(new SqlParameter()
            {
                ParameterName = "CustomerId",
                SqlDbType = SqlDbType.Int,
                Value = customerId
            });
            prms.Add(new SqlParameter()
            {
                ParameterName = "PageIndex",
                SqlDbType = SqlDbType.Int,
                Value = pageIndex
            });
            prms.Add(new SqlParameter()
            {
                ParameterName = "PageSize",
                SqlDbType = SqlDbType.Int,
                Value = pageSize
            });


            //if (OrderId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderIdFrom",
                SqlDbType = SqlDbType.Int,
                Value = OrderIdFrom
            });

            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderIdTo",
                SqlDbType = SqlDbType.Int,
                Value = OrderIdTo
            });
            //}
            //if(OrderStatus != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "OrderStatus",
                SqlDbType = SqlDbType.Int,
                Value = OrderStatus
            });
            //}
            //if(PaymentStatus != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "PayStatus",
                SqlDbType = SqlDbType.Int,
                Value = PaymentStatus
            });
            //}
            //if (!string.IsNullOrEmpty(ReciverName))
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverName",
                SqlDbType = SqlDbType.NVarChar,
                Value = string.IsNullOrEmpty(ReciverName) ? DBNull.Value : (object)ReciverName
            });
            //}
            //if (ReciverCountryId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverProvinceId",
                SqlDbType = SqlDbType.Int,
                Value = ReciverCountryId
            });
            //}
            //if (ReciverStateProvinceId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "RecieverCityId",
                SqlDbType = SqlDbType.Int,
                Value = ReciverStateProvinceId
            });
            //}
            //if (SenderCountryId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "SenderProvinceId",
                SqlDbType = SqlDbType.Int,
                Value = SenderCountryId
            });
            //}
            //if (SenderStateProvinceId != 0)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "SenderCityId",
                SqlDbType = SqlDbType.Int,
                Value = SenderStateProvinceId
            });
            //}


            //if (createdFromUtc.HasValue)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "FromDate",
                SqlDbType = SqlDbType.Date,
                Value = (object)createdFromUtc ?? DBNull.Value
            });
            //}
            //if (createdToUtc.HasValue)
            //{
            prms.Add(new SqlParameter()
            {
                ParameterName = "ToDate",
                SqlDbType = SqlDbType.Date,
                Value = (object)createdToUtc ?? DBNull.Value
            });

            prms.Add(new SqlParameter()
            {
                ParameterName = "ServiceTypes",
                SqlDbType = SqlDbType.NVarChar,
                Value = (ServiceTypes != null && ServiceTypes.Any()) ? (object)string.Join(",", ServiceTypes) : DBNull.Value
            });

            //}

            string Query = $@"EXECUTE [dbo].[Sp_SearchOrders] {string.Join(", ", prms.Select(p => "@" + p.ParameterName))}";

            return _dbContext.SqlQuery<SearchedOrder>(Query, prms.ToArray()).ToList();
        }
        public OrdersSum GetOrdersSumByCustomer(int customerId)
        {
            var query = $"SELECT CAST(SUM(OrderDiscount) AS BIGINT) DiscountSum,CAST(SUM(OrderTotal) AS BIGINT) OrderTotal FROM [Order] WHERE CustomerId = {customerId}";
            return _dbContext.SqlQuery<OrdersSum>(query).FirstOrDefault();
        }
    }
}