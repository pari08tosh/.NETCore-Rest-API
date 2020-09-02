using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class CreateGenreDTO
    {
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
    }
}