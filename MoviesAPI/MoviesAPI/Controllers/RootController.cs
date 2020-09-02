using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.DTOs;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class RootController : ControllerBase
    {
        [HttpGet(Name = "getRoot")]
        public ActionResult<IEnumerable<LinkDTO>> Get()
        {
            List<LinkDTO> links = new List<LinkDTO>();

            links.Add(new LinkDTO(href: Url.Link("getRoot", new { }), rel: "self", method: "GET"));
            links.Add(new LinkDTO(href: Url.Link("Login", new { }), rel: "login", method: "POST"));
            links.Add(new LinkDTO(href: Url.Link("GetGenres", new { }), rel: "get-genres", method: "GET"));
            links.Add(new LinkDTO(href: Url.Link("GetMovies", new { }), rel: "get-movies", method: "GET"));
            links.Add(new LinkDTO(href: Url.Link("GetPeople", new { }), rel: "get-people", method: "GET"));
            return links;
        }
    }

}