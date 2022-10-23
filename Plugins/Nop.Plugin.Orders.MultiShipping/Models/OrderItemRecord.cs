using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class OrderItemRecord
    {
        public int Id { get; set; }
        public int? OrderItemId { get; set; }
        public string WeightCategoryValue { get; set; }
        public int? WeightCategoryCost { get; set; }
        public int? WeightCategoryPrice { get; set; }
        public string CartonValue { get; set; }
        public int? CartonPrice { get; set; }
        public int? CartonCost { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Length { get; set; }
        public string BoxType { get; set; }
        public string SmsValue { get; set; }
        public int? SmsCost { get; set; }
        public int? SmsPrice { get; set; }
        public string PrintLogoValue { get; set; }
        public int? PrintLogoCost { get; set; }
        public int? PrintLogoPrice { get; set; }
        public int? CompulsoryInsurancePrice { get; set; }
        public int? CompulsoryInsuranceCost { get; set; }
        public string InsuranceValue { get; set; }
        public int? InsuranceCost { get; set; }
        public int? InsurancePrice { get; set; }
        public string AccessPrinterValue { get; set; }
        public int? AccessPrinterCost { get; set; }
        public int? AccessPrinterPrice { get; set; }
    }
}
