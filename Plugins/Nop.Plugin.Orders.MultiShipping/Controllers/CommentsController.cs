using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Models.Comment;
using Nop.Plugin.Orders.MultiShipping.Services.Comment;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Orders.MultiShipping.Controllers
{
    public class CommentsController : BasePublicController
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        public IActionResult Index()
        {
            return View("Comments/index");
        }

        public IActionResult List()
        {
            return View("Comments/List");
        }

        [HttpPost]
        public IActionResult List(CommentSearchInput searchInput)
        {
            if(searchInput.WeightCategory == "0")
            {
                searchInput.WeightCategory = null;
            }
            return Ok(_commentService.SearchComments(searchInput).Select(p=>new CommentModel()
            {
                Description = p.Description,
                Rate = p.Rate,
                ServiceId = p.ServiceId,
                ServiceName = p.ServiceName
            }));
        }

        [HttpPost]
        public IActionResult Post(CommentModel model)
        {
            var result = _commentService.Insert(model, out string msg);
            if (result)
            {
                return Ok();
            }
            return Ok(msg);
        }
    }
}
