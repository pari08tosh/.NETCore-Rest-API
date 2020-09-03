using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/movies")]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public MoviesController(ApplicationDBContext context, IMapper mapper, IFileStorageService fileStorageService) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        /// <summary>
        /// Get all movies. Supports pagination and querying.
        /// </summary>
        /// <param name="moviesFilterDTO"></param>
        /// <returns></returns>
        [HttpGet(Name = "GetMovies")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult<List<MovieDTO>>> Get([FromQuery] MoviesFilterDTO moviesFilterDTO)
        {
            var queryable = _context.Movies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(moviesFilterDTO.Title))
            {
                queryable = queryable.Where(x => x.Title.Contains(moviesFilterDTO.Title));
            }

            if (moviesFilterDTO.InTheaters)
            {
                queryable = queryable.Where(x => x.InTheaters);
            }

            if (moviesFilterDTO.GernreId != 0)
            {
                queryable = queryable.Where(x => x.MoviesGenres.Select(y => y.GenreId)
                                     .Contains(moviesFilterDTO.GernreId));
            }

            if (moviesFilterDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                queryable = queryable.Where(x => x.ReleaseDate > today);
            }

            if (!string.IsNullOrWhiteSpace(moviesFilterDTO.OrderBy))
            {
                try
                {
                    queryable = queryable.OrderBy(moviesFilterDTO.OrderBy);
                }
                catch (ParseException e)
                {
                    ModelState.TryAddModelError(nameof(moviesFilterDTO.OrderBy), e.Message);
                    return BadRequest(ModelState);
                }
            }

            return await Get<Movie, MovieDTO>(
                queryable,
                new PaginationDTO()
                {
                    RecordsPerPage = moviesFilterDTO.RecordsPerPage,
                    Page = moviesFilterDTO.Page
                }
            );
        }

        [HttpGet("{id:int}", Name = "getMovie")]
        public async Task<ActionResult<MovieDetailsDTO>> Get(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Person)
                .Include(x => x.MoviesGenres).ThenInclude(x => x.Genre)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return _mapper.Map<MovieDetailsDTO>(movie);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Post([FromForm] CreateMovieDTO createMovieDTO)
        {
            var movie = _mapper.Map<Movie>(createMovieDTO);
            if (createMovieDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createMovieDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createMovieDTO.Poster.FileName);
                    movie.Poster = await _fileStorageService.SaveFile(content, extension, "movies", createMovieDTO.Poster.ContentType);
                }
            }

            AnnotateActorsOrder(movie);

            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();

            var movieDTO = _mapper.Map<MovieDTO>(movie);

            return CreatedAtRoute("getMovie", new { id = movieDTO.Id }, movieDTO);
        }

        private static void AnnotateActorsOrder(Movie movie)
        {
            if (movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] CreateMovieDTO createMovieDTO)
        {
            var movieDBEntry = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movieDBEntry == null)
            {
                return NotFound();
            }

            movieDBEntry = _mapper.Map(createMovieDTO, movieDBEntry);

            if (createMovieDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createMovieDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createMovieDTO.Poster.FileName);
                    movieDBEntry.Poster = await _fileStorageService.EditFile(content, extension, "movies", movieDBEntry.Poster, createMovieDTO.Poster.ContentType);
                }
            }

            await _context.Database.ExecuteSqlInterpolatedAsync($"delete from MoviesActors where MovieId = {movieDBEntry.Id}; delete from MoviesGenres where MovieId = {movieDBEntry.Id}");

            AnnotateActorsOrder(movieDBEntry);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _context.Movies.AnyAsync(x => x.Id == id);

            if (!exists)
            {
                return NotFound();
            }
            _context.Remove(new Movie { Id = id });
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PatchMovieDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var movieDBEntry = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

            if (movieDBEntry == null)
            {
                return NotFound();
            }

            var movieDTO = _mapper.Map<PatchMovieDTO>(movieDBEntry);

            patchDocument.ApplyTo(movieDTO, ModelState);

            var isValid = TryValidateModel(movieDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            movieDBEntry = _mapper.Map(movieDTO, movieDBEntry);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}