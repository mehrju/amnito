using Microsoft.AspNetCore.Mvc;

using Nop.Plugin.Misc.PrintedReports_Requirements.Service.Interface;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Web.Controllers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
	public class NewHomeController : BasePublicController
	{
		private readonly I_IndexPageService _I_IndexPageService;
		public NewHomeController(I_IndexPageService I_IndexPageService)
		{
			_I_IndexPageService = I_IndexPageService;
		}
		public IActionResult Index()
		{
			var Model = new Nop.Plugin.Orders.MultiShipping.Models.vm_Index();
			Model.List_Item_RSS = new List<Item>();
			try
			{
				#region rss
				var rssFeed = new Uri("https://mag.postbar.ir/?call_custom_simple_rss=1");
				var request = (HttpWebRequest)WebRequest.Create(rssFeed);
				request.Method = "GET";
				var response = (HttpWebResponse)request.GetResponse();
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var feedContents = reader.ReadToEnd();
					var document = XDocument.Parse(feedContents);



					//====================================
					var posts = (from p in document.Descendants("item")
								 select new
								 {
									 Title = (p.Element("title") == null) ? "" : p.Element("title").Value,
									 Link = (p.Element("link") == null) ? "" : p.Element("link").Value,
									 Comments = (p.Element("comments") == null) ? "" : p.Element("comments").Value,
									 PubDate = (p.Element("pubDate") == null) ? "" : (p.Element("pubDate").Value),
									 Description = (p.Element("description") == null) ? "" : p.Element("description").Value,
									 category = (p.Element("category") == null) ? "" : p.Element("category").Value,
									 Source = (p.Element("enclosure") == null) ? "" : (string)p.Element("enclosure").Attribute("url").Value

								 }).Where(p => p.category != "135").ToList();

					foreach (var post in posts)
					{
						Nop.Plugin.Orders.MultiShipping.Models.Item temp = new Nop.Plugin.Orders.MultiShipping.Models.Item();
						temp.Title = post.Title;
						temp.Link = post.Link;
						temp.Comments = post.Comments;
						temp.PubDate = post.PubDate;
						temp.Description = post.Description;
						temp.Source = post.Source;
						Model.List_Item_RSS.Add(temp);
					}
				}
				#endregion
			}
			catch
			{

			}
			finally
			{
				Model.vmSlidShow_Index = _I_IndexPageService.GetSlidshowAsync();
			}
			string submarket = "Postbar";
			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/{submarket}/Index.cshtml", Model);
		}

		public IActionResult IranPost()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/IranPost.cshtml");
		}

		public IActionResult DomesticPost()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/DomesticPost.cshtml");
		}
		public IActionResult Peykhub()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Peykhub.cshtml");
		}

		public IActionResult OnlineOrder()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/DomesticPost/OnlineOrder.cshtml");
		}

		public IActionResult CodOrder()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/DomesticPost/CodOrder.cshtml");
		}
		public IActionResult AmnitoOrder()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/DomesticPost/AmnitoOrder.cshtml");
		}
		public IActionResult BulkOrder()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/DomesticPost/BulkOrder.cshtml");
		}

		public IActionResult OrganizationOrder()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/DomesticPost/OrganizationOrder.cshtml");
		}

		public IActionResult Shopex()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/DomesticPost/Shopex.cshtml");
		}

		public IActionResult Wallet()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Wallet.cshtml");
		}
		public IActionResult Agencies()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Agencies.cshtml");
		}
		public IActionResult Packing()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Packing.cshtml");
		}
		public IActionResult ApiRequest()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Api-request.cshtml");
		}
		//public IActionResult Postbar()
		//{

		//	return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/Postbar.cshtml");
		//}
		public IActionResult Postex()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Postex.cshtml");
		}
		public IActionResult Chapar()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/Chapar.cshtml");
		}
		public IActionResult Mahex()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/Mahex.cshtml");
		}
		public IActionResult Yarbox()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/Yarbox.cshtml");
		}
		public IActionResult PDE()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/PDE.cshtml");
		}
		public IActionResult RaheAsemanAbi()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/RaheAsemanAbi.cshtml");
		}
		public IActionResult TPG()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/TPG.cshtml");
		}
		public IActionResult SnappBox()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/SnappBox.cshtml");
		}
		public IActionResult PersiaExpress()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/PersiaExpress.cshtml");
		}
		public IActionResult Obar()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Partners/Obar.cshtml");
		}
		public IActionResult ForeignPost()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/ForeignPost.cshtml");
		}

		public IActionResult Kadox()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Kadox.cshtml");
		}

		public IActionResult ShipitoPlus()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/ShipitoPlus.cshtml");
		}
		public IActionResult PostexYar()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/PostexYar.cshtml");
		}
		public IActionResult Coldex()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Coldex.cshtml");
		}
		public IActionResult Subex()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/Subex.cshtml");
		}
		public IActionResult HeavyPost()
		{

			return View($"~/Plugins/Orders.MultiShipping/Views/NewCheckout/PostexNew/HeavyPost.cshtml");
		}
	}

}


