using Microsoft.AspNetCore.Http;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.plugin.Orders.ExtendedShipment.Domain;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Nop.plugin.Orders.ExtendedShipment.Models;

namespace Nop.plugin.Orders.ExtendedShipment.Infrastructure
{
    public class UrlRewriteMiddleware
    {
        private readonly RequestDelegate _next;

        public UrlRewriteMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, IStaticCacheManager cacheManager)
        {
            string CacheKey = "REWRITE_PATHS_KEY";
            if (!cacheManager.IsSet(CacheKey))
            {
                var _repo = EngineContext.Current.Resolve<IRepository<Tbl_RewritePaths>>();
                var _rewritePaths = _repo.TableNoTracking.Where(p => p.IsActive)
                    .Select(n => new RewriteModel
                    {
                        Id = n.Id,
                        IsActive = n.IsActive,
                        NewPath = n.NewPath.ToLower().Replace("http://", "").Replace("https://", "").Replace("www.", ""),
                        OldPath = n.OldPath.ToLower().Replace("http://", "").Replace("https://", "").Replace("www.", "")
                    }).ToList();
                cacheManager.Set(CacheKey, _rewritePaths, 1440);
            }
            var Url = httpContext.Request.GetDisplayUrl().ToLower();
            var rewirtePaths = cacheManager.Get<List<RewriteModel>>(CacheKey);
            if (rewirtePaths != null && rewirtePaths.Any())
            {
                Url = Url.ToLower().Replace("http://", "").Replace("https://", "").Replace("www.", "");
                var path = rewirtePaths.Find(x => x.OldPath == (Url));
                if (path != null)
                {
                    httpContext.Response.OnStarting((state) =>
                    {
                        httpContext.Response.Clear();
                        httpContext.Response.StatusCode = 301;
                        httpContext.Response.Headers.Add("Location", "http://" + path.NewPath);
                        return Task.FromResult(0);
                    }, null);
                }
                else
                    await _next(httpContext);
            }
        }

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RewriteMiddlewareExtensions
    {
        public static IApplicationBuilder UseRewriteMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UrlRewriteMiddleware>();
        }
    }
}
