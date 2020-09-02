using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class TokenDTO
    {
        public string Token { get; set; }

        public DateTime Expirey { get; set; }
    }
}