using Nop.Plugin.Misc.PrintedReports_Requirements.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class vm_Index
    {
        public List<Item> List_Item_RSS { get; set; }
        public vmSlidShow_Index vmSlidShow_Index { get; set; }
        public vmServiceProvider_Index vmServiceProvider_Index { get; set; }
        public bool HideForItSaz { get; set; }
        public bool HideForConam { get; set; }


    }
    public class Item
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Comments { get; set; }
        public string PubDate { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public int category { get; set; }
    }
    

   
}
