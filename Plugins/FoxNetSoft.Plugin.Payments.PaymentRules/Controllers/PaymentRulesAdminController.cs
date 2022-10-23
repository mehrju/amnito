using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using FoxNetSoft.Plugin.Payments.PaymentRules.Domain;
using FoxNetSoft.Plugin.Payments.PaymentRules.Logger;
using FoxNetSoft.Plugin.Payments.PaymentRules.Mapper;
using FoxNetSoft.Plugin.Payments.PaymentRules.Models;
using FoxNetSoft.Plugin.Payments.PaymentRules.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Shipping;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;

namespace FoxNetSoft.Plugin.Payments.PaymentRules.Controllers
{

    public class PaymentRulesAdminController : BaseAdminController
    {
        #region field

        private readonly PaymentRulesSettings _paymentRulesSettings;

        private readonly IPaymentRulesService _paymentRulesService;

        private readonly ILocalizationService _localizationService;

        private readonly IWorkContext _workContext;

        private readonly ICustomerService _customerService;

        private readonly ICountryService _countryService;

        private readonly IStateProvinceService _stateProvinceService;

        private readonly ICustomerActivityService _customerActivityService;

        private readonly IPermissionService _permissionService;

        private readonly IAclService _aclService;

        private readonly IStoreService _storeService;

        private readonly IStoreMappingService _storeMappingService;

        private readonly ISpecificationAttributeService _specificationAttributeService;

        private readonly ICheckoutAttributeService _checkoutAttributeService;

        private readonly ICustomerAttributeService _customerAttributeService;

        private readonly ICategoryService _categoryService;

        private readonly IManufacturerService _manufacturerService;

        private readonly IVendorService _vendorService;

        private readonly IShippingService _shippingService;

        private readonly PaymentSettings _paymentSettings;

        private readonly IWebHelper _webHelper;

        private readonly FNSLogger _fnsLogger;

        private readonly bool showDebugInfo;

        #endregion

        #region ctor

        public PaymentRulesAdminController(PaymentRulesSettings paymentRulesSettings,
            IPaymentRulesService paymentRulesService
            , ILocalizationService localizationService
            , IWorkContext workContext
            , ICustomerService customerService
            , ICountryService countryService
            , IStateProvinceService stateProvinceService
            , ICustomerActivityService customerActivityService
            , IPermissionService permissionService
            , IAclService aclService
            , IStoreService storeService
            , IStoreMappingService storeMappingService
            , ISpecificationAttributeService specificationAttributeService
            , ICheckoutAttributeService checkoutAttributeService
            , ICustomerAttributeService customerAttributeService
            , ICategoryService categoryService
            , IManufacturerService manufacturerService
            , IVendorService vendorService
            , IShippingService shippingService
            , PaymentSettings paymentSettings
            , IWebHelper webHelper
            )
        {
            _paymentRulesSettings = paymentRulesSettings;
            _paymentRulesService = paymentRulesService;
            _localizationService = localizationService;
            _workContext = workContext;
            _customerService = customerService;
            _countryService = countryService;
            _stateProvinceService = stateProvinceService;
            _customerActivityService = customerActivityService;
            _aclService = aclService;
            _permissionService = permissionService;
            _storeService = storeService;
            _storeMappingService = storeMappingService;
            _specificationAttributeService = specificationAttributeService;
            _checkoutAttributeService = checkoutAttributeService;
            _customerAttributeService = customerAttributeService;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _vendorService = vendorService;
            _shippingService = shippingService;
            _paymentSettings = paymentSettings;
            _webHelper = webHelper;
            showDebugInfo = _paymentRulesSettings.showDebugInfo;
            _fnsLogger = new FNSLogger(showDebugInfo);
        }

        #endregion

        public virtual IActionResult AddNewGroup(int externalId, string name)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(externalId);
            if (paymentRuleById == null) throw new ArgumentException("Payment rule could not be loaded");
            if (paymentRuleById.Requirements.FirstOrDefault(requirement =>
            {
                if (requirement.ParentId.HasValue) return false;
                return requirement.IsGroup;
            }) == null)
                paymentRuleById.Requirements.Add(new PaymentRuleRequirement
                {
                    IsGroup = true,
                    InteractionType = GroupInteractionType.And,
                    RequirementCategory = RequirementCategory.DefaultRequirementGroup.ToString()
                });
            var paymentRuleRequirement = new PaymentRuleRequirement
            {
                IsGroup = true,
                RequirementCategory = name,
                InteractionType = GroupInteractionType.And
            };
            paymentRuleById.Requirements.Add(paymentRuleRequirement);
            _paymentRulesService.UpdatePaymentRule(paymentRuleById);
            if (!string.IsNullOrEmpty(name))
                return Json(new {Result = true, NewRequirementId = paymentRuleRequirement.Id});
            paymentRuleRequirement.RequirementCategory = string.Format("#{0}", paymentRuleRequirement.Id);
            _paymentRulesService.UpdatePaymentRule(paymentRuleById);
            return Json(new {Result = true, NewRequirementId = paymentRuleRequirement.Id});
        }

