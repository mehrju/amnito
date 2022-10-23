using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Models
{
    
    public class vmSlidShow_Index
    {
        public List<ItemSlideShow> List_SlideShow { get; set; }
    }
    public class ItemSlideShow
    {
        public string Title { get; set; }
        public string UrlImage { get; set; }
        public string UrlImageMobile { get; set; }
        public string Discription { get; set; }
        public string UrlPage { get; set; }
        public bool IsVideo { get; set; }
        public int TimeInterval { get; set; }
        public double Duration { get; set; }
    }
}
