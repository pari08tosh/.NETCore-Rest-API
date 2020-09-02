using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.DTOs
{
    public class GenreDTO : IGenerateHATEOASLinks
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ResourceCollectionDTO<GenreDTO> GenerateLinks<GenreDTO>(GenreDTO dto, IUrlHelper urlHelper)
        {
            var resourceCollectionDTO = new ResourceCollectionDTO<GenreDTO>(dto);

            resourceCollectionDTO.Links.Add(new LinkDTO(urlHelper.Link("ReplaceGenre", new { id = this.Id }), "replace-genre", "PUT"));
            resourceCollectionDTO.Links.Add(new LinkDTO(urlHelper.Link("DeleteGenre", new { id = this.Id }), "delete-genre", "DELETE"));
            resourceCollectionDTO.Links.Add(new LinkDTO(urlHelper.Link("GetGenre", new { id = this.Id }), "get-genre", "GET"));

            return resourceCollectionDTO;

        }

        public ResourceCollectionDTO<List<ResourceCollectionDTO<GenreDTO>>> GenerateLinksForCollection<GenreDTO>(List<ResourceCollectionDTO<GenreDTO>> dtos, IUrlHelper urlHelper)
        {
            var resourceCollectionDTO = new ResourceCollectionDTO<List<ResourceCollectionDTO<GenreDTO>>>(dtos);
            resourceCollectionDTO.Links.Add(new LinkDTO(urlHelper.Link("GetGenres", new { }), "get-genres", "GET"));
            resourceCollectionDTO.Links.Add(new LinkDTO(urlHelper.Link("CreateGenre", new { }), "create-genre", "POST"));
            return resourceCollectionDTO;
        }
    }
}