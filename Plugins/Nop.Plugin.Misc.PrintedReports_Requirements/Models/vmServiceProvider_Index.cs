using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models
{
    public class vmServiceProvider_Index
    {
        public vmServiceProvider_Index()
        {
            List_Provider = new List<ItemServiceProvider>();
        }
        public List<ItemServiceProvider> List_Provider { get; set; }
    }
    public class ItemServiceProvider
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image_Url { get; set; }
        public string PageDiscription_Url { get; set; }

    }
}
