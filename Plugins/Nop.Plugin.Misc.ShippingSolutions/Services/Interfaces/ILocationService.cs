using Nop.Plugin.Misc.ShippingSolutions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public interface ILocationService
    {
        void InsertAddressLocation(int AddressId, float Lat, float Lon);
        Tbl_Address_LatLong getAddressLocation(int AddressId);
    }
}
