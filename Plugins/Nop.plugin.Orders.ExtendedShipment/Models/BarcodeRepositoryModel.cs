using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class BarcodeRepositoryModel:BaseEntity
    {
        [XmlElement("Barcode")]
        [Index(name:"IX_Barcode",IsClustered =false,IsUnique = true)]
        public string Barcode { get; set; }
        public int? ShipmentId { get; set; }
    }
    [XmlRoot("ArrayOfBarcodeList")]
    public class BarcodeList
    {
        [XmlElement("BarcodeList")]
        public List<BarcodeRepositoryModel> list { get; set; }
    }

}
