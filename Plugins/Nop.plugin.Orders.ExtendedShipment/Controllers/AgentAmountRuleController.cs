using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.plugin.Orders.ExtendedShipment.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    public class AgentAmountRuleController : BaseAdminController
    {
        private readonly IProductService _productService;
        private readonly IAgentAmountRuleService _agentAmountRuleService;
        private readonly IExtendedShipmentService _extendedShipmentService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _StoreContext;

        #region Define Agent Amount Rule
        public AgentAmountRuleController(IProductService productService
            , IAgentAmountRuleService agentAmountRuleService
            , IExtendedShipmentService extendedShipmentService
            , IGenericAttributeService genericAttributeService
            , IWorkContext workContext
            , IStoreContext StoreContext)
        {
            _StoreContext = StoreContext;
            _productService = productService;
            _agentAmountRuleService = agentAmountRuleService;
            _extendedShipmentService = extendedShipmentService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }
        public IActionResult Index()
        {
            var model = new AgentAmountRuleModel();
            model.AvailableProduct = new List<SelectListItem>();
            model.AvailableProduct.Add(new SelectListItem() { Value = "0", Text = "انتخاب کنید....", Selected = true });
            model.AvailableProduct.AddRange(_extendedShipmentService.ListOfService());
            return View("/Plugins/Orders.ExtendedShipment/Views/AgentAmountRule.cshtml", model);
        }
        [HttpGet]
        public IActionResult GetAgentList(string searchtext)
        {
            var data = _agentAmountRuleService.getAvailableAgentList(searchtext).Select(p=> new {id=p.Value,text=p.Text }).ToList();
            return Json(new { results = data });
        }
        public IActionResult DefineAgentAmountRuleIndex()
        {
            var model = new AgentAmountRuleModel();
            model.AvailableProduct = _extendedShipmentService.ListOfService();
            return View("/Plugins/Orders.ExtendedShipment/Views/DefineAgentAmountRule.cshtml", model);
        }
        public IActionResult getProductAttribute(int productId)
        {
            var product = _productService.GetProductById(productId);
            List<SelectListItem> productAttrList = new List<SelectListItem>()
            {
                new SelectListItem(){Value = "0",Text = "انتخاب کنید....",Selected = true}
            };
            var DataItem = product.ProductAttributeMappings.Select(p => p.ProductAttribute).Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
            productAttrList.AddRange(DataItem);
            return Json(productAttrList);
        }
        public IActionResult getProductAttributeValue(int productId, int productAttributeId)
        {
            var product = _productService.GetProductById(productId);
            List<SelectListItem> productAttrValueList = new List<SelectListItem>()
            {
                new SelectListItem(){Value = "0",Text = "انتخاب کنید....",Selected = true}
            };
            var DataItem = product.ProductAttributeMappings.Where(p => p.ProductAttributeId == productAttributeId).SelectMany(p => p.ProductAttributeValues).Select(p => new SelectListItem()
            {
                Text = p.Name,
                Value = p.Id.ToString()
            }).ToList();
            productAttrValueList.AddRange(DataItem);
            return Json(productAttrValueList);
        }
        [HttpPost]
        public IActionResult SaveAgentAmountRule(AgentAmountRuleModel model)
        {
            string error = "";
            var result = _agentAmountRuleService.SaveAgentAmountRule(model, out error);
            return Json(new { success = result, message = error });
        }
        [HttpPost]
        public IActionResult getAgentAmountList(DataSourceRequest command)
        {
            int count = 0;
            var data = _agentAmountRuleService.getList();
            data.ForEach(p => p.CreateDate = p.CreateDate.Value.ToLocalTime());
            var gridModel = new DataSourceResult
            {
                Data = data,
                Total = data.Count
            };
            return Json(gridModel);
        }
        [HttpPost]
        public IActionResult DeleteAgentAmountList(int agentAmountRuleId)
        {
            var result = _agentAmountRuleService.DdeleteAgentAmountRule(agentAmountRuleId);
            return Json(new { success = result, message = (result ? "عملیات با موفقیت انجام شد" : "بروز خطا در زمان حذف") });
        }
        [HttpPost]
        public IActionResult UpdateAgentAmountRule(AgentAmountRuleModel model)
        {
            string str_error = "";
            var result = _agentAmountRuleService.updateAgentAmountRule(model,out str_error);
            return Json(new { success = result, message = str_error });
        }
        #endregion

        #region Assign Agent Maount Rule
        public IActionResult AssignAgentAmountRuleIndex()
        {
            return View("/Plugins/Orders.ExtendedShipment/Views/AssignAgentAmountRule.cshtml");
        } 
        public IActionResult SaveAssignAgentAmountRule(AssignAgentAmountRuleModel model)
        {
            string error = "";
            var result = _agentAmountRuleService.SaveAssignAgentAmountRule(model, out error);
            return Json(new { success = result, message = error });
        }
        [HttpPost]
        public IActionResult DeActiveAssignAgentAmountRule(int Id)
        {
            string error = "";
            _agentAmountRuleService.DeActiveAssignAgentAmountRule(Id, out error);
            return Json(new { message = error });
        }
        [HttpPost]
        public IActionResult getAssignAgentAmountList(DataSourceRequest command, AssignAgentAmountRuleModel model)
        {
            var data = _agentAmountRuleService.getAssignAgentAmountList(model);
            data.ForEach(p => p.AssignmentDate = p.AssignmentDate.Value.ToLocalTime());
            var gridModel = new DataSourceResult
            {
                Data = data,
                Total = data.Count
            };
            return Json(gridModel);
        }
        #endregion

        [HttpPost]
        public virtual IActionResult ChangeAgentAmountRuleEnable(string name, bool value)
        {
            //permission validation is not required here
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            _genericAttributeService.SaveAttribute(_StoreContext.CurrentStore, name, value);

            return Json(new
            {
                Result = true
            });
        }
    }
}
