using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class PatchMovieDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool InTheaters { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}