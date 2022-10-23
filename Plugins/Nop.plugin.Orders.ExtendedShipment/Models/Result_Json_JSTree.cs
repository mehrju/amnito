using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Models
{
    public class Result_Json_JSTree
    {
        public int id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public List<Child> children { get; set; }
    }
    public class Child
    {
        public int id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
    }
}
