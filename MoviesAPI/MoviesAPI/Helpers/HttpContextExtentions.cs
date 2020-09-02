using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;

namespace MoviesAPI.Helpers
{
    public static class HttpContextExtentions
    {
        public async static Task InsertPaginationParametersInResponse<T>(this HttpContext httpContext,
        IQueryable<T> queryable, int recordsPerPage, int pageNumber)
        {
            if (httpContext == null)
            {
                throw new ArgumentException(nameof(httpContext));
            }

            double entityCount = await queryable.CountAsync();
            double pageCount = Math.Ceiling(entityCount / recordsPerPage);

            httpContext.Response.Headers.Add("page-count", pageCount.ToString());

            if (pageCount > pageNumber)
            {
                var currentUrl = httpContext.Request.GetEncodedUrl();
                var uriBuilder = new UriBuilder(currentUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["page"] = (pageNumber + 1).ToString();
                query["recordsPerPage"] = recordsPerPage.ToString();
                uriBuilder.Query = query.ToString();
                currentUrl = uriBuilder.ToString();

                httpContext.Response.Headers.Add("next-query", currentUrl);
            }

        }
    }
}