        public IActionResult ConfigureOneRequirement(string systemName, int externalId, int? requirementId)
        {
            RequirementCategory requirementCategory;
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return Content("Access denied");
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(externalId);
            if (paymentRuleById == null) throw new ArgumentException("PaymentRule could not be loaded");
            if (!Enum.TryParse(systemName, out requirementCategory))
                throw new ArgumentException("Requirement category was wrong");
            PaymentRuleRequirement paymentRuleRequirement = null;
            if (requirementId.HasValue)
            {
                paymentRuleRequirement =
                    paymentRuleById.Requirements.FirstOrDefault(dr => dr.Id == requirementId.Value);
                if (paymentRuleRequirement == null) return Content("Failed to load requirement.");
            }

            var requirementModel = new RequirementModel
            {
                RequirementId = requirementId.HasValue ? requirementId.Value : 0,
                ExternalId = externalId,
                RequirementCategory = systemName
            };
            method_7(requirementModel, paymentRuleRequirement);
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format("PaymentRules{0}",
                requirementId.HasValue ? requirementId.Value.ToString() : "0");
            return View(method_11("RequirementBox"), requirementModel);
        }

        [HttpPost]
        public IActionResult ConfigureOneRequirement(RequirementModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedKendoGridJson();
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(model.ExternalId);
            if (paymentRuleById == null) throw new ArgumentException("PaymentRule could not be loaded");
            PaymentRuleRequirement paymentRuleRequirement = null;
            if (model.RequirementId > 0)
                paymentRuleRequirement =
                    paymentRuleById.Requirements.FirstOrDefault(dr => dr.Id == model.RequirementId);
            if (paymentRuleRequirement == null)
            {
                paymentRuleRequirement = new PaymentRuleRequirement
                {
                    RequirementCategory = model.RequirementCategory,
                    RequirementProperty = model.RequirementProperty,
                    RequirementOperator = model.RequirementOperator,
                    RequirementValue = model.RequirementValue
                };
                paymentRuleById.Requirements.Add(paymentRuleRequirement);
            }
            else
            {
                paymentRuleRequirement.RequirementProperty = model.RequirementProperty;
                paymentRuleRequirement.RequirementOperator = model.RequirementOperator;
                paymentRuleRequirement.RequirementValue = model.RequirementValue;
            }

            _paymentRulesService.UpdatePaymentRule(paymentRuleById);
            return Json(new {Result = true, NewRequirementId = paymentRuleRequirement.Id});
        }

        public IActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var paymentRuleModel = new PaymentRuleModel();
            method_2(null, paymentRuleModel, false);
            method_1();
            return View(method_11("Create"), paymentRuleModel);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Create(PaymentRuleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            if (ModelState.IsValid)
            {
                //var num = 0;
                //if (!testDataPlugin_0.IsRegisted) num = _paymentRulesService.GetAllPaymentRules(true).Count();
                //if (testDataPlugin_0.IsRegisted || num < 5)
                //{
                    var entity = model.ToEntity();
                    if (model.CheckedPaymentMethods == null)
                        entity.Payments = "";
                    else
                        entity.Payments = string.Join(",", model.CheckedPaymentMethods);
                    _paymentRulesService.InsertPaymentRule(entity);
                    method_6(entity, model);
                    method_4(entity, model);
                    _paymentRulesService.UpdatePaymentRule(entity);
                    _customerActivityService.InsertActivity("FoxNetSoft.PaymentRules",
                        _localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.ActivityLog.AddPaymentRule"),
                        entity.Id);
                    SuccessNotification(_localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.Added"),
                        true);
                    if (!continueEditing) return RedirectToAction("List");
                    return RedirectToAction("Edit", new {id = entity.Id});
                //}

                //ErrorNotification(_localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.IsNoRegisted"),
                //    true);
            }

            method_2(null, model, false);
            return View(method_11("Create"), model);
        }

        [ActionName("Delete")]
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(id);
            if (paymentRuleById == null) return RedirectToAction("List");
            _paymentRulesService.DeletePaymentRule(paymentRuleById);
            _customerActivityService.InsertActivity("FoxNetSoft.PaymentRules",
                _localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.ActivityLog.DeletePaymentRule"),
                paymentRuleById.Id);
            SuccessNotification(_localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.Deleted"), true);
            return RedirectToAction("List");
        }

