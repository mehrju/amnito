using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.PhoneOrder
{
    public class PhoneOrderModel
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        /// <summary>
        /// شماره همراه
        /// </summary>
        [Required]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// شماره ثابت
        /// </summary>
        [Required]
        public string TellNumber { get; set; }
        [Range(1, int.MaxValue)]
        public int CityId { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string PostalCode { get; set; }
        public bool IsCarRequired { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public bool HasLocation { get; set; }

        public DateTime CreateDate { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public int StateId { get; set; }
        [Range(1, int.MaxValue)]
        public int ServiceId { get; set; }
        public bool HasOrder { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        public string Error { get; set; }
        public int? RelatedCustomerId { get; set; }
        public int? OrderId { get; set; }
    }
}
