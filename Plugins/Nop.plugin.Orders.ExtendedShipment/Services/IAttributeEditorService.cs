using Nop.Core.Domain.Customers;
using System.Collections.Generic;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public interface IAttributeEditorService
    {
        void EditOrderItemAttributes(Nop.Core.Domain.Catalog.Product product, Customer customer, string attributeXml, int orderItemId, Dictionary<string, string> keyValuePairs);
    }
}