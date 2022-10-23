using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace Nop.plugin.Orders.ExtendedShipment.Services
{
    class RewriteRule : Microsoft.AspNetCore.Rewrite.IRule
    {
        private readonly string OldPath;
        private readonly string NewPath;
        public RewriteRule(string OldPath, string NewPath)
        {
            this.OldPath = OldPath;
            this.NewPath = NewPath;
        }
        public void ApplyRule(RewriteContext context)
        {
            //var request = context.HttpContext.Request;
            //var host = request.Host;
            //if (request.Path.StartsWithSegments(new PathString(this.NewPath)))
            //{
            //    return;
            //}
            //if (request.Path.Value.Contains(OldPath))
            //{
            //    var newLocation = $"{this.NewPath}{request.QueryString}";

            //    var response = context.HttpContext.Response;
            //    response.StatusCode = StatusCodes.Status301MovedPermanently;
            //    context.Result = RuleResult.EndResponse;
            //    response.Headers[HeaderNames.Location] = newLocation;
            //}
        }
    }
}
