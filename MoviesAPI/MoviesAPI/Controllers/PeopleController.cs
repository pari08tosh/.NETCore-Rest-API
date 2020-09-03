using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/people")]
    public class PeopleController : CustomBaseController
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<PeopleController> _logger;
        private readonly IFileStorageService _fileStorageService;

        public PeopleController(ApplicationDBContext context, IMapper mapper, ILogger<PeopleController> logger, IFileStorageService fileStorageService) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        [HttpGet(Name = "GetPeople")]
        public async Task<ActionResult<List<PersonDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            return await Get<Person, PersonDTO>(paginationDTO: paginationDTO);
        }

        [HttpGet("{id:int}", Name = "getPerson")]
        public async Task<ActionResult<PersonDTO>> Get(int id)
        {
            var person = await _context.Person.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return _mapper.Map<PersonDTO>(person);
        }


        [HttpPost]
        public async Task<ActionResult<PersonDTO>> Post([FromForm] CreatePersonDTO createPersonDTO)
        {

            var person = _mapper.Map<Person>(createPersonDTO);

            if (createPersonDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createPersonDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createPersonDTO.Picture.FileName);
                    person.Picture = await _fileStorageService.SaveFile(content, extension, "people", createPersonDTO.Picture.ContentType);
                }
            }

            await _context.AddAsync(person);
            await _context.SaveChangesAsync();

            var personDTO = _mapper.Map<PersonDTO>(person);

            return new CreatedAtRouteResult("getPerson", new { id = personDTO.Id }, personDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] CreatePersonDTO createPersonDTO)
        {
            var personDBEntry = await _context.Person.FirstOrDefaultAsync(x => x.Id == id);

            if (personDBEntry == null)
            {
                return NotFound();
            }

            personDBEntry = _mapper.Map(createPersonDTO, personDBEntry);

            if (createPersonDTO.Picture != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await createPersonDTO.Picture.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(createPersonDTO.Picture.FileName);
                    personDBEntry.Picture = await _fileStorageService.EditFile(content, extension, "people", personDBEntry.Picture, createPersonDTO.Picture.ContentType);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Person>(id);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PatchPersonDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var personDBEntry = await _context.Person.FirstOrDefaultAsync(x => x.Id == id);

            if (personDBEntry == null)
            {
                return NotFound();
            }

            var personDTO = _mapper.Map<PatchPersonDTO>(personDBEntry);

            patchDocument.ApplyTo(personDTO, ModelState);

            var isValid = TryValidateModel(personDTO);

            if (!isValid)
            {
                return BadRequest(ModelState);
            }

            personDBEntry = _mapper.Map(personDTO, personDBEntry);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }



}