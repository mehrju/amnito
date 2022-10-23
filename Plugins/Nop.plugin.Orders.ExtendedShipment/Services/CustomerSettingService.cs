using Nop.Core;
using Nop.plugin.Orders.ExtendedShipment.Models;
using Nop.Services.Common;
using Nop.Services.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class CustomerSettingService : ICustomerSettingService
    {
        private readonly ICustomerService _customerService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;

        public CustomerSettingService(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext)
        {
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
        }

        public CustomerSetting GetCustomerSetting(int customerId = 0)
        {
            if(customerId == 0)
            {
                if(_workContext.CurrentCustomer != null)
                customerId = _workContext.CurrentCustomer.Id;
            }
            if(customerId == 0)
            {
                return null;
            }

            var customer = _customerService.GetCustomerById(customerId);

            CustomerSetting setting = new CustomerSetting();
            setting.ShowDiscountOnPrintPdf = customer.GetAttribute<bool>(nameof(setting.ShowDiscountOnPrintPdf));
            return setting;
        }
    }
}
