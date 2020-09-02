using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class PatchPersonDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}