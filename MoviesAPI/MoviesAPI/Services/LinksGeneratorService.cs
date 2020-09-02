using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using MoviesAPI.DTOs;

namespace MoviesAPI.Services
{
    public class LinksGeneratorService
    {
        private readonly IUrlHelperFactory _urlHelperFacory;
        private readonly IActionContextAccessor _actionContextAccessor;

        public LinksGeneratorService(IUrlHelperFactory urlHelperFacory, IActionContextAccessor actionContextAccessor)
        {
            _urlHelperFacory = urlHelperFacory;
            _actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper GetUrlHelper()
        {
            return _urlHelperFacory.GetUrlHelper(_actionContextAccessor.ActionContext);
        }

        public void Generate<T>(ResultExecutingContext context, ResultExecutionDelegate next) where T : class, IGenerateHATEOASLinks, new()
        {
            var urlHelper = GetUrlHelper();
            var result = context.Result as ObjectResult;
            var model = result.Value as T;
            if (model == null)
            {
                var modelList = result.Value as List<T> ?? throw new ArgumentException("Invalid Reponse Type for HATEOAS link generation", nameof(T));
                var resourceList = modelList.Select(dto => dto.GenerateLinks(dto, urlHelper)).ToList();
                var blankObject = new T();
                result.Value = blankObject.GenerateLinksForCollection(resourceList, urlHelper);
            }
            else
            {
                result.Value = model.GenerateLinks(model, urlHelper);
            }
        }
    }

}