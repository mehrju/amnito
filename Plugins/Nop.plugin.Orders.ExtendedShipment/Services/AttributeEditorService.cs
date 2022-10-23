using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class AttributeEditorService : IAttributeEditorService
    {
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IDbContext _dbContext;
        private readonly IProductAttributeFormatter _productAttributeFormatter;

        public AttributeEditorService(IProductAttributeParser productAttributeParser,
            IDbContext dbContext,
            IProductAttributeFormatter productAttributeFormatter)
        {
            _productAttributeParser = productAttributeParser;
            _dbContext = dbContext;
            _productAttributeFormatter = productAttributeFormatter;
        }

        public void EditOrderItemAttributes(Nop.Core.Domain.Catalog.Product product, Customer customer,string attributeXml, int orderItemId, Dictionary<string, string> keyValuePairs)
        {
            var prAttrMapp = _productAttributeParser.ParseProductAttributeMappings(attributeXml);

            foreach (var item in keyValuePairs)
            {
                var attr = prAttrMapp.First(p => p.ProductAttribute.Name.Contains(item.Key));

                attributeXml = _productAttributeParser.AddProductAttribute(_productAttributeParser.RemoveProductAttribute(attributeXml, attr), attr, item.Value);
            }

            var newAttrs = _productAttributeParser.ParseProductAttributeValues(attributeXml);

            var attrDesc = _productAttributeFormatter.FormatAttributes(product, attributeXml, customer);

            _dbContext.ExecuteSqlCommand($"UPDATE OrderItem SET AttributesXml = N'{attributeXml}',AttributeDescription = N'{attrDesc}' where Id = {orderItemId}");
        }
    }
}
