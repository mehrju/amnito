using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    public partial class BS_CategoryIcons : BaseEntity
    {
        public int SubCategoryId { get; set; }

        public string Extension { get; set; }
    }
}
