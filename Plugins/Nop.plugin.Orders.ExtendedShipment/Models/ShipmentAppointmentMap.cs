using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class ShipmentAppointmentMap : EntityTypeConfiguration<ShipmentAppointmentModel>
    {
        public ShipmentAppointmentMap()
        {
            ToTable("ShipmentAppointment");
            HasKey(p => p.Id);
            Property(p => p.ShipmentId);
            Property(p => p.IsAutoPersuitCode);
            Property(p => p.PostAdminId);
            Property(p => p.PostManId);
            Property(p => p.AppointmentDate);
            Property(p => p.Barcode);
            Property(p => p.IsDefrentWeight);
            Property(p => p.DataCollect);
            Property(p => p.CodCost);
            Property(p => p.CodBmValue);
        }
    }
    public class CountryCodeMap : EntityTypeConfiguration<CountryCodeModel>
    {
        public CountryCodeMap()
        {
            ToTable("CountryCode");
            HasKey(p => p.Id);
            Property(p => p.CountryId);
            Property(p => p.CountryCode);
            Property(p => p.printCountryCode);
        }
    }
    public class StateCodeMap : EntityTypeConfiguration<StateCodemodel>
    {
        public StateCodeMap()
        {
            ToTable("StateCode");
            HasKey(p => p.Id);
            Property(p => p.stateId);
            Property(p => p.StateCode);
            Property(p => p.SenderStateCode);
        }
    }
    
}
