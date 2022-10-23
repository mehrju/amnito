using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.GeoPoint
{
    public class Tb_CountryGeoPoints : BaseEntity
    {
        [Required]
        public int CountryId { get; set; }
        [Required]
        public decimal Lat { get; set; }
        [Required]
        public decimal Lon { get; set; }
    }
}
