using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Events;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using Nop.plugin.Orders.ExtendedShipment.Services.Tozico;
using Nop.Services.Common;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Events
{
    public class CustomerEventConsumer : IConsumer<EntityUpdated<Customer>>, IConsumer<EntityInserted<Customer>>
    {
        private readonly IRepository<Tbl_CustomerDepositCode> _repository_CustomerDepositCode;
        private readonly ITozicoService _tozicoService;

        public CustomerEventConsumer(IRepository<Tbl_CustomerDepositCode> repository_CustomerDepositCode,
            ITozicoService tozicoService)
        {
            _repository_CustomerDepositCode = repository_CustomerDepositCode;
            _tozicoService = tozicoService;
        }

        public void HandleEvent(EntityUpdated<Customer> eventMessage)
        {
            CheckAndInsertDepositCode(eventMessage.Entity);
            CheckAndInsertTozicoBranch(eventMessage.Entity);
        }

        private void CheckAndInsertTozicoBranch(Customer entity)
        {
            if (HasRole(entity))
            {
                _tozicoService.AddOrUpdateBranches(new List<Models.Tozico.Input.Branch>()
                {
                    new Models.Tozico.Input.Branch()
                    {
                        Id = entity.Id,
                        Description = "",
                        IsActive = true,
                        Name = entity.GetAttribute<string>( SystemCustomerAttributeNames.FirstName ) +  " " + entity.GetAttribute<string>( SystemCustomerAttributeNames.LastName ),
                        Phone= entity.Username,
                        AreaArray = new List<double[]>
                        {

                        }
                    }
                });
            }
            //else if (entity.IsRegistered())
            //{
            //    _tozicoService.AddOrUpdateBranches(new List<Models.Tozico.Input.Branch>()
            //    {
            //        new Models.Tozico.Input.Branch()
            //        {
            //            Id = entity.Id,
            //            IsActive = false
            //        }
            //    });
            //}
        }

        private void CheckAndInsertDepositCode(Customer entity)
        {
            if (HasRole(entity))
            {
                if (!_repository_CustomerDepositCode.Table.Any(p => p.CustomerId == entity.Id))
                {
                    _repository_CustomerDepositCode.Insert(new Tbl_CustomerDepositCode()
                    {
                        CustomerId = entity.Id
                    });
                }
            }
        }

        private static bool HasRole(Customer entity)
        {
            return entity.CustomerRoles.Any(p => p.Id == 31);
        }

        public void HandleEvent(EntityInserted<Customer> eventMessage)
        {
            CheckAndInsertDepositCode(eventMessage.Entity);
            CheckAndInsertTozicoBranch(eventMessage.Entity);
        }
    }
}