        public IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(id);
            if (paymentRuleById == null) return RedirectToAction("List");
            var model = paymentRuleById.ToModel();
            method_2(paymentRuleById, model, false);
            method_1();
            return View(method_11("Edit"), model);
        }

        [HttpPost]
        [ParameterBasedOnFormName("save-continue", "continueEditing")]
        public IActionResult Edit(PaymentRuleModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(model.Id);
            if (paymentRuleById == null) return RedirectToAction("List");
            if (!ModelState.IsValid)
            {
                method_2(paymentRuleById, model, false);
                return View(method_11("Edit"), model);
            }

            paymentRuleById = model.ToEntity(paymentRuleById);
            if (model.CheckedPaymentMethods == null)
                paymentRuleById.Payments = "";
            else
                paymentRuleById.Payments = string.Join(",", model.CheckedPaymentMethods);
            _paymentRulesService.UpdatePaymentRule(paymentRuleById);
            method_6(paymentRuleById, model);
            method_4(paymentRuleById, model);
            _paymentRulesService.UpdatePaymentRule(paymentRuleById);
            _customerActivityService.InsertActivity("FoxNetSoft.PaymentRules",
                _localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.ActivityLog.EditPaymentRule"),
                paymentRuleById.Id);
            SuccessNotification(_localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.Updated"), true);
            if (!continueEditing) return RedirectToAction("List");
            return RedirectToAction("Edit", paymentRuleById.Id);
        }

        public virtual IActionResult GetRequirementConfigurationUrl(string systemName, int externalId,
            int? requirementId)
        {
            RequirementCategory requirementCategory;
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            if (string.IsNullOrEmpty(systemName)) throw new ArgumentNullException("systemName");
            if (!Enum.TryParse(systemName, out requirementCategory))
                throw new ArgumentException("Payment requirement rule could not be loaded");
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(externalId);
            if (paymentRuleById == null) throw new ArgumentException("Payment rule could not be loaded");
            var str = method_8(systemName, paymentRuleById, requirementId);
            return Json(new {url = str});
        }

        public virtual IActionResult GetRequirements(int externalId, int requirementId, int? parentId,
            int? interactionTypeId, bool deleteRequirement)
        {
            GroupInteractionType? interactionType;
            int id;
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var requirementMetaInfos = new List<PaymentRuleModel.RequirementMetaInfo>();
            var paymentRuleById = _paymentRulesService.GetPaymentRuleById(externalId);
            if (paymentRuleById == null) return Json(requirementMetaInfos);
            var nullable = paymentRuleById.Requirements.FirstOrDefault(requirement => requirement.Id == requirementId);
            if (nullable != null)
            {
                if (!deleteRequirement)
                {
                    var paymentRuleRequirement = paymentRuleById.Requirements.FirstOrDefault(requirement =>
                    {
                        if (requirement.ParentId.HasValue) return false;
                        return requirement.IsGroup;
                    });
                    if (paymentRuleRequirement != null)
                        id = paymentRuleRequirement.Id;
                    else
                        id = 0;
                    var num = id;
                    if (num == 0)
                    {
                        var paymentRuleRequirement1 = new PaymentRuleRequirement
                        {
                            IsGroup = true,
                            InteractionType = GroupInteractionType.And,
                            RequirementCategory = RequirementCategory.DefaultRequirementGroup.ToString()
                        };
                        paymentRuleById.Requirements.Add(paymentRuleRequirement1);
                        _paymentRulesService.UpdatePaymentRule(paymentRuleById);
                        num = paymentRuleRequirement1.Id;
                    }

                    if (parentId.HasValue)
                        nullable.ParentId = parentId.Value;
                    else if (num != nullable.Id) nullable.ParentId = num;
                    if (interactionTypeId.HasValue) nullable.InteractionTypeId = interactionTypeId;
                    _paymentRulesService.UpdatePaymentRule(paymentRuleById);
                }
                else
                {
                    method_10(new List<PaymentRuleRequirement>
                    {
                        nullable
                    });
                    if (!paymentRuleById.Requirements.Any(requirement => requirement.ParentId.HasValue))
                        method_10(paymentRuleById.Requirements);
                }
            }

            var list = paymentRuleById.Requirements.Where(requirement =>
            {
                if (requirement.ParentId.HasValue) return false;
                return requirement.IsGroup;
            }).ToList();
            var paymentRuleRequirement2 = list.FirstOrDefault();
            if (paymentRuleRequirement2 != null)
                interactionType = paymentRuleRequirement2.InteractionType;
            else
                interactionType = null;
            var nullable1 = interactionType;
            if (nullable1.HasValue) requirementMetaInfos = method_9(list, nullable1.Value, paymentRuleById).ToList();
            var selectListItems = (
                from requirement in paymentRuleById.Requirements
                where requirement.IsGroup
                select requirement).Select(requirement =>
            {
                var selectListItem = new SelectListItem();
                selectListItem.Value = requirement.Id.ToString();
                selectListItem.Text = requirement.RequirementCategory;
                return selectListItem;
            }).ToList();
            return Json(new {Requirements = requirementMetaInfos, AvailableGroups = selectListItems});
        }

        [HttpPost]
        public IActionResult GetRequirementValues(int requirementId = 0, string requirementProperty = "",
            string requirementCategory = "")
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedKendoGridJson();
            var requirementModel = new RequirementModel
            {
                RequirementId = requirementId,
                RequirementCategory = requirementCategory,
                RequirementProperty = requirementProperty
            };
            method_7(requirementModel, null);
            ViewData.TemplateInfo.HtmlFieldPrefix = "PaymentRules0";
            var str = RenderPartialViewToString(method_11("_RequirementBox.Values"), requirementModel);
            IList<JueryAnswer> jueryAnswers = new List<JueryAnswer>();
            foreach (var availableOperator in requirementModel.AvailableOperators)
                jueryAnswers.Add(new JueryAnswer
                {
                    id = availableOperator.Value,
                    name = availableOperator.Text
                });
            return Json(new {requirementoperators = jueryAnswers, updaterequirementvalueshtml = str});
        }

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();
            var paymentRuleListModel = new PaymentRuleListModel();
            method_1();
            return View(method_11("List"), paymentRuleListModel);
        }

        [HttpPost]
        public IActionResult List(DataSourceRequest command, PaymentRuleListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedKendoGridJson();
            var allPaymentRules = _paymentRulesService.GetAllPaymentRules(true);
            var dataSourceResult = new DataSourceResult();
            dataSourceResult.Data = allPaymentRules.Select(paymentRule_0 =>
            {
                var paymentRuleModel = paymentRule_0.ToModel();
                method_2(paymentRule_0, paymentRuleModel, true);
                return paymentRuleModel;
            });
            return Json(dataSourceResult);
        }

        private void method_0(string string_0)
        {
            if (showDebugInfo) _fnsLogger.LogMessage(string_0);
        }

        private void method_1()
        {
            //if (testDataPlugin_0.IsExpired)
            //{
            //    ErrorNotification(_localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.IsExpired"), true);
            //    return;
            //}

            //if (!testDataPlugin_0.IsRegisted)
            //    ErrorNotification(_localizationService.GetResource("Admin.FoxNetSoft.PaymentRules.IsNoRegisted"),
            //        true);
        }

        private void method_10(ICollection<PaymentRuleRequirement> icollection_0)
        {
            var list = icollection_0.ToList();
            for (var i = 0; i < list.Count; i++)
            {
                if (list[i].ChildRequirements.Any()) method_10(list[i].ChildRequirements);
                _paymentRulesService.DeletePaymentRuleRequirement(list[i]);
            }
        }

        private string method_11(string string_0)
        {
            return string.Concat(PluginLog.Folder, "Views/PaymentRulesAdmin/", string_0, ".cshtml");
        }

        private void method_2(PaymentRule paymentRule_0, PaymentRuleModel paymentRuleModel_0, bool bool_1)
        {
            DateTime? startDateTimeUtc;
            if (paymentRuleModel_0 == null) throw new ArgumentNullException("model");
            if (bool_1)
            {
                paymentRuleModel_0.RequirementsHTML = "";
                paymentRuleModel_0.PaymentMethodsHTML = "";
                if (paymentRule_0 != null && !string.IsNullOrWhiteSpace(paymentRule_0.Payments))
                {
                    var stringBuilder = new StringBuilder();
                    stringBuilder.Append("<div class=\"payment-rule-list\">");
                    var strArrays = paymentRule_0.Payments.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < strArrays.Length; i++)
                    {
                        var str = strArrays[i];
                        stringBuilder.Append(string.Concat(str, "<br />"));
                    }

                    stringBuilder.Append("</div>");
                    paymentRuleModel_0.PaymentMethodsHTML = stringBuilder.ToString();
                }

                if (paymentRule_0 != null)
                {
                    var stringBuilder1 = new StringBuilder();
                    stringBuilder1.Append("<div class=\"requirements-list\">");
                    var resource = "";
                    if (paymentRule_0.LimitedToStores)
                    {
                        var storeMappings = _storeMappingService.GetStoreMappings(paymentRule_0);
                        if (storeMappings.Count > 0)
                            for (var j = 0; j < storeMappings.Count; j++)
                            {
                                resource = string.Concat(resource, storeMappings[j].Store.Name);
                                if (j != storeMappings.Count - 1) resource = string.Concat(resource, ",");
                            }
                    }

                    if (string.IsNullOrWhiteSpace(resource))
                        resource = _localizationService.GetResource("Admin.Common.All");
                    stringBuilder1.Append(string.Concat(
                        _localizationService.GetResource(
                            "Admin.FoxNetSoft.PaymentRules.PaymentRule.List.RequirementsInfo.Store"), " <i>", resource,
                        "</i><br />"));
                    var resource1 = "";
                    var aclRecords = _aclService.GetAclRecords(paymentRule_0);
                    if (aclRecords.Count() > 0)
                        for (var k = 0; k < aclRecords.Count; k++)
                        {
                            resource1 = string.Concat(resource1, aclRecords[k].CustomerRole.Name);
                            if (k != aclRecords.Count - 1) resource1 = string.Concat(resource1, ",");
                        }

                    if (string.IsNullOrWhiteSpace(resource1))
                        resource1 = _localizationService.GetResource("Admin.Common.All");
                    stringBuilder1.Append(string.Concat(
                        _localizationService.GetResource(
                            "Admin.FoxNetSoft.PaymentRules.PaymentRule.List.RequirementsInfo.CustomerRole"), " <i>",
                        resource1, "</i><br />"));
                    if (paymentRule_0.StartDateTimeUtc.HasValue)
                    {
                        var str1 = _localizationService.GetResource(
                            "Admin.FoxNetSoft.PaymentRules.PaymentRule.List.RequirementsInfo.StartDateTimeUtc");
                        startDateTimeUtc = paymentRule_0.StartDateTimeUtc;
                        stringBuilder1.Append(string.Concat(str1, " <i>", startDateTimeUtc.ToString(), "</i><br />"));
                    }

                    if (paymentRule_0.EndDateTimeUtc.HasValue)
                    {
                        var resource2 = _localizationService.GetResource(
                            "Admin.FoxNetSoft.PaymentRules.PaymentRule.List.RequirementsInfo.EndDateTimeUtc");
                        startDateTimeUtc = paymentRule_0.EndDateTimeUtc;
                        stringBuilder1.Append(string.Concat(resource2, " <i>", startDateTimeUtc.ToString(),
                            "</i><br />"));
                    }

                    if (paymentRule_0.Requirements.Any())
                        stringBuilder1.Append(string.Concat(
                            _localizationService.GetResource(
                                "Admin.FoxNetSoft.PaymentRules.PaymentRule.List.RequirementsInfo.Requirements"),
                            "<br />"));
                    stringBuilder1.Append("</div>");
                    paymentRuleModel_0.RequirementsHTML = stringBuilder1.ToString();
                }
            }

            if (!bool_1)
            {
                var availableRequirements = paymentRuleModel_0.AvailableRequirements;
                var selectListItem1 = new SelectListItem();
                selectListItem1.Text =
                    _localizationService.GetResource(
                        "Admin.FoxNetSoft.PaymentRules.Requirements.PaymentRequirementType.Select");
                selectListItem1.Value = "";
                availableRequirements.Add(selectListItem1);
                var allCheckoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(0, false);
                var allCustomerAttributes = _customerAttributeService.GetAllCustomerAttributes();
                var specificationAttributes =
                    _specificationAttributeService.GetSpecificationAttributes(0, 2147483647);
                foreach (RequirementCategory value in Enum.GetValues(typeof(RequirementCategory)))
                {
                    if (value == RequirementCategory.DefaultRequirementGroup ||
                        value == RequirementCategory.CheckoutAttributes && !allCheckoutAttributes.Any() ||
                        value == RequirementCategory.CustomerAttributes && !allCustomerAttributes.Any() ||
                        value == RequirementCategory.SpecificationAttributes && !specificationAttributes.Any())
                        continue;
                    var selectListItems = paymentRuleModel_0.AvailableRequirements;
                    var selectListItem2 = new SelectListItem();
                    selectListItem2.Text = value.GetLocalizedEnum(_localizationService, _workContext);
                    selectListItem2.Value = value.ToString();
                    selectListItems.Add(selectListItem2);
                }

                if (paymentRule_0 != null)
                {
                    var requirements =
                        from requirement in paymentRule_0.Requirements
                        where requirement.IsGroup
                        select requirement;
                    paymentRuleModel_0.AvailableRequirementGroups = requirements.Select(requirement =>
                    {
                        var selectListItem = new SelectListItem();
                        selectListItem.Value = requirement.Id.ToString();
                        selectListItem.Text = requirement.RequirementCategory;
                        return selectListItem;
                    }).ToList();
                }

                method_5(paymentRuleModel_0, paymentRule_0, false);
                method_3(paymentRuleModel_0, paymentRule_0, false);
                var strs = new List<string>();
                if (paymentRule_0 != null && !string.IsNullOrWhiteSpace(paymentRule_0.Payments))
                    strs = paymentRule_0.Payments.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var activePaymentMethodSystemName in _paymentSettings.ActivePaymentMethodSystemNames)
                {
                    var availablePaymentMethods = paymentRuleModel_0.AvailablePaymentMethods;
                    var selectListItem3 = new SelectListItem();
                    selectListItem3.Value = activePaymentMethodSystemName;
                    selectListItem3.Text = activePaymentMethodSystemName;
                    selectListItem3.Selected = strs.Any(p =>
                        p.Equals(activePaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    availablePaymentMethods.Add(selectListItem3);
                }
            }
        }

        private void method_3(PaymentRuleModel paymentRuleModel_0, PaymentRule paymentRule_0, bool bool_1)
        {
            if (paymentRuleModel_0 == null) throw new ArgumentNullException("model");
            if (!bool_1 && paymentRule_0 != null)
                paymentRuleModel_0.SelectedCustomerRoleIds =
                    _aclService.GetCustomerRoleIdsWithAccess(paymentRule_0).ToList();
            foreach (var allCustomerRole in _customerService.GetAllCustomerRoles(true))
            {
                var availableCustomerRoles = paymentRuleModel_0.AvailableCustomerRoles;
                var selectListItem = new SelectListItem();
                selectListItem.Text = allCustomerRole.Name;
                selectListItem.Value = allCustomerRole.Id.ToString();
                selectListItem.Selected = paymentRuleModel_0.SelectedCustomerRoleIds.Contains(allCustomerRole.Id);
                availableCustomerRoles.Add(selectListItem);
            }
        }

        private void method_4(PaymentRule paymentRule_0, PaymentRuleModel paymentRuleModel_0)
        {
            paymentRule_0.SubjectToAcl = paymentRuleModel_0.SelectedCustomerRoleIds.Any();
            var aclRecords = _aclService.GetAclRecords(paymentRule_0);
            foreach (var allCustomerRole in _customerService.GetAllCustomerRoles(true))
                if (!paymentRuleModel_0.SelectedCustomerRoleIds.Contains(allCustomerRole.Id))
                {
                    var aclRecord = aclRecords.FirstOrDefault(acl => acl.CustomerRoleId == allCustomerRole.Id);
                    if (aclRecord == null) continue;
                    _aclService.DeleteAclRecord(aclRecord);
                }
                else
                {
                    if (aclRecords.Count(acl => acl.CustomerRoleId == allCustomerRole.Id) != 0) continue;
                    _aclService.InsertAclRecord(paymentRule_0, allCustomerRole.Id);
                }
        }

        private void method_5(PaymentRuleModel paymentRuleModel_0, PaymentRule paymentRule_0, bool bool_1)
        {
            if (paymentRuleModel_0 == null) throw new ArgumentNullException("model");
            if (!bool_1 && paymentRule_0 != null)
                paymentRuleModel_0.SelectedStoreIds =
                    _storeMappingService.GetStoresIdsWithAccess(paymentRule_0).ToList();
            foreach (var allStore in _storeService.GetAllStores(true))
            {
                var availableStores = paymentRuleModel_0.AvailableStores;
                var selectListItem = new SelectListItem();
                selectListItem.Text = allStore.Name;
                selectListItem.Value = allStore.Id.ToString();
                selectListItem.Selected = paymentRuleModel_0.SelectedStoreIds.Contains(allStore.Id);
                availableStores.Add(selectListItem);
            }
        }

        private void method_6(PaymentRule paymentRule_0, PaymentRuleModel paymentRuleModel_0)
        {
            paymentRule_0.LimitedToStores = paymentRuleModel_0.SelectedStoreIds.Any();
            var storeMappings = _storeMappingService.GetStoreMappings(paymentRule_0);
            foreach (var allStore in _storeService.GetAllStores(true))
                if (!paymentRuleModel_0.SelectedStoreIds.Contains(allStore.Id))
                {
                    var storeMapping = storeMappings.FirstOrDefault(sm => sm.StoreId == allStore.Id);
                    if (storeMapping == null) continue;
                    _storeMappingService.DeleteStoreMapping(storeMapping);
                }
                else
                {
                    if (storeMappings.Count(sm => sm.StoreId == allStore.Id) != 0) continue;
                    _storeMappingService.InsertStoreMapping(paymentRule_0, allStore.Id);
                }
        }

        private void method_7(RequirementModel requirementModel_0, PaymentRuleRequirement paymentRuleRequirement_0)
        {
            RequirementCategory requirementCategory;
            IEnumerator enumerator;
            int id;
            SelectListItem selectListItem;
            ParameterExpression parameterExpression;
            if (!Enum.TryParse(requirementModel_0.RequirementCategory, out requirementCategory))
                throw new ArgumentException("Requirement category was wrong");
            if (paymentRuleRequirement_0 != null)
            {
                requirementModel_0.RequirementProperty = paymentRuleRequirement_0.RequirementProperty;
                requirementModel_0.RequirementOperator = paymentRuleRequirement_0.RequirementOperator;
                requirementModel_0.RequirementValue = paymentRuleRequirement_0.RequirementValue;
            }

            var strs = new List<string>();
            switch (requirementCategory)
            {
                case RequirementCategory.BillingAddress:
                case RequirementCategory.ShippingAddress:
                {
                    foreach (ShippingAddressOperators value in Enum.GetValues(typeof(ShippingAddressOperators)))
                    {
                        var availableProperties = requirementModel_0.AvailableProperties;
                        var selectListItem1 = new SelectListItem();
                        selectListItem1.Text = value.GetLocalizedEnum(_localizationService, _workContext);
                        selectListItem1.Value = value.ToString();
                        selectListItem1.Selected = value.ToString() == requirementModel_0.RequirementProperty;
                        availableProperties.Add(selectListItem1);
                    }

                    var shippingAddressOperator = ShippingAddressOperators.CountryIsoCode;
                    Enum.TryParse(requirementModel_0.RequirementProperty, out shippingAddressOperator);
                    if (shippingAddressOperator == ShippingAddressOperators.Country)
                    {
                        requirementModel_0.IsValueComboBox = true;

                        foreach (var current in _countryService.GetAllCountries(0, false))
                        {
                            var availableValues = requirementModel_0.AvailableValues;
                            selectListItem = new SelectListItem();
                            parameterExpression = Expression.Parameter(typeof(Country), "x");
                            selectListItem.Text = current.GetLocalized(Expression.Lambda<Func<Country, string>>(
                                Expression.Property(parameterExpression,
                                    (MethodInfo) MethodBase.GetMethodFromHandle(typeof(Country).GetMethod("get_Name")
                                        .MethodHandle)), parameterExpression));
                            selectListItem.Value = current.Id.ToString();
                            availableValues.Add(selectListItem);
                        }
                    }
                    else if (shippingAddressOperator == ShippingAddressOperators.Province)
                    {
                        requirementModel_0.IsValueComboBox = true;
                        foreach (var stateProvince in _stateProvinceService.GetStateProvinces(false))
                        {
                            var selectListItems = requirementModel_0.AvailableValues;
                            selectListItem = new SelectListItem();
                            parameterExpression = Expression.Parameter(typeof(StateProvince), "x");
                            selectListItem.Text = stateProvince.GetLocalized(
                                Expression.Lambda<Func<StateProvince, string>>(
                                    Expression.Property(parameterExpression,
                                        (MethodInfo) MethodBase.GetMethodFromHandle(typeof(StateProvince)
                                            .GetMethod("get_Name").MethodHandle)), parameterExpression));
                            selectListItem.Value = stateProvince.Id.ToString();
                            selectListItems.Add(selectListItem);
                        }
                    }

                    break;
                }
                case RequirementCategory.CheckoutAttributes:
                {
                    foreach (var checkoutAttribute in _checkoutAttributeService.GetAllCheckoutAttributes(0, false))
                    {
                        var availableProperties1 = requirementModel_0.AvailableProperties;
                        selectListItem = new SelectListItem();
                        parameterExpression = Expression.Parameter(typeof(CheckoutAttribute), "x");
                        selectListItem.Text = checkoutAttribute.GetLocalized(
                            Expression.Lambda<Func<CheckoutAttribute, string>>(
                                Expression.Property(parameterExpression,
                                    (MethodInfo) MethodBase.GetMethodFromHandle(typeof(CheckoutAttribute)
                                        .GetMethod("get_Name").MethodHandle)), parameterExpression));
                        selectListItem.Value = checkoutAttribute.Id.ToString();
                        id = checkoutAttribute.Id;
                        selectListItem.Selected = id.ToString() == requirementModel_0.RequirementProperty;
                        availableProperties1.Add(selectListItem);
                    }

                    break;
                }
                case RequirementCategory.OrderTotals:
                {
                    enumerator = Enum.GetValues(typeof(OrderTotalOperators)).GetEnumerator();
                    try
                    {
                        while (enumerator.MoveNext())
                        {
                            var orderTotalOperator = (OrderTotalOperators) enumerator.Current;
                            var selectListItems1 = requirementModel_0.AvailableProperties;
                            var selectListItem2 = new SelectListItem();
                            selectListItem2.Text =
                                orderTotalOperator.GetLocalizedEnum(_localizationService, _workContext);
                            selectListItem2.Value = orderTotalOperator.ToString();
                            selectListItem2.Selected =
                                orderTotalOperator.ToString() == requirementModel_0.RequirementProperty;
                            selectListItems1.Add(selectListItem2);
                        }
                    }
                    finally
                    {
                        var disposable = enumerator as IDisposable;
                        if (disposable != null) disposable.Dispose();
                    }

                    break;
                }
                case RequirementCategory.SpecificationAttributes:
                {
                    foreach (var specificationAttribute in _specificationAttributeService.GetSpecificationAttributes(
                        0, 2147483647))
                    {
                        var availableProperties2 = requirementModel_0.AvailableProperties;
                        selectListItem = new SelectListItem();
                        parameterExpression = Expression.Parameter(typeof(SpecificationAttribute), "x");
                        selectListItem.Text = specificationAttribute.GetLocalized(
                            Expression.Lambda<Func<SpecificationAttribute, string>>(
                                Expression.Property(parameterExpression,
                                    (MethodInfo) MethodBase.GetMethodFromHandle(typeof(SpecificationAttribute)
                                        .GetMethod("get_Name").MethodHandle)), parameterExpression));
                        selectListItem.Value = specificationAttribute.Id.ToString();
                        id = specificationAttribute.Id;
                        selectListItem.Selected = id.ToString() == requirementModel_0.RequirementProperty;
                        availableProperties2.Add(selectListItem);
                    }

                    var num = 0;
                    int.TryParse(requirementModel_0.RequirementProperty, out num);
                    if (num == 0)
                    {
                        var selectListItem3 = requirementModel_0.AvailableProperties.FirstOrDefault();
                        if (selectListItem3 != null) int.TryParse(selectListItem3.Value, out num);
                    }

                    if (num <= 0) break;
                    requirementModel_0.IsValueComboBox = true;
                    using (var enumerator4 = _specificationAttributeService
                        .GetSpecificationAttributeOptionsBySpecificationAttribute(num).GetEnumerator())
                    {
                        while (enumerator4.MoveNext())
                        {
                            var specificationAttributeOption = enumerator4.Current;
                            var availableValues1 = requirementModel_0.AvailableValues;
                            selectListItem = new SelectListItem();
                            parameterExpression = Expression.Parameter(typeof(SpecificationAttributeOption), "x");
                            selectListItem.Text = specificationAttributeOption.GetLocalized(
                                Expression.Lambda<Func<SpecificationAttributeOption, string>>(
                                    Expression.Property(parameterExpression,
                                        (MethodInfo) MethodBase.GetMethodFromHandle(typeof(SpecificationAttributeOption)
                                            .GetMethod("get_Name").MethodHandle)), parameterExpression));
                            selectListItem.Value = specificationAttributeOption.Id.ToString();
                            availableValues1.Add(selectListItem);
                        }
                    }

                    break;
                }
                case RequirementCategory.ShippingMethods:
                {
                    foreach (ShipmentMethodOperators shipmentMethodOperator in Enum.GetValues(
                        typeof(ShipmentMethodOperators)))
                    {
                        var selectListItems2 = requirementModel_0.AvailableProperties;
                        var selectListItem4 = new SelectListItem();
                        selectListItem4.Text =
                            shipmentMethodOperator.GetLocalizedEnum(_localizationService, _workContext);
                        selectListItem4.Value = shipmentMethodOperator.ToString();
                        selectListItem4.Selected =
                            shipmentMethodOperator.ToString() == requirementModel_0.RequirementProperty;
                        selectListItems2.Add(selectListItem4);
                    }

                    var shipmentMethodOperator1 = ShipmentMethodOperators.ShippingProvider;
                    Enum.TryParse(requirementModel_0.RequirementProperty, out shipmentMethodOperator1);
                    if (shipmentMethodOperator1 != ShipmentMethodOperators.ShippingProvider) break;
                    requirementModel_0.IsValueComboBox = true;
                    foreach (var shippingRateComputationMethod in _shippingService
                        .LoadAllShippingRateComputationMethods(null, 0))
                    {
                        var availableValues2 = requirementModel_0.AvailableValues;
                        var selectListItem5 = new SelectListItem();
                        selectListItem5.Text = shippingRateComputationMethod.PluginDescriptor.SystemName;
                        selectListItem5.Value = shippingRateComputationMethod.PluginDescriptor.SystemName;
                        availableValues2.Add(selectListItem5);
                    }

                    foreach (var pickupPointProvider in _shippingService.LoadAllPickupPointProviders(null, 0))
                    {
                        var availableValues3 = requirementModel_0.AvailableValues;
                        var selectListItem6 = new SelectListItem();
                        selectListItem6.Text = pickupPointProvider.PluginDescriptor.SystemName;
                        selectListItem6.Value = pickupPointProvider.PluginDescriptor.SystemName;
                        availableValues3.Add(selectListItem6);
                    }

                    break;
                }
                case RequirementCategory.CustomerAttributes:
                {
                    foreach (var customerAttribute in _customerAttributeService.GetAllCustomerAttributes())
                    {
                        var availableProperties3 = requirementModel_0.AvailableProperties;
                        selectListItem = new SelectListItem();
                        parameterExpression = Expression.Parameter(typeof(CustomerAttribute), "x");
                        selectListItem.Text = customerAttribute.GetLocalized(
                            Expression.Lambda<Func<CustomerAttribute, string>>(
                                Expression.Property(parameterExpression,
                                    (MethodInfo) MethodBase.GetMethodFromHandle(typeof(CustomerAttribute)
                                        .GetMethod("get_Name").MethodHandle)), parameterExpression));
                        selectListItem.Value = customerAttribute.Id.ToString();
                        id = customerAttribute.Id;
                        selectListItem.Selected = id.ToString() == requirementModel_0.RequirementProperty;
                        availableProperties3.Add(selectListItem);
                    }

                    break;
                }
                case RequirementCategory.Product:
                {
                    var allVendors = _vendorService.GetAllVendors("", 0, 2147483647, false);
                    foreach (ProductOperators productOperator in Enum.GetValues(typeof(ProductOperators)))
                    {
                        if (productOperator == ProductOperators.Vendor && !allVendors.Any()) continue;
                        var selectListItems3 = requirementModel_0.AvailableProperties;
                        var selectListItem7 = new SelectListItem();
                        selectListItem7.Text = productOperator.GetLocalizedEnum(_localizationService, _workContext);
                        selectListItem7.Value = productOperator.ToString();
                        selectListItem7.Selected = productOperator.ToString() == requirementModel_0.RequirementProperty;
                        selectListItems3.Add(selectListItem7);
                    }

                    requirementModel_0.IsValueComboBox = true;
                    var productOperator1 = ProductOperators.Category;
                    Enum.TryParse(requirementModel_0.RequirementProperty, out productOperator1);
                    switch (productOperator1)
                    {
                        case ProductOperators.Category:
                        {
                            foreach (var allCategory in _categoryService.GetAllCategories("", 0, 0, 2147483647, false)
                            )
                            {
                                var availableValues4 = requirementModel_0.AvailableValues;
                                var selectListItem8 = new SelectListItem();
                                selectListItem8.Text = allCategory.GetFormattedBreadCrumb(_categoryService, ">>", 0);
                                selectListItem8.Value = allCategory.Id.ToString();
                                availableValues4.Add(selectListItem8);
                            }

                            break;
                        }
                        case ProductOperators.Manufacturer:
                        {
                            foreach (var allManufacturer in _manufacturerService.GetAllManufacturers("", 0, 0,
                                2147483647, false))
                            {
                                var selectListItems4 = requirementModel_0.AvailableValues;
                                selectListItem = new SelectListItem();
                                parameterExpression = Expression.Parameter(typeof(Manufacturer), "x");
                                selectListItem.Text = allManufacturer.GetLocalized(
                                    Expression.Lambda<Func<Manufacturer, string>>(
                                        Expression.Property(parameterExpression,
                                            (MethodInfo) MethodBase.GetMethodFromHandle(typeof(Manufacturer)
                                                .GetMethod("get_Name").MethodHandle)), parameterExpression));
                                selectListItem.Value = allManufacturer.Id.ToString();
                                selectListItems4.Add(selectListItem);
                            }

                            break;
                        }
                        case ProductOperators.Vendor:
                        {
                            foreach (var allVendor in allVendors)
                            {
                                var availableValues5 = requirementModel_0.AvailableValues;
                                selectListItem = new SelectListItem();
                                parameterExpression = Expression.Parameter(typeof(Vendor), "x");
                                selectListItem.Text = allVendor.GetLocalized(
                                    Expression.Lambda<Func<Vendor, string>>(
                                        Expression.Property(parameterExpression,
                                            (MethodInfo) MethodBase.GetMethodFromHandle(typeof(Vendor)
                                                .GetMethod("get_Name").MethodHandle)), parameterExpression));
                                selectListItem.Value = allVendor.Id.ToString();
                                availableValues5.Add(selectListItem);
                            }

                            break;
                        }
                    }

                    break;
                }
            }

            strs.Add("==");
            strs.Add("!=");
            if (requirementCategory != RequirementCategory.Product)
            {
                strs.Add(">");
                strs.Add(">=");
                strs.Add("<");
                strs.Add("<=");
                strs.Add("in");
                strs.Add("contain");
            }

            foreach (var str in strs)
            {
                var availableOperators = requirementModel_0.AvailableOperators;
                var selectListItem9 = new SelectListItem();
                selectListItem9.Text = CommonHelper.ConvertEnum(str);
                selectListItem9.Value = str;
                selectListItem9.Selected = str == requirementModel_0.RequirementOperator;
                availableOperators.Add(selectListItem9);
            }
        }

        private string method_8(string string_0, PaymentRule paymentRule_0, int? nullable_0)
        {
            if (paymentRule_0 == null) throw new ArgumentNullException("paymentRule");
            var str = string.Concat("Admin/PaymentRulesAdmin/ConfigureOneRequirement?systemName=", string_0,
                "&externalId=", paymentRule_0.Id);
            if (nullable_0.HasValue) str = string.Concat(str, string.Format("&requirementId={0}", nullable_0.Value));
            bool? nullable = null;
            return string.Format("{0}{1}", _webHelper.GetStoreLocation(nullable), str);
        }

        private IList<PaymentRuleModel.RequirementMetaInfo> method_9(IEnumerable<PaymentRuleRequirement> ienumerable_0,
            GroupInteractionType groupInteractionType_0, PaymentRule paymentRule_0)
        {
            var paymentRuleRequirement = ienumerable_0.LastOrDefault();
            return ienumerable_0.Select(requirement =>
            {
                var requirementMetaInfo = new PaymentRuleModel.RequirementMetaInfo
                {
                    RequirementId = requirement.Id,
                    ParentId = requirement.ParentId,
                    IsGroup = requirement.IsGroup,
                    RequirementName = requirement.RequirementCategory,
                    IsLastInGroup = paymentRuleRequirement == null || paymentRuleRequirement.Id == requirement.Id,
                    InteractionTypeId = (int) groupInteractionType_0
                };
                var groupInteractionType = requirement.InteractionType.HasValue
                    ? requirement.InteractionType.Value
                    : GroupInteractionType.And;
                requirementMetaInfo.AvailableInteractionTypes = groupInteractionType.ToSelectList(true, null, true);
                if (requirement.IsGroup)
                {
                    requirementMetaInfo.ChildRequirements = method_9(requirement.ChildRequirements,
                        groupInteractionType, paymentRule_0);
                    return requirementMetaInfo;
                }

                requirementMetaInfo.RequirementName = requirement.RequirementCategory;
                requirementMetaInfo.ConfigurationUrl =
                    method_8(requirement.RequirementCategory, paymentRule_0, requirement.Id);
                return requirementMetaInfo;
            }).ToList();
        }

        
    }
}