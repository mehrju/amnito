using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Web.Models.Media;


namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class MenufactureOverViewModelApi
    {
         public MenufactureOverViewModelApi()
        {
            DefaultPictureModel = new DashboardModel.PictureModel();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public DashboardModel.PictureModel DefaultPictureModel ;
    }
}
