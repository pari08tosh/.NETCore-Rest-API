using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class CreatePersonDTO : PatchPersonDTO
    {
        [FileSizeValidator(maxFileSizeInMB: 10)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Picture { get; set; }
    }
}