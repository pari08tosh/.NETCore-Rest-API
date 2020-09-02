using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MoviesAPI.Helpers
{
    public class HATEOASAttribute : ResultFilterAttribute
    {
        protected bool ShouldIncludeHATEOAS(ResultExecutingContext context)
        {
            var result = context.Result as ObjectResult;
            if (!IsSuccessfulResponse(result))
            {
                return false;
            }

            if (context.HttpContext.Request.Headers.ContainsKey("Include-HATEOAS"))
            {
                if (context.HttpContext.Request.Headers["Include-HATEOAS"][0] != "true")
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;

        }

        private bool IsSuccessfulResponse(ObjectResult result)
        {
            if (result == null || result.Value == null || !IsSuccessStatusCode(result.StatusCode))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsSuccessStatusCode(int? statusCode)
        {
            if (statusCode != null)
            {
                if (statusCode >= 200 && statusCode <= 299)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}