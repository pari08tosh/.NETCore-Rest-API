using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Helpers;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class CreateMovieDTO : PatchMovieDTO
    {
        [FileSizeValidator(maxFileSizeInMB: 10)]
        [ContentTypeValidator(ContentTypeGroup.Image)]
        public IFormFile Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenresIds { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<CreateActorDTO>>))]
        public List<CreateActorDTO> Actors { get; set; }


    }
}