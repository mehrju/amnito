using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class CategoryInfoModel : BaseEntity
    {
        public int CategoryId { get; set; }
        public bool IsCod { get; set; }
        public bool IsPishtaz { get; set; }
        public bool IsSefareshi { get; set; }
        public bool IsVIje { get; set; }
        public bool IsNromal { get; set; }
        public bool IsDroonOstani { get; set; }
        public bool IsAdjoining { get; set; }
        public bool IsNotAdjacent { get; set; }
        public bool IsHeavyTransport { get; set; }
        public bool IsForeign { get; set; }
        public bool IsInCity { get; set; }
        public bool IsAmanat { get; set; }
        public bool IsTwoStep { get; set; }
        public bool HasHagheMaghar { get; set; }
        public bool IsPrivatepost { get; set; }
        public bool IsCollectorDistibuter { get; set; }
        public bool NeedToDistribution { get; set; }
        public string CategoryName { get; set; }
    }
}
