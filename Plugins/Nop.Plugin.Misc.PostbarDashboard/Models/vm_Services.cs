using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PostbarDashboard.Models
{
   public class vm_Services
    {
        public List<ItemService> ListServices { get; set; }
    }
    public class ItemService
    {
        public string UrlImage { get; set; }
        public string Name { get; set; }
        public string Discription { get; set; }
        public string UrlPage { get; set; }
        public string UrlPardukht { get; set; }
        public decimal Price { get; set; }

    }
}
