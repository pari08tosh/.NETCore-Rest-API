using System;

namespace MoviesAPI.DTOs
{
    public class MoviesFilterDTO : PaginationDTO
    {
        public string Title { get; set; }
        public bool InTheaters { get; set; }
        public bool UpcomingReleases { get; set; }
        public int GernreId { get; set; }
        public string OrderBy { get; set; }
    }

}