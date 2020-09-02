using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genre, GenreDTO>().ReverseMap();
            CreateMap<CreateGenreDTO, Genre>();
            CreateMap<Person, PersonDTO>().ReverseMap();

            CreateMap<CreatePersonDTO, Person>()
                .ForMember(x => x.Picture, options => options.Ignore());

            CreateMap<Person, PatchPersonDTO>().ReverseMap();

            CreateMap<Movie, MovieDTO>().ReverseMap();

            CreateMap<CreateMovieDTO, Movie>()
               .ForMember(x => x.Poster, options => options.Ignore())
               .ForMember(x => x.MoviesGenres, options => options.MapFrom(MapMoviesGenres))
               .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));

            CreateMap<Movie, MovieDetailsDTO>()
               .ForMember(x => x.Poster, options => options.Ignore())
               .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
               .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));


            CreateMap<Movie, PatchMovieDTO>().ReverseMap();

            CreateMap<MoviesFilterDTO, PaginationDTO>();

            CreateMap<IdentityUser, UserDTO>();
        }

        private List<GenreDTO> MapMoviesGenres(Movie movie, MovieDetailsDTO movieDetails)
        {
            var result = new List<GenreDTO>();

            foreach (var movieGenre in movie.MoviesGenres)
            {
                result.Add(new GenreDTO() { Id = movieGenre.GenreId, Name = movieGenre.Genre.Name });
            }
            return result;
        }

        private List<ActorDTO> MapMoviesActors(Movie movie, MovieDetailsDTO movieDetails)
        {
            var result = new List<ActorDTO>();

            foreach (var actor in movie.MoviesActors)
            {
                result.Add(new ActorDTO { PersonId = actor.PersonId, PersonName = actor.Person.Name, Character = actor.CharacterName });
            }
            return result;
        }

        private List<MoviesGenres> MapMoviesGenres(CreateMovieDTO createMovieDTO, Movie movie)
        {
            var result = new List<MoviesGenres>();

            foreach (var id in createMovieDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }

        private List<MoviesActors> MapMoviesActors(CreateMovieDTO createMovieDTO, Movie movie)
        {
            var result = new List<MoviesActors>();

            foreach (var actor in createMovieDTO.Actors)
            {
                result.Add(new MoviesActors { PersonId = actor.PersonId, CharacterName = actor.Character });
            }
            return result;
        }

    }
}