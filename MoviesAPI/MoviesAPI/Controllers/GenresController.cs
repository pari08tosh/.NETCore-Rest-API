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
    public class GenresController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<GenresController> _logger;

        public GenresController(ILogger<GenresController> logger, ApplicationDBContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = "GetGenres")]
        //[ResponseCache(Duration = 60)]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[ServiceFilter(typeof(MyActionFilter))]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        public async Task<List<GenreDTO>> Get()
        {
            // Arrange
            var genres = await _context.Genres.AsNoTracking().ToListAsync();
            var generesDTOs = _mapper.Map<List<GenreDTO>>(genres);

            return generesDTOs;
        }

        [HttpGet("{id:int}", Name = "GetGenre")]
        [ServiceFilter(typeof(GenreHATEOASAttribute))]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            //Arrange
            var genre = await _context.Genres.FindAsync(id);

            if (genre == null)
            {
                return NotFound();
            }
            var genereDTO = _mapper.Map<GenreDTO>(genre);

            return genereDTO;
        }

        [HttpPost(Name = "CreateGenre")]
        public async Task<ActionResult> Post([FromBody] CreateGenreDTO createGenreDTO)
        {
            var genre = _mapper.Map<Genre>(createGenreDTO);

            await _context.AddAsync(genre);
            await _context.SaveChangesAsync();

            var genreDTO = _mapper.Map<GenreDTO>(genre);

            return new CreatedAtRouteResult("getGenre", new { id = genreDTO.Id }, genreDTO);
        }

        [HttpPut("{id:Int}", Name = "ReplaceGenre")]
        public async Task<ActionResult> Put(int id, [FromBody] CreateGenreDTO createGenreDTO)
        {
            var genre = _mapper.Map<Genre>(createGenreDTO);
            genre.Id = id;

            var exists = await _context.Genres.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }

            _context.Entry(genre).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:Int}", Name = "DeleteGenre")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Genres.AnyAsync(x => x.Id == id);
            if (!exists)
            {
                return NotFound();
            }
            _context.Remove(new Genre() { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}