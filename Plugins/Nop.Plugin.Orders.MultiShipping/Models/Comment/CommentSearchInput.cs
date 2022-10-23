using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Models.Comment
{
    public class CommentSearchInput
    {
        public int FromCityId { get; set; }
        public int ToCityId { get; set; }
        public string WeightCategory { get; set; }
    }
}
