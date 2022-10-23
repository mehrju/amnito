using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class UserStetesModel:BaseEntity
    {
        [DisplayName("کاربر")]
        public int CustomerId { get; set; }

        public int StateId { get; set; }
        [NotMapped]
        [DisplayName("نقش")]
        public int RoleId { get; set; }
        [NotMapped]
        [DisplayName("استان")]
        public int CountryId { get; set; }
        [NotMapped]
        public IList<SelectListItem> AvailableCountries { get; set; }
        [NotMapped]
        public IList<SelectListItem> AvailableRoles { get; set; }
        [NotMapped]
        public IList<SelectListItem> AvailableCustomer { get; set; }


    }
}
