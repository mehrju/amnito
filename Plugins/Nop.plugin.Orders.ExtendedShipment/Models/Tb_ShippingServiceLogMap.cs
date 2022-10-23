using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
	public class Tb_ShippingServiceLogMap : EntityTypeConfiguration<Tb_ShippingServiceLog>
	{
		public Tb_ShippingServiceLogMap()
		{
			ToTable("Tb_ShippingServiceLog");
			HasKey(m => m.Id);
			Property(m => m.xPostUserName);
			Property(m => m.xPostPassword);
			Property(m => m.xPostType);
			Property(m => m.xBarcode_Out);
			Property(m => m.xResponseDate);
			Property(m => m.xRequestDate);
			Property(m => m.xStateCode);
			Property(m => m.xSenderStateCode);
			Property(m => m.xErrorMessage);

		}
	}
}
