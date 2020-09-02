using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.DTOs
{
    public interface IGenerateHATEOASLinks
    {
        ResourceCollectionDTO<T> GenerateLinks<T>(T dto, IUrlHelper urlHelper);
        ResourceCollectionDTO<List<ResourceCollectionDTO<T>>> GenerateLinksForCollection<T>(List<ResourceCollectionDTO<T>> DTOs, IUrlHelper urlHelper);
    }
}