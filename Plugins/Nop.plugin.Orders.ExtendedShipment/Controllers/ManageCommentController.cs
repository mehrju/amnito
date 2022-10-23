using Microsoft.AspNetCore.Mvc;
using Nop.Data;
using Nop.plugin.Orders.ExtendedShipment.Models.Comment;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.plugin.Orders.ExtendedShipment.Controllers
{
    [AdminAntiForgery(true)]
    public class ManageCommentController : BaseAdminController
    {
        private readonly IDbContext _dbContext;

        public ManageCommentController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual IActionResult List()
        {
            return View("/Plugins/Orders.ExtendedShipment/Views/Comment/List.cshtml");
        }

        public virtual IActionResult GetComments()
        {
            var comments = _dbContext.SqlQuery<CommentModel>("SELECT TOP 2000 tc.Id,tc.TrackingCode,tc.Description,tc.Rate,tc.IsPublished FROM dbo.Tbl_Comment AS tc ORDER BY tc.IsPublished ASC,tc.CreateDate DESC").ToList();
            var gridModel = new DataSourceResult
            {
                Data = comments,
                Total = comments.Count
            };
            return Ok(gridModel);
        }

        public IActionResult SetPublish(int id, bool isPublish)
        {
            var result = _dbContext.ExecuteSqlCommand($"UPDATE dbo.Tbl_Comment SET IsPublished = {(isPublish ? 1 : 0)} WHERE Id = {id}");
            if(result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
