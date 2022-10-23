using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.GeoPoint
{
    public class Tb_StateProvinceGeoPoints : BaseEntity
    {
        [Required]
        public int StateProvinceId { get; set; }
        [Required]
        public decimal Lat { get; set; }
        [Required]
        public decimal Lon { get; set; }
    }
}
