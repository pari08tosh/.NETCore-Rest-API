using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Filters;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/genres")]
    public class GenresController : CustomBaseController
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<GenresController> _logger;

        public GenresController(ILogger<GenresController> logger, ApplicationDBContext context, IMapper mapper) : base(context, mapper)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = "GetGenres")]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        public async Task<List<GenreDTO>> Get()
        {
            return await Get<Genre, GenreDTO>();
        }

        [HttpGet("{id:int}", Name = "GetGenre")]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            return await Get<Genre, GenreDTO>(id);
        }

        [HttpPost(Name = "CreateGenre")]
        public async Task<ActionResult> Post([FromBody] CreateGenreDTO createGenreDTO)
        {
            return await Post<CreateGenreDTO, Genre, GenreDTO>(createGenreDTO);
        }

        [HttpPut("{id:Int}", Name = "ReplaceGenre")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateGenreDTO createGenreDTO)
        {
            return await Put<CreateGenreDTO, Genre, GenreDTO>(id, createGenreDTO);
        }

        [HttpDelete("{id:Int}", Name = "DeleteGenre")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Genre>(id);
        }
    }
}