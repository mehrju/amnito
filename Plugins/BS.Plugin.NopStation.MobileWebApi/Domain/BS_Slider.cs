using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
   public partial class BS_Slider : BaseEntity 
    {
        public int PictureId { get; set; }
        public DateTime? SliderActiveStartDate { get; set; }
        public DateTime? SliderActiveEndDate { get; set; }
        public bool IsProduct { get; set; }
        public int ProdOrCatId { get; set; }
    }
}
