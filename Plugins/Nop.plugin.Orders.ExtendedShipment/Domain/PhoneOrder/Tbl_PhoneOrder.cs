using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain.PhoneOrder
{
    public class Tbl_PhoneOrder : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// شماره ثابت
        /// </summary>
        public string TellNumber { get; set; }
        public int CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public bool IsCarRequired { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool HasLocation { get; set; }
        public DateTime CreateDate { get; set; }
        public int ServiceId { get; set; }
    }
}
