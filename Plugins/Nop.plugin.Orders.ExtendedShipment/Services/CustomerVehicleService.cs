using Nop.Core.Data;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    public class CustomerVehicleService : ICustomerVehicleService
    {
        private readonly IRepository<Tbl_CustomerVehicle> _repository_TblCustomerVehicle;

        public CustomerVehicleService(IRepository<Tbl_CustomerVehicle> repository_TblCustomerVehicle)
        {
            _repository_TblCustomerVehicle = repository_TblCustomerVehicle;
        }

        public List<Tbl_CustomerVehicle> GetCustomerVehicles(int customerId)
        {
            return _repository_TblCustomerVehicle.Table.Where(p =>!p.IsDelete && p.CustomerId == customerId).ToList();
        }


        public Tbl_CustomerVehicle GetCustomervehicleById(int Id)
        {
            return _repository_TblCustomerVehicle.GetById(Id);
        }


        public void SaveCustomerVehicle(Tbl_CustomerVehicle model)
        {
            if(model.Id == 0)
            {
                _repository_TblCustomerVehicle.Insert(model);
            }
            else
            {
                var entity = _repository_TblCustomerVehicle.GetById(model.Id);
                entity.IsActive = model.IsActive;
                entity.Name = model.Name;
                entity.Phone = model.Phone;
                entity.VehicleTypeEnum = model.VehicleTypeEnum;
                entity.CapacityCount = model.CapacityCount;
                entity.CapacityPercent = model.CapacityPercent;
                entity.CapacityVolume = model.CapacityVolume;
                entity.CapacityWeight = model.CapacityWeight;
                entity.Description = model.Description;
                _repository_TblCustomerVehicle.Update(entity);
            }
        }

        public void RemoveCustomerVehicle(Tbl_CustomerVehicle entity)
        {
            entity.IsDelete = true;
            _repository_TblCustomerVehicle.Update(entity);
        }
    }
}
