using Nop.Core.Data;
using Nop.Plugin.Misc.ShippingSolutions.Domain;
using Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<Tbl_Address_LatLong> _repositoryTbl_Address_LatLong;
        public LocationService(IRepository<Tbl_Address_LatLong> repositoryTbl_Address_LatLong)
        {
            _repositoryTbl_Address_LatLong = repositoryTbl_Address_LatLong;
        }
        public void InsertAddressLocation(int AddressId,float Lat,float Lon)
        {
            Tbl_Address_LatLong model = new Tbl_Address_LatLong() {
                AddressId = AddressId,
                Lat = Lat,
                Long = Lon
            };
            _repositoryTbl_Address_LatLong.Insert(model);
        }
        public Tbl_Address_LatLong getAddressLocation(int AddressId)
        {
            return _repositoryTbl_Address_LatLong.Table.Where(p => p.AddressId == AddressId).FirstOrDefault();
        }
    }
}
