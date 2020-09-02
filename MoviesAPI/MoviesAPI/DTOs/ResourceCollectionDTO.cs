using System.Collections.Generic;

namespace MoviesAPI.DTOs
{
    public class ResourceCollectionDTO<T>
    {
        public T Data { get; set; }
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();

        public ResourceCollectionDTO(T data)
        {
            Data = data;
        }
    }
}