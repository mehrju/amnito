using Nop.Core;
using Nop.Core.Data;
using Nop.Plugin.Misc.PostbarDashboard.Models;
using Nop.Plugin.Misc.PrintedReports_Requirements.Models;
using Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Nop.Plugin.Misc.PrintedReports_Requirements.Service
{
    public class IndexPageService : I_IndexPageService
    {
        private readonly IRepository<Tbl_ServiceProviderDashboard> _repositoryTbl_ServiceProviderDashboard;
        private readonly IRepository<Tbl_Carousel_slideshow> _repositoryTbl_Carousel_slideshow;
        private readonly IStoreContext _storeContext;
        public IndexPageService
            (
            IRepository<Tbl_ServiceProviderDashboard> repositoryTbl_ServiceProviderDashboard, IRepository<Tbl_Carousel_slideshow> repositoryTbl_Carousel_slideshow, IStoreContext storeContext
            )
        {
            _repositoryTbl_Carousel_slideshow = repositoryTbl_Carousel_slideshow;
            _repositoryTbl_ServiceProviderDashboard = repositoryTbl_ServiceProviderDashboard;
            _storeContext = storeContext;
        }


        public vmSlidShow_Index GetSlidshowAsync()
        {
            vmSlidShow_Index Model = new vmSlidShow_Index();
            Model.List_SlideShow = new List<ItemSlideShow>();
            try
            {
                #region list slidshow
                List<Tbl_Carousel_slideshow> Listsl = _repositoryTbl_Carousel_slideshow.Table.Where(p => p.IsActive && p.DateStart <= DateTime.Now && (p.DateExpire == null || p.DateExpire >= DateTime.Now) && p.LimitedStore.Contains(_storeContext.CurrentStore.Id.ToString())).ToList();

                if (Listsl.Count > 0)
                {

                    foreach (var item in Listsl)
                    {
                        ItemSlideShow t = new ItemSlideShow();
                        t.Title = item.Title_Carousel_slideshow;
                        t.Discription = item.Discrition_Carousel_slideshow;
                        t.UrlImage = "/ImageSlideShowDashboard/" + item.UrlImage;
                        t.UrlImageMobile = "/ImageSlideShowDashboard/" + item.UrlImageMobile;
                        t.UrlPage = item.UrlPage != null ? item.UrlPage : "#";
                        t.IsVideo = item.IsVideo;
                        t.TimeInterval = item.TimeInterval;
                        t.Duration = 2.5;// await (t.IsVideo == true ? getdurationAsync(item.UrlPage) : null);
                        Model.List_SlideShow.Add(t);
                    }
                }

                #endregion
                return Model;
            }
            catch (Exception ex)
            {
                return Model;
            }
           
            
        }
        public async Task<double> getdurationAsync(string s)
        {

            IMediaInfo mediaInfo = await MediaInfo.Get(s);
            TimeSpan videoDuration = mediaInfo.VideoStreams.First().Duration;
            return videoDuration.TotalMinutes;
            //WMPLib.WindowsMediaPlayerClass wmp = new WMPLib.WindowsMediaPlayerClass();
            //string FilePath = "yourFilePath";
            //IWMPMedia mediaInfo = wmp.newMedia(FilePath);
            //return 0;
        }

        public vmServiceProvider_Index GetServiceProvider()
        {
            vmServiceProvider_Index Model = new vmServiceProvider_Index();
            Model.List_Provider = new List<ItemServiceProvider>();
            try
            {
                #region list service provider
                List<Tbl_ServiceProviderDashboard> Listp = _repositoryTbl_ServiceProviderDashboard.Table.Where(p => p.IsActive).ToList();
                if (Listp.Count > 0)
                {
                    foreach (var item in Listp)
                    {
                        ItemServiceProvider t = new ItemServiceProvider();
                        t.Id = item.Id;
                        t.Title = item.TitleServiceProviderDashboard;
                        t.PageDiscription_Url = item.UrlPageDiscreption != null ? item.UrlPageDiscreption : "#";
                        t.Image_Url = "/ImageServiceProviderDashboard/" + item.UrlImage;
                        Model.List_Provider.Add(t);
                    }
                }

                #endregion
                return Model;
            }
            catch(Exception ex)
            {
                return Model;
            }
           
        }

    }
}
