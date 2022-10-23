using System;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Controllers;

namespace NopMaster.Plugin.Payments.Wallet.Controllers
{
    public class WalletAdminController : BaseAdminController
    {
        private readonly IStoreService istoreService_0;

        private readonly IWorkContext _workContext;

        private CurrencySettings currencySettings_0;

        private readonly ICurrencyService icurrencyService_0;

        private readonly IDateTimeHelper idateTimeHelper_0;

        private readonly ILocalizationService ilocalizationService_0;

        private readonly IOrderReportService iorderReportService_0;

        private readonly IOrderService iorderService_0;

        private readonly IPriceFormatter ipriceFormatter_0;

        private readonly IProductService iproductService_0;

        private readonly ISettingService isettingService_0;

        public WalletAdminController(IStoreService storeService, IWorkContext workContext,
            ISettingService settingService, IPermissionService permissionService, IDateTimeHelper _dateTimeHelper,
            IProductService _productService, IOrderService _orderService, IPriceFormatter _priceFormatter,
            ILocalizationService _localizationService, IOrderReportService _orderReportService,
            CurrencySettings _currencySettings, ICurrencyService _currencyService)
        {
            istoreService_0 = storeService;
            _workContext = workContext;
            isettingService_0 = settingService;
            _permissionService = permissionService;
            idateTimeHelper_0 = _dateTimeHelper;
            iorderService_0 = _orderService;
            iproductService_0 = _productService;
            ipriceFormatter_0 = _priceFormatter;
            ilocalizationService_0 = _localizationService;
            iorderReportService_0 = _orderReportService;
            currencySettings_0 = _currencySettings;
            icurrencyService_0 = _currencyService;
        }

        public IPermissionService _permissionService { get; set; }

        [HttpGet]
        public IActionResult Configure()
        {
            var activeStoreScopeConfiguration = GetActiveStoreScopeConfiguration(istoreService_0, _workContext);
            var setting =
                isettingService_0.GetSetting("NopMaster.Wallet_ProductId", activeStoreScopeConfiguration, false);
            int productId = setting == null ? 0 : int.Parse(setting.Value);
            ViewBag.productId = productId;
            return base.View("~/Plugins/Payments.Wallet/Views/Configure.cshtml", productId);
        }

        [HttpPost]
        public IActionResult Configure(int productId)
        {
            var activeStoreScopeConfiguration = GetActiveStoreScopeConfiguration(istoreService_0, _workContext);
            ;
            isettingService_0.SetSetting("NopMaster.Wallet_ProductId", productId, activeStoreScopeConfiguration, true);
            SuccessNotification("با موفقیت ذخیره شد. تولید شده توسط ناپ مستر . ", true);
            ViewBag.productId = productId;
            return base.View("~/Plugins/Payments.Wallet/Views/Configure.cshtml", productId);
        }

        protected bool HasAccessToProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_workContext.CurrentVendor == null)
                //not a vendor; has access
                return true;

            var vendorId = _workContext.CurrentVendor.Id;
            return product.VendorId == vendorId;
        }

        //      [HttpPost]
        //public IActionResult OrderList(DataSourceRequest command, OrderListModel model)
        //{
        //	// 
        //	// Current member / type: Microsoft.AspNetCore.Mvc.IActionResult NopMaster.Plugin.Payments.Wallet.Controllers.WalletAdminController::OrderList(Nop.Web.Framework.Kendoui.DataSourceRequest,Nop.Web.Areas.Admin.Models.Orders.OrderListModel)
        //	// File path: D:\Projects\Web_APP\nope plugin\4.0\monybag\NopMaster.Plugin.Payments.Wallet-cleaned.dll
        //	// 
        //	// Product version: 2019.1.118.0
        //	// Exception in: Microsoft.AspNetCore.Mvc.IActionResult OrderList(Nop.Web.Framework.Kendoui.DataSourceRequest,Nop.Web.Areas.Admin.Models.Orders.OrderListModel)
        //	// 
        //	// An item with the same key has already been added.
        //	//    at System.ThrowHelper.ThrowArgumentException(ExceptionResource resource)
        //	//    at System.Collections.Generic.Dictionary`2.Insert(TKey key, TValue value, Boolean add)
        //	//    at System.Collections.Generic.Dictionary`2.System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey,TValue>>.Add(KeyValuePair`2 keyValuePair)
        //	//    at Telerik.JustDecompiler.Common.Extensions.AddRange[,](IDictionary`2 , IDictionary`2 ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Common\Extensions.cs:line 99
        //	//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:line 66
        //	//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 89
        //	//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
        //	//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
        //	//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 322
        //	//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 616
        //	//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:line 101
        //	//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 127
        //	//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
        //	//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
        //	//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 322
        //	//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:line 124
        //	//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 87
        //	//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
        //	//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
        //	//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 322
        //	//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:line 124
        //	//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 87
        //	//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
        //	//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 383
        //	//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 59
        //	//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
        //	//    at ..Visit[,]( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 286
        //	//    at ..Visit( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 317
        //	//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 337
        //	//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLambdaExpressions.cs:line 130
        //	//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
        //	//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
        //	//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
        //	//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
        //	//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
        //	// 
        //	// mailto: JustDecompilePublicFeedback@telerik.com

        //}
    }
}