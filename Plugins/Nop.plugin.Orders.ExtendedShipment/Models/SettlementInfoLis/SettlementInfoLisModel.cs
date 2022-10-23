using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models.SettlementInfoLis
{
    public class SettlementInfoLisModel:BaseEntity
    {
        [DisplayName("شماره فایل")]
        public int FileId { get; set; }
        [DisplayName("نام کاربری")]
        public string UserName { get; set; }
        [DisplayName("شماره سفارش")]
        public int OrderId { get; set; } 
        [DisplayName("تاریخ واریز به کیف پول از")]
        [UIHint("DateNullable")]
        public DateTime? DepositDateFrom { get; set; }
        [DisplayName("تاریخ واریز به کیف پول تا")]
        [UIHint("DateNullable")]
        public DateTime? DepositDateTo { get; set; }
        [UIHint("DateNullable")]
        [DisplayName("تاریخ تسویه از")]
        public DateTime? SettlementDateFrom { get; set; }
        [UIHint("DateNullable")]
        [DisplayName("تاریخ تسویه تا")]
        public DateTime? SettlementDateTo { get; set; }
    }
}
