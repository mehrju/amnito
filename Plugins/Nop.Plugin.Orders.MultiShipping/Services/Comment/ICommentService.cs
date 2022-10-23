using Nop.Plugin.Orders.MultiShipping.Domain;
using Nop.Plugin.Orders.MultiShipping.Models;
using Nop.Plugin.Orders.MultiShipping.Models.Comment;
using System.Collections.Generic;

namespace Nop.Plugin.Orders.MultiShipping.Services.Comment
{
    public interface ICommentService
    {
        bool Insert(CommentModel commentModel, out string msg);
        List<CommentSearchOutput> SearchComments(CommentSearchInput searchInput);
    }
}