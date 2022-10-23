using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models
{
    public class vm_TopicCollection
    {
        public List<ItemTopic> ItemTopics { get; set; }
    }
    public class ItemTopic
    {
        public string  UrlImage { get; set; }
        public string  Title { get; set; }
        public string  SystemName { get; set; }
        public string  DateInsert { get; set; }
        public string  DateUpdate { get; set; }
        public string  UrlPage { get; set; }

        public List<int> idStore { get; set; }
    }
}
