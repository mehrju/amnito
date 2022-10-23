using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Topics;
using Nop.Plugin.Misc.Ticket.Domain;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Topics;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class TopicCollectionController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storecontext;
        private readonly IRepository<Tbl_TrainingAcademyTopic> _repositoryTbl_TrainingAcademyTopic;
        private readonly ITopicService _topicservice;
        private readonly IStoreMappingService _storeMappingService;

        public TopicCollectionController
            (
            IWorkContext workContext,
            IStoreService storeService,
            IStoreContext storecontext,
            IRepository<Tbl_TrainingAcademyTopic> repositoryTbl_TrainingAcademyTopic,
            ITopicService topicservice,
             IStoreMappingService storeMappingService
            )
        {
            this._workContext = workContext;
            this._storeService = storeService;
            _storecontext = storecontext;
            _repositoryTbl_TrainingAcademyTopic = repositoryTbl_TrainingAcademyTopic;
            _topicservice = topicservice;
            _storeMappingService = storeMappingService;
        }
        public IActionResult Index()
        {
            var model = new vm_TopicCollection();
            model.ItemTopics = new List<ItemTopic>();
            var list = _repositoryTbl_TrainingAcademyTopic.Table.Where(p => p.IsActive).ToList();

            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    ItemTopic temp = new ItemTopic();
                    temp.DateInsert = ConvertToJalali(item.DateInsert);
                    temp.DateUpdate = item.DateUpdate!=null? ConvertToJalali(item.DateUpdate.GetValueOrDefault()):"";
                    temp.UrlImage = "/ImageTrainingTopic/" + item.UrlImage;
                    temp.UrlPage = Urlp(item.IdTopic);
                    temp.Title = _topicservice.GetTopicById(item.IdTopic).Title;
                    temp.SystemName = _topicservice.GetTopicById(item.IdTopic).SystemName;
                    temp.idStore = GetNameStorTopic(item.IdTopic).Item2;
                    model.ItemTopics.Add(temp);
                }
            }


            if (_storecontext.CurrentStore.Id == 5)
            {
                model.ItemTopics = model.ItemTopics.Where(p => p.idStore.Contains(5)).ToList();
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Shipito/TopicCollection.cshtml", model);

            }
            else if (_storecontext.CurrentStore.Id == 3)
            {
                model.ItemTopics = model.ItemTopics.Where(p => p.idStore.Contains(3)).ToList();
                return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Postbar/TopicCollection.cshtml", model);

            }
            else
            {
                string test = HttpContext.Session.GetString("ComeFrom");
                if (test == "Ap")
                    return View("~/Plugins/Orders.MultiShipping/Views/NewCheckout/Ap/TopicCollection.cshtml", model);
                else
                    return Challenge();
            }
        }

        private string ConvertToJalali(DateTime dt)
        {
            PersianCalendar pr = new PersianCalendar();
            return pr.GetYear(dt) + "/" + pr.GetMonth(dt).ToString("00") + "/" +
                   pr.GetDayOfMonth(dt).ToString("00") + " " + pr.GetHour(dt).ToString("00") + ":" + pr.GetMinute(dt).ToString("00")
                   + ":" + pr.GetSecond(dt).ToString("00");
        }
        public string Urlp(int id)
        {
            Topic topic = _topicservice.GetTopicById(id);
            string s = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            return s;
        }
        public (string, List<int>) GetNameStorTopic(int Id)
        {
            string result = "";
            List<int> Storids = new List<int>();
            if (Id > 0)
            {
                Topic topic = _topicservice.GetTopicById(Id);
                if (topic.LimitedToStores == false)
                {
                    //all
                    result = "همه فروشگاهها";
                    var t = _storeService.GetAllStores(false).ToList();
                    if (t.Count > 0)
                    {
                        foreach (var item in t)
                        {
                            Storids.Add(item.Id);
                        }
                    }
                }
                else
                {

                }
                var listidstor = _storeMappingService.GetStoresIdsWithAccess(topic).ToList();
                if (listidstor.Count > 0)
                {
                    Storids = listidstor;
                    foreach (var item in listidstor)
                    {
                        result += _storeService.GetStoreById(item).Name;
                        if (listidstor.Count > 1)
                        {
                            result += ", ";
                        }

                    }
                }
            }
            return (result, Storids);
        }
    }
}
