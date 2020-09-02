using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using MoviesAPI.DTOs;
using MoviesAPI.Services;

namespace MoviesAPI.Helpers
{
    public class GenreHATEOASAttribute : HATEOASAttribute
    {
        private readonly LinksGeneratorService _linksGeneratorService;

        public GenreHATEOASAttribute(LinksGeneratorService linksGeneratorService)
        {
            _linksGeneratorService = linksGeneratorService;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (!ShouldIncludeHATEOAS(context))
            {
                await next();
                return;
            }

            _linksGeneratorService.Generate<GenreDTO>(context, next);
            await next();
        }
    }

}