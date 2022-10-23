using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Orders;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class AgentAmountRuleService : IAgentAmountRuleService
    {
        private readonly IRepository<AgentAmountRuleModel> _repositoryAgentAmountRule;
        private readonly IRepository<AssignAgentAmountRuleModel> _repositoryCustomerAgentAssignment;
        private readonly IDbContext _dbContext;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IStoreContext _StoreContext;
        private readonly IAccountingService _accountingService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IWebHelper _webHelper;
        private readonly IChargeWalletFailService _chargeWalletFailService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IRepository<OrderNote> _orderNoteRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        public AgentAmountRuleService(IRepository<AgentAmountRuleModel> repositoryAgentAmountRule
        , IRepository<AssignAgentAmountRuleModel> repositoryCustomerAgentAssignment
        , IDbContext dbContext
        , IWorkContext workContext
        , ICustomerService customerService
        , IGenericAttributeService genericAttributeService
        , IStoreContext StoreContext
        , IRepository<GenericAttribute> genericAttributeRepository
        , IAccountingService accountingService
        , IProductAttributeParser productAttributeParser
        , IWebHelper webHelper
        , IChargeWalletFailService chargeWalletFailService
        , IRepository<OrderNote> orderNoteRepository
        , IRewardPointService rewardPointService
        )
        {
            _rewardPointService = rewardPointService;
            _accountingService = accountingService;
            _genericAttributeRepository = genericAttributeRepository;
            _customerService = customerService;
            _dbContext = dbContext;
            _repositoryAgentAmountRule = repositoryAgentAmountRule;
            _repositoryCustomerAgentAssignment = repositoryCustomerAgentAssignment;
            _workContext = workContext;
            _StoreContext = StoreContext;
            _genericAttributeService = genericAttributeService;
            _productAttributeParser = productAttributeParser;
            _webHelper = webHelper;
            _chargeWalletFailService = chargeWalletFailService;
            _orderNoteRepository = orderNoteRepository;
        }

        public bool SaveAgentAmountRule(AgentAmountRuleModel model, out string error)
        {
            model.CreateCustomerId = _workContext.CurrentCustomer.Id;
            model.CreateDate = DateTime.Now;
            error = "";
            if (model.CreateCustomerId == 0)
            {
                error = "کاربر ایجاد کننده قابل تشخیص نیست";
                return false;
            }
            if (model.Price <= 0)
            {
                error = "مبلغ وارد شده برای حق نمایندگی صحیح نمی باشد";
                return false;
            }
            if (model.ProductAttributeId == 0)
            {
                error = "ویژگی محصول انتخاب شده نا معتبر می باشد";
                return false;
            }
            if (model.ProductAttributeValueId == 0)
            {
                error = "مقدار ویژگی محصول انتخاب شده نا معتبر می باشد";
                return false;
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                error = "عنوان را وارد کنید";
                return false;
            }
            if (model.ProductId == 0)
            {
                error = "کالای مورد نظر را وارد نمایید";
                return false;
            }
            if ((model.MinCount == 0 && model.MaxCount == 0) || (model.MaxCount < model.MinCount))
            {
                error = "مقدار وارد شده برای تعداد ها نامعتبر می باشد";
                return false;
            }
            _repositoryAgentAmountRule.Insert(model);
            error = "اطلاعات با موفقیت ذخیره شد";
            return true;
        }

        public bool DdeleteAgentAmountRule(int agentAmountRuleId)
        {
            var data = _repositoryAgentAmountRule.Table.SingleOrDefault(p => p.Id == agentAmountRuleId);
            data.DeleteCustomerId = _workContext.CurrentCustomer.Id;
            data.DeletedDate = DateTime.Now;
            _repositoryAgentAmountRule.Update(data);
            return true;
        }

        public List<AgentAmountRuleModel> getList()
        {
            string query = @"SELECT
                                TAAR.Id
                                    , TAAR.Name
                                    , TAAR.ProductId
                                    , TAAR.ProductAttributeId
                                    , TAAR.ProductAttributeValueId
                                    , TAAR.Price
                                    , CreateDate
                                    , TAAR.CreateCustomerId
                                    , TAAR.DeletedDate
                                    , TAAR.DeleteCustomerId
                                    , p.Name ProductName
                                    , pa.Name ProductAttributeName
                                    , pav.Name ProductAttributeValueName
                                    , TAAR.MinCount
                                    , TAAR.MaxCount
                                FROM
                                    dbo.Tb_AgentAmountRule AS TAAR
                                    INNER JOIN dbo.Customer AS C ON C.Id = TAAR.CreateCustomerId
                                    LEFT JOIN dbo.Product AS P ON P.Id = TAAR.ProductId
                                    LEFT JOIN dbo.ProductAttribute AS PA ON PA.Id = TAAR.ProductAttributeId
                                    LEFT JOIN dbo.ProductAttributeValue AS PAV ON PAV.Id = TAAR.ProductAttributeValueId
                                    LEFT JOIN dbo.Customer AS Cd ON cd.Id = TAAR.DeleteCustomerId
                                WHERE
                                    TAAR.DeletedDate IS NULL
                            ORDER BY 
                                TAAR.Id DESC ";
            return _dbContext.SqlQuery<AgentAmountRuleModel>(query, new object[0]).ToList();
        }

        public List<AssignAgentAmountRuleModel> getAssignAgentAmountList(AssignAgentAmountRuleModel model)
        {
            string where = "";
            where = " AND TCAAR.IsActive = " + (model.IsActive ? "1" : "0");
            if (model.CustomerId != 0)
                where += " AND TCAAR.CustomerId = " + model.CustomerId;
            if (model.AgentAmountRuleId != 0)
                where += " AND TAAR.Id = " + model.AgentAmountRuleId;

            string query = $@"SELECT
	                            C.Email+'_'+C.Username CustmoerName,
	                            TAAR.Name AgentAmountRuleName,
	                            P.Name+'-'+PA.Name+'-'+PAV.Name AgentAmountRuleTitle,
	                            TAAR.Price,
	                            TCAAR.Id,
	                            CASE WHEN TCAAR.IsActive = 1 THEN N'بلی' ELSE N'خیر' END StrIsActive,
                                TCAAR.AssignmentDate,
                                TCAAR.IsActive,
                                TCAAR.CustomerId,
                                TAAR.MinCount,
                                TAAR.MaxCount
                            FROM
	                            dbo.Tb_CustomerAgentAssignmentRule AS TCAAR
	                            INNER JOIN dbo.Tb_AgentAmountRule AS TAAR ON TAAR.Id = TCAAR.AgentAmountRuleId
	                            INNER JOIN dbo.Customer AS C ON C.Id = TCAAR.CustomerId
	                            INNER JOIN dbo.Product AS P ON P.Id = TAAR.ProductId
	                            INNER JOIN dbo.ProductAttribute AS PA ON pa.Id = TAAR.ProductAttributeId
	                            INNER JOIN dbo.ProductAttributeValue AS PAV ON pav.Id = TAAR.ProductAttributeValueId
                            WHERE
	                            TAAR.DeletedDate IS NULL
                                {where}
                            ORDER BY TCAAR.ID";
            return _dbContext.SqlQuery<AssignAgentAmountRuleModel>(query, new object[0]).ToList();
        }
        public bool SaveAssignAgentAmountRule(AssignAgentAmountRuleModel model, out string error)
        {
            if (model.CustomerId == 0)
            {
                error = "نماینده مورد نظر را انتخاب کنید";
                return false;
            }
            if (model.AgentAmountRuleId == 0)
            {
                error = "قانون نمایندگی مورد نظر را انتخاب کنید";
                return false;
            }
            string AgentAmountRuleName = CanAssign(model.CustomerId, model.AgentAmountRuleId);
            if (!string.IsNullOrEmpty(AgentAmountRuleName))
            {
                error = $"قانون {AgentAmountRuleName} برای این نماینده با مشخصات مشابه قانون انتخابی شما فعال می باشد";
                return false;
            }
            model.AssignmentCustomerId = model.CustomerId;//_workContext.CurrentCustomer.Id;
            model.AssignmentDate = DateTime.Now;
            _repositoryCustomerAgentAssignment.Insert(model);
            error = "عملیات با موفقیت انجام شد";
            return true;

        }
        private string CanAssign(int CustomerId, int AgentAmountRuleId)

        {
            SqlParameter[] prms = new SqlParameter[]{
                new SqlParameter() { ParameterName = "CustomerId", SqlDbType = SqlDbType.Int, Value = CustomerId },
                new SqlParameter() { ParameterName = "AgentAmoutnRuleId", SqlDbType = SqlDbType.Int, Value = AgentAmountRuleId },
                new SqlParameter() { ParameterName = "productId", SqlDbType = SqlDbType.Int, Value = 0 },
                new SqlParameter() { ParameterName = "productAttributeId", SqlDbType = SqlDbType.Int, Value = 0 },
                new SqlParameter() { ParameterName = "productAttributeValueId", SqlDbType = SqlDbType.Int, Value = 0 },
                new SqlParameter() { ParameterName = "minCount", SqlDbType = SqlDbType.Int, Value = 0 },
                new SqlParameter() { ParameterName = "maxCount", SqlDbType = SqlDbType.Int, Value = 0 }

            };
            string query = @"SELECT
	                            @productId = TAAR.ProductId,
	                            @productAttributeId = TAAR.ProductAttributeId,
	                            @productAttributeValueId = TAAR.ProductAttributeValueId,
	                            @minCount = TAAR.MinCount,
	                            @maxCount = TAAR.MaxCount
                            FROM
	                            dbo.Tb_AgentAmountRule AS TAAR
                            WHERE
	                            TAAR.Id = @AgentAmoutnRuleId

                            SELECT
	                            TOP(1)TAAR.Name
                            FROM
	                            dbo.Tb_CustomerAgentAssignmentRule AS TCAAR
	                            INNER JOIN dbo.Tb_AgentAmountRule AS TAAR ON TAAR.Id = TCAAR.AgentAmountRuleId
                            WHERE
	                            TCAAR.CustomerId = @CustomerId
	                            AND TAAR.ProductId = @productId
	                            AND TAAR.ProductAttributeId = @productAttributeId
	                            AND TAAR.ProductAttributeValueId = @productAttributeValueId
	                            AND TAAR.MinCount = @minCount
	                            AND TAAR.MaxCount = @maxCount
	                            AND TAAR.DeletedDate IS NULL 
                                AND TCAAR.IsActive = 1";
            return _dbContext.SqlQuery<string>(query, prms).FirstOrDefault();
        }
        public void DeActiveAssignAgentAmountRule(int AssignAgentAmountRuleId, out string error)
        {
            if (AssignAgentAmountRuleId == 0)
            {
                error = "شناسه نامعتبر میباشد لطفا مجددا سعی کنید";
                return;
            }
            var data = _repositoryCustomerAgentAssignment.Table.SingleOrDefault(p => p.Id == AssignAgentAmountRuleId);
            if (data == null)
            {
                error = "شناسه نامعتبر میباشد لطفا مجددا سعی کنید";
                return;
            }
            if (!data.IsActive)
            {
                string AgentAmountRuleName = CanAssign(data.CustomerId, data.AgentAmountRuleId);
                if (!string.IsNullOrEmpty(AgentAmountRuleName))
                {
                    error = $"قانون {AgentAmountRuleName} برای این نماینده با مشخصات مشابه قانون انتخابی شما فعال می باشد";
                    return;
                }
            }
            data.DeActiveDate = DateTime.Now;
            data.DeActiveCustomerId = _workContext.CurrentCustomer.Id;
            data.IsActive = !data.IsActive;
            _repositoryCustomerAgentAssignment.Update(data);
            error = "عملیات با موفقیت انجام شد";
        }
        public bool updateAgentAmountRule(AgentAmountRuleModel model, out string error)
        {
            if (model.Id == 0)
            {
                error = "شناسه قانون نمایندگی جهت به روز رسانی معتبر نمی باشد";
                return false;
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                error = "وارد کردن عنوان قانون حق نمایندگی الزامی می باشد";
                return false;
            }
            var data = _repositoryAgentAmountRule.Table.Single(p => p.Id == model.Id);
            data.Name = model.Name;
            _repositoryAgentAmountRule.Update(data);
            error = "ویرایش با موفقیت انجام شد";
            return true;
        }
        public List<SelectListItem> getAvailableAgentList(string userName)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "0", Text = "انتخاب کنید....", Selected = true });
            items.AddRange(_customerService.GetAllCustomers(username: userName).Select(p => new SelectListItem()
            {
                Text = (p.Username ?? "") + "-" + (p.GetFullName() ?? ""),
                Value = p.Id.ToString()
            }).ToList());
            return items;
        }
        public List<SelectListItem> getAvailableAgentAmountRule()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Value = "0", Text = "انتخاب کنید....", Selected = true });
            items.AddRange(_repositoryAgentAmountRule.Table.Where(p => p.DeletedDate == null).Select(p => new SelectListItem()
            {
                Text = p.Name + " " + p.Id.ToString(),
                Value = p.Id.ToString()
            }).ToList());
            return items;
        }
        public bool getAgentAmountRuleEnable(string name)
        {
            var result = _genericAttributeRepository.Table.Where(p => p.Key == name && p.KeyGroup == "Store").OrderByDescending(p => p.Id).FirstOrDefault()?.Value;
            if (result == null)
                return false;
            return bool.Parse(result);
        }

        public bool IsCustmoerInAgentRole(int customerId)
        {
            return (_repositoryCustomerAgentAssignment.Table.Any(p => p.CustomerId == customerId && p.IsActive == true) || IsCustomerInPrivcatePostDiscount(customerId));
        }
        public bool IsCustomerInPrivcatePostDiscount(int customerId)
        {
            string query = $@"IF EXISTS(
                            SELECT
	                            TOP(1) 1
                            FROM
	                            dbo.Tb_PrivatePostDiscount AS TPPD
                            WHERE
	                            TPPD.CustomerId = {customerId}
	                            AND TPPD.IsActive = 1)
                            BEGIN
                                SELECT CAST(1 AS BIT) hasDiscount
                            END
                            ELSE
                            BEGIN
                                SELECT CAST(0 AS BIT) hasDiscount
                            END";

            return _dbContext.SqlQuery<bool?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(false);
        }
        public PrivatePostDiscount GetPrivatePostDiscount(int CostomerId, int ServiceId)
        {
            string query = $@"SELECT TOP(1)
	                        ISNULL(ISNULL(TPPD.DiscountPercent,TPPD.DiscountPrice),0) DisCountAmount,
	                        CAST(CASE WHEN TPPD.DiscountPercent IS not NULL AND ISNULL(TPPD.DiscountPrice,0) = 0 THEN 1 ELSE 0 END AS BIT) IsPercent
                        FROM
	                        dbo.Tb_PrivatePostDiscount AS TPPD
                        WHERE
	                        TPPD.CustomerId = {CostomerId}
	                        AND TPPD.IsActive = 1
	                        AND TPPD.ServiceId = {ServiceId} ORDER BY id DESC";
            return _dbContext.SqlQuery<PrivatePostDiscount>(query, new object[0]).FirstOrDefault();
        }
        public int GetPrivatePostDiscountForCustomer(int CustomerId, int ServiceId)
        {
            string query = $@"SELECT TOP(1)
	                            TPPD.DiscountPercent
                            FROM
	                            dbo.Tb_CustomerPrivatePostDiscount AS TPPD
                            WHERE
	                            TPPD.CustomerId = {CustomerId}
	                            AND CAST(TPPD.ExpireDate AS DATE) >= CAST(GETDATE() AS DATE)
	                            AND TPPD.ServiceId = {ServiceId} ORDER BY id DESC";
            return _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
        }
        /// <summary>
        /// محسابه خدمات نمایندگی درخواستی از سمت نماینده
        /// </summary>
        /// <param name="order"></param>
        public void CalcOutLineAgentSaleAmount(Order order)
        {
            try
            {
                if (_accountingService.HasChargeWallethistory(3, order.Id))
                    return;
                int AgentSaleAmount = 0;
                foreach (var item in order.OrderItems)
                {
                    if (!_accountingService.IsChargedWalletForAgentAMountRule(item.Id, 3))
                    {
                        var Lst_PAM = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml);
                        if (Lst_PAM.Any(p => p.ProductAttribute.Name.Contains("ارزش افزوده")))
                        {
                            var pam = Lst_PAM.FirstOrDefault(p => p.ProductAttribute.Name.Contains("ارزش افزوده"));

                            var txtPrice = _productAttributeParser.ParseValues(item.AttributesXml, pam.Id).FirstOrDefault();
                            AgentSaleAmount += txtPrice.ToEnDigit() * item.Quantity;
                        }
                    }
                }
                if (AgentSaleAmount > 0)
                {
                    string Str_error = "";
                    AgentSaleAmount -= (AgentSaleAmount * 10) / 100;
                    int rewardPointHistoryId = ChargeWalletForAgentSaleAmount(order, AgentSaleAmount, "", out Str_error);
                    _accountingService.InsertChargeWallethistory(new Models.ChargeWalletHistoryModel()
                    {
                        orderId = order.Id,
                        orderItemId = null,
                        rewaridPointHistoryId = rewardPointHistoryId,

                        shipmentId = null,
                        AgentAmountRuleId = null,
                        ChargeWalletTypeId = 3,
                        Description = "واریز مبلغ خدمات نمایندگی برای سفارش شماره " + " " + order.Id + " پس از کسر 10 درصد حق مشارکت امنیتو",
                        Point = AgentSaleAmount,
                        IpAddress = _webHelper.GetCurrentIpAddress(),
                        CreateDate = DateTime.Now
                    });
                    InsertOrderNote(Str_error, order.Id);
                }
            }
            catch (Exception ex)
            {
                common.LogException(ex);
                _chargeWalletFailService.InsertFailedLog(ex, new { orderId = order.Id }, "OrderPaidEventConsumer.CalcOutLineAgentSaleAmount  -> محسابه خدمات نمایندگی درخواستی از سمت نماینده");
                throw;
            }

        }
        /// <summary>
        /// محسابه حق نمایندگی بر اساس قانون حق نمایندگی
        /// </summary>
        /// <param name="order"></param>
        public InlineAgentSaleAMount getInlineAgentSaleAmount(OrderItem item, out PrivatePostDiscount privatePostDiscount)
        {
            try
            {
                privatePostDiscount = null;
                int serviceId = item.Product.ProductCategories.First().CategoryId;
                if (new int[] { 654, 655, 662, 725, 726, 727 }.Contains(serviceId))
                {
                    var PAM = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml).Select(p => p.ProductAttribute.Id).ToList();
                    string PaIds = string.Join(",", PAM);
                    var PAVs = _productAttributeParser.ParseProductAttributeValues(item.AttributesXml).ToList();
                    string PavIds = string.Join(",", PAVs.Select(p => p.Id).ToList());
                    if (string.IsNullOrEmpty(PaIds) || string.IsNullOrEmpty(PavIds))
                    {
                        string MsgNote = $"اطلاعات ویژگی و مقادیر ویژگی برای این آیتم نا معتبر می باشد" + "---" + item.Product.Name;
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    string query = @"SELECT
	                            TAAR.Price,
                                TCAAR.AgentAmountRuleId
                            FROM
	                            dbo.Tb_CustomerAgentAssignmentRule AS TCAAR
	                            INNER JOIN dbo.Tb_AgentAmountRule AS TAAR ON TAAR.Id = TCAAR.AgentAmountRuleId
                            WHERE
	                            TAAR.DeletedDate IS NULL
	                            AND TCAAR.IsActive = 1
	                            AND TAAR.ProductId = " + item.ProductId + @"
	                            AND TCAAR.CustomerId = " + item.Order.CustomerId + @"
	                            AND TAAR.ProductAttributeId IN (" + PaIds + @")
	                            AND TAAR.ProductAttributeValueId IN (" + PavIds + ")";

                    var LstPrice = _dbContext.SqlQuery<InlineAgentSaleAMount>(query, new object[0]).ToList();
                    if (LstPrice.Count() > 1)
                    {
                        string MsgNote = $"بیش از یک قانون حق نمایندگی در یک وزن برای نماینده این سفارش  تعریف شده است." + "---" + item.Product.Name;
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    var data = LstPrice.First();
                    data.Price = data.Price * item.Quantity;
                    return data;
                }
                else //پست خصوصی
                {
                    PrivatePostDiscount discount = GetPrivatePostDiscount(item.Order.CustomerId, serviceId);
                    privatePostDiscount = discount;
                    if (discount == null)
                    {
                        string MsgNote = $"در این سرویس برای مشتری تخفیفی تعریف نشده";
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    if (new int[] { 722, 723, 667, 670 }.Contains(serviceId))
                    {
                        var engAndPrice = getGatwayEngPrice(item);
                        if (engAndPrice == 0)
                        {
                            string MsgNote = $"قیمت های ایتم ${item.Id} در این سفارش به درستی محاسته نشده و قانون حق نمایندگی برای آن محاسبه نشد" + "---" + item.Product.Name;
                            InsertOrderNote(MsgNote, item.OrderId);
                            return null;
                        }
                        int discountPrice = 0;
                        if (discount.IsPercent)
                        {
                            discountPrice = ((engAndPrice * discount.DisCountAmount) / 100) * item.Quantity;
                        }
                        else
                        {
                            discountPrice = discount.DisCountAmount * item.Quantity;
                        }
                        //if (discountPrice > 0)
                        //    discountPrice -= (discountPrice * 9 / 100);
                        InlineAgentSaleAMount ilineAgentSaleAMount = new InlineAgentSaleAMount()
                        {
                            AgentAmountRuleId = null,
                            Price = discountPrice
                        };
                        return ilineAgentSaleAMount;
                    }
                    else
                    {
                        var engAndPrice = getEngAndPostPrice(item);
                        if (engAndPrice == null)
                        {
                            string MsgNote = $"قیمت های ایتم ${item.Id} در این سفارش به درستی محاسته نشده و قانون حق نمایندگی برای آن محاسبه نشد" + "---" + item.Product.Name;
                            InsertOrderNote(MsgNote, item.OrderId);
                            return null;
                        }
                        int discountPrice = 0;
                        if (discount.IsPercent)
                        {
                            discountPrice = ((engAndPrice.IncomePrice * discount.DisCountAmount) / 100) * item.Quantity;
                        }
                        else
                        {
                            discountPrice = discount.DisCountAmount * item.Quantity;
                        }
                        InlineAgentSaleAMount ilineAgentSaleAMount = new InlineAgentSaleAMount()
                        {
                            AgentAmountRuleId = null,
                            Price = discountPrice
                        };
                        return ilineAgentSaleAMount;
                    }
                }
            }
            catch (Exception ex)
            {
                privatePostDiscount = null;
                InsertOrderNote($"خطا در زمان محاسبه تخفیف حق نمایندگی برای آیتم {item.Id} به وجود آمده" + "\r\n" +
                    ex.Message + (ex.InnerException != null ? "--> " + ex.InnerException.Message : ""), item.OrderId);
                return null;
            }
        }
        public InlineAgentSaleAMount getInlineCustomerDiscountForShipment(OrderItem item, out PrivatePostDiscount privatePostDiscount)
        {
            try
            {
                privatePostDiscount = null;
                int serviceId = item.Product.ProductCategories.First().CategoryId;
                if (new int[] { 654, 655, 662, 725, 726, 727 }.Contains(serviceId))
                {
                    var PAM = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml).Select(p => p.ProductAttribute.Id).ToList();
                    string PaIds = string.Join(",", PAM);
                    var PAVs = _productAttributeParser.ParseProductAttributeValues(item.AttributesXml).ToList();
                    string PavIds = string.Join(",", PAVs.Select(p => p.Id).ToList());
                    if (string.IsNullOrEmpty(PaIds) || string.IsNullOrEmpty(PavIds))
                    {
                        string MsgNote = $"اطلاعات ویژگی و مقادیر ویژگی برای این آیتم نا معتبر می باشد" + "---" + item.Product.Name;
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    string query = @"SELECT
	                            TAAR.Price,
                                TCAAR.AgentAmountRuleId
                            FROM
	                            dbo.Tb_CustomerAgentAssignmentRule AS TCAAR
	                            INNER JOIN dbo.Tb_AgentAmountRule AS TAAR ON TAAR.Id = TCAAR.AgentAmountRuleId
                            WHERE
	                            TAAR.DeletedDate IS NULL
	                            AND TCAAR.IsActive = 1
	                            AND TAAR.ProductId = " + item.ProductId + @"
	                            AND TCAAR.CustomerId = " + item.Order.CustomerId + @"
	                            AND TAAR.ProductAttributeId IN (" + PaIds + @")
	                            AND TAAR.ProductAttributeValueId IN (" + PavIds + ")";

                    var LstPrice = _dbContext.SqlQuery<InlineAgentSaleAMount>(query, new object[0]).ToList();
                    if (LstPrice.Count() > 1)
                    {
                        string MsgNote = $"بیش از یک قانون حق نمایندگی در یک وزن برای نماینده این سفارش  تعریف شده است." + "---" + item.Product.Name;
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    var data = LstPrice.First();
                    data.Price = data.Price * item.Quantity;
                    return data;
                }
                else //پست خصوصی
                {
                    int discountPercent = GetPrivatePostDiscountForCustomer(item.Order.CustomerId, serviceId);

                    if (discountPercent == 0)
                    {
                        string MsgNote = $"در این سرویس برای مشتری تخفیفی تعریف نشده";
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    if (new int[] { 722, 723, 667, 670 }.Contains(serviceId))
                    {
                        var engAndPrice = getGatwayEngPrice(item);
                        if (engAndPrice == 0)
                        {
                            string MsgNote = $"قیمت های ایتم ${item.Id} در این سفارش به درستی محاسته نشده و قانون حق نمایندگی برای آن محاسبه نشد" + "---" + item.Product.Name;
                            InsertOrderNote(MsgNote, item.OrderId);
                            return null;
                        }
                        int discountPrice = 0;
                        discountPrice = ((engAndPrice * discountPercent) / 100);
                        InlineAgentSaleAMount ilineAgentSaleAMount = new InlineAgentSaleAMount()
                        {
                            AgentAmountRuleId = null,
                            Price = discountPrice
                        };
                        return ilineAgentSaleAMount;
                    }
                    else
                    {
                        var engAndPrice = getEngAndPostPrice(item);
                        if (engAndPrice == null)
                        {
                            string MsgNote = $"قیمت های ایتم ${item.Id} در این سفارش به درستی محاسته نشده و قانون حق نمایندگی برای آن محاسبه نشد" + "---" + item.Product.Name;
                            InsertOrderNote(MsgNote, item.OrderId);
                            return null;
                        }
                        int discountPrice = 0;
                        discountPrice = ((engAndPrice.IncomePrice * discountPercent) / 100);
                        InlineAgentSaleAMount ilineAgentSaleAMount = new InlineAgentSaleAMount()
                        {
                            AgentAmountRuleId = null,
                            Price = discountPrice
                        };
                        return ilineAgentSaleAMount;
                    }
                }
            }
            catch (Exception ex)
            {
                privatePostDiscount = null;
                InsertOrderNote($"خطا در زمان محاسبه تخفیف حق نمایندگی برای آیتم {item.Id} به وجود آمده" + "\r\n" +
                    ex.Message + (ex.InnerException != null ? "--> " + ex.InnerException.Message : ""), item.OrderId);
                return null;
            }
        }

        public InlineAgentSaleAMount getInlineDsicountByCustomer(OrderItem item, int CustomerId, out PrivatePostDiscount privatePostDiscount)
        {
            try
            {
                privatePostDiscount = null;
                Customer _discountCustomer = null;
                if (CustomerId > 0)
                    _discountCustomer = _customerService.GetCustomerById(CustomerId);
                if (_discountCustomer == null)
                {
                    return null;
                }
                int serviceId = item.Product.ProductCategories.First().CategoryId;
                if (new int[] { 654, 655, 667, 670, 662, 725, 726, 727 }.Contains(serviceId))
                {
                    var PAM = _productAttributeParser.ParseProductAttributeMappings(item.AttributesXml).Select(p => p.ProductAttribute.Id).ToList();
                    string PaIds = string.Join(",", PAM);
                    var PAVs = _productAttributeParser.ParseProductAttributeValues(item.AttributesXml).ToList();
                    string PavIds = string.Join(",", PAVs.Select(p => p.Id).ToList());
                    if (string.IsNullOrEmpty(PaIds) || string.IsNullOrEmpty(PavIds))
                    {
                        string MsgNote = $"اطلاعات ویژگی و مقادیر ویژگی برای این آیتم نا معتبر می باشد" + "---" + item.Product.Name;
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    string query = @"SELECT
	                            TAAR.Price,
                                TCAAR.AgentAmountRuleId
                            FROM
	                            dbo.Tb_CustomerAgentAssignmentRule AS TCAAR
	                            INNER JOIN dbo.Tb_AgentAmountRule AS TAAR ON TAAR.Id = TCAAR.AgentAmountRuleId
                            WHERE
	                            TAAR.DeletedDate IS NULL
	                            AND TCAAR.IsActive = 1
	                            AND TAAR.ProductId = " + item.ProductId + @"
	                            AND TCAAR.CustomerId = " + CustomerId + @"
	                            AND TAAR.ProductAttributeId IN (" + PaIds + @")
	                            AND TAAR.ProductAttributeValueId IN (" + PavIds + ")";

                    var LstPrice = _dbContext.SqlQuery<InlineAgentSaleAMount>(query, new object[0]).ToList();
                    if (LstPrice.Count() > 1)
                    {
                        string MsgNote = $"بیش از یک قانون حق نمایندگی در یک وزن برای نماینده این سفارش  تعریف شده است." + "---" + item.Product.Name;
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    var data = LstPrice.First();
                    data.Price = data.Price * item.Quantity;
                    return data;
                }
                else //پست خصوصی
                {
                    PrivatePostDiscount discount = GetPrivatePostDiscount(CustomerId, serviceId);
                    privatePostDiscount = discount;
                    if (discount == null)
                    {
                        string MsgNote = $"در این سرویس برای مشتری تخفیفی تعریف نشده";
                        InsertOrderNote(MsgNote, item.OrderId);
                        return null;
                    }
                    if (new int[] { 722, 723, 667, 670 }.Contains(serviceId))
                    {
                        var engAndPrice = getGatwayEngPrice(item);
                        if (engAndPrice == 0)
                        {
                            string MsgNote = $"قیمت های ایتم ${item.Id} در این سفارش به درستی محاسته نشده و قانون حق نمایندگی برای آن محاسبه نشد" + "---" + item.Product.Name;
                            InsertOrderNote(MsgNote, item.OrderId);
                            return null;
                        }
                        int discountPrice = 0;
                        if (discount.IsPercent)
                        {
                            discountPrice = ((engAndPrice * discount.DisCountAmount) / 100) * item.Quantity;
                        }
                        else
                        {
                            discountPrice = discount.DisCountAmount * item.Quantity;
                        }
                        //if (discountPrice > 0)
                        //    discountPrice -= (discountPrice * 9 / 100);
                        InlineAgentSaleAMount ilineAgentSaleAMount = new InlineAgentSaleAMount()
                        {
                            AgentAmountRuleId = null,
                            Price = discountPrice
                        };
                        return ilineAgentSaleAMount;
                    }
                    else
                    {
                        var engAndPrice = getEngAndPostPrice(item);
                        if (engAndPrice == null)
                        {
                            string MsgNote = $"قیمت های ایتم ${item.Id} در این سفارش به درستی محاسته نشده و قانون حق نمایندگی برای آن محاسبه نشد" + "---" + item.Product.Name;
                            InsertOrderNote(MsgNote, item.OrderId);
                            return null;
                        }
                        int discountPrice = 0;
                        if (discount.IsPercent)
                        {
                            discountPrice = ((engAndPrice.IncomePrice * discount.DisCountAmount) / 100) * item.Quantity;
                        }
                        else
                        {
                            discountPrice = discount.DisCountAmount * item.Quantity;
                        }
                        InlineAgentSaleAMount ilineAgentSaleAMount = new InlineAgentSaleAMount()
                        {
                            AgentAmountRuleId = null,
                            Price = discountPrice
                        };
                        return ilineAgentSaleAMount;
                    }
                }
            }
            catch (Exception ex)
            {
                privatePostDiscount = null;
                InsertOrderNote($"خطا در زمان محاسبه تخفیف حق نمایندگی برای آیتم {item.Id} به وجود آمده" + "\r\n" +
                    ex.Message + (ex.InnerException != null ? "--> " + ex.InnerException.Message : ""), item.OrderId);
                return null;
            }
        }


        private int getGatwayEngPrice(OrderItem item)
        {
            string query = $@"SELECT
		                        sa.CodCost CodCost
	                        FROM
		                        dbo.ShipmentAppointment AS SA
		                        INNER JOIN dbo.Shipment AS S ON S.Id = SA.ShipmentId
		                        INNER JOIN dbo.ShipmentItem AS SI ON SI.ShipmentId = S.Id
		                        INNER JOIN dbo.OrderItem AS OI ON SI.OrderItemId = OI.Id
	                        WHERE
		                        OI.Id = {item.Id}";
            var CodCost = _dbContext.SqlQuery<int?>(query, new object[0]).FirstOrDefault().GetValueOrDefault(0);
            int basePrice = (CodCost - 8000);
            int _bajePrice = ((basePrice * 100) / (100 - 30)) + 8000;
            _bajePrice += (_bajePrice * 9) / 100;
            return _bajePrice;
        }
        private int ChargeWalletForAgentSaleAmount(Order order, int price, string inputMsg, out string Msg)
        {
            string Message = (string.IsNullOrEmpty(inputMsg) ? ("واریز مبلغ خدمات نمایندگی برای سفارش شماره" + " " + order.Id + " با کسر مالیات") : inputMsg);
            //price = price; - ((price * 9) / 100);
            //TODO : Reward point
            int rewardPointHistoryId = _rewardPointService.AddRewardPointsHistoryEntry(order.Customer, price,
                order.StoreId, Message, usedAmount: 0);
            if (rewardPointHistoryId > 0)
            {
                Msg = "واریز به کیف پول با موفقیت انجام شد";
                return rewardPointHistoryId;
            }
            Msg = "اشکال در واریز به کیف پول.لطفا با مدیر فنی تماس بگیرید";
            return 0;
        }

        public void InsertOrderNote(string note, int orderId)
        {
            OrderNote Onote = new OrderNote()
            {
                Note = note + "-" + _workContext.CurrentCustomer.Id.ToString(),
                CreatedOnUtc = DateTime.Now.ToUniversalTime(),
                DisplayToCustomer = false,
                OrderId = orderId
            };
            _orderNoteRepository.Insert(Onote);
        }
        public ApiOrderItemPrice getEngAndPostPrice(OrderItem orderitem)
        {
            string query = $@"SELECT TOP(1)
	                         TCPOI.IncomePrice
	                        , TCPOI.EngPrice
                            , TCPOI.AttrPrice
                        FROM
	                        dbo.Tb_CalcPriceOrderItem AS TCPOI
                        WHERE
	                        TCPOI.OrderItemId = {orderitem.Id}";
            return _dbContext.SqlQuery<ApiOrderItemPrice>(query, new object[0]).FirstOrDefault();
        }

        public class PrivatePostDiscount
        {
            public int DisCountAmount { get; set; }
            public bool IsPercent { get; set; }
        }
    }
}
