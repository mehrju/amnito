using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
	public class Tb_ShippingServiceLog : BaseEntity
	{
		public string xPostUserName { get; set; }
		public string xPostPassword { get; set; }
		public string xSenderStateCode { get; set; }
		public string xStateCode { get; set; }
		public int xPostType { get; set; }
		public string xBarcode_Out { get; set; }
		public string xErrorMessage { get; set; }
		public DateTime? xRequestDate { get; set; }
		public DateTime? xResponseDate { get; set; }

	}
}
