using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NopFarsi.Payments.SepShaparak.Models
{
    public class SepTokenModel
    {
        public string Action { get; set; }
        public string TerminalId { get; set; }
        public string RedirectUrl { get; set; }
        public string ResNum { get; set; }
        public long Amount { get; set; }
        public long CellNumber { get; set; }
    }
}

