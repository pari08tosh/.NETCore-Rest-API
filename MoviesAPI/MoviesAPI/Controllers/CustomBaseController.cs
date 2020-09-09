using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public CustomBaseController(ApplicationDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>(IQueryable<TEntity> queryable = null, PaginationDTO paginationDTO = null) where TEntity : class
        {
            if (queryable == null)
            {
                queryable = _context.Set<TEntity>().AsQueryable();
            }

            List<TEntity> entities = null;

            if (paginationDTO != null)
            {
                await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage, paginationDTO.Page);

                entities = await queryable.Paginate(paginationDTO.RecordsPerPage, paginationDTO.Page).ToListAsync();
            }
            else
            {
                entities = await queryable.AsNoTracking().ToListAsync();
            }

            var dtos = _mapper.Map<List<TDTO>>(entities);
            return dtos;
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return _mapper.Map<TDTO>(entity);
        }

        public async Task<ActionResult> Post<TCreateDTO, TEntity, TDTO>(TCreateDTO createDTO)
        {
            var entity = _mapper.Map<TEntity>(createDTO);

            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();

            var dto = _mapper.Map<TDTO>(entity);

            return new CreatedAtRouteResult("getGenre", new { id = typeof(TDTO).GetProperty("Id").GetValue(dto) }, dto);
        }

        public async Task<ActionResult> Put<TCreateDTO, TEntity, TDTO>(int id, TCreateDTO createDTO) where TEntity : class
        {
            var entityDB = await _context.Set<TEntity>().FindAsync(id);

            if (entityDB == null)
            {
                return NotFound();
            }

            entityDB = _mapper.Map(createDTO, entityDB);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<ActionResult> Delete<TEntity>(int id) where TEntity : class, new()
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }



    }
}