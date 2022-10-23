using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Domain
{
    public class Tbl_CustomerDepositCode : BaseEntity
    {
        public int CustomerId { get; set; }
        [NotMapped]
        public string DepositCode
        {
            get
            {
                return Id.ToString().PadLeft(3, '0');
            }
        }
    }
}
