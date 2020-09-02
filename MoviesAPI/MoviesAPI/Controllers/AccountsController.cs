using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI.DTOs;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDBContext _context;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration configuration, ApplicationDBContext context, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _mapper = mapper;
        }


        [HttpPost("create", Name = "CreateUser")]
        public async Task<ActionResult<TokenDTO>> CreateUser([FromBody] UserCredentialsDTO userCredentialsDTO)
        {
            var user = new IdentityUser { UserName = userCredentialsDTO.EmailAddress, Email = userCredentialsDTO.EmailAddress };
            var result = await _userManager.CreateAsync(user, userCredentialsDTO.Password);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentialsDTO);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<TokenDTO>> Login([FromBody] UserCredentialsDTO userCredentialsDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(
                userCredentialsDTO.EmailAddress,
                userCredentialsDTO.Password,
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await BuildToken(userCredentialsDTO);
            }
            else
            {
                return BadRequest("Invalid Login attempt");
            }
        }

        private async Task<TokenDTO> BuildToken(UserCredentialsDTO userCredentialsDTO)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, userCredentialsDTO.EmailAddress),
                new Claim(ClaimTypes.Email, userCredentialsDTO.EmailAddress)
            };

            var user = await _userManager.FindByEmailAsync(userCredentialsDTO.EmailAddress);
            var userRoles = (List<string>)await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["jwt:secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirey = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expirey,
                signingCredentials: creds
            );

            return new TokenDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expirey = expirey
            };
        }

        [HttpGet("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<UserDTO>>> GetUsers([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = _context.Users.AsQueryable();
            queryable = queryable.OrderBy(x => x.Email);
            await HttpContext.InsertPaginationParametersInResponse(queryable, paginationDTO.RecordsPerPage, paginationDTO.Page);
            var users = await queryable.Paginate(paginationDTO.RecordsPerPage, paginationDTO.Page).ToListAsync();
            return _mapper.Map<List<UserDTO>>(users);
        }

        [HttpGet("users/{id}/roles", Name = "getUserRoles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<string>>> GetUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ModelState.TryAddModelError("UserId", "Not Found");
                return NotFound(ModelState);
            }

            var roles = (List<string>)await _userManager.GetRolesAsync(user);

            return roles;
        }


        [HttpGet("roles")]
        public async Task<ActionResult<List<string>>> GetRoles()
        {
            return await _context.Roles.Select(x => x.Name).ToListAsync();
        }

        [HttpPost("assignRoles")]
        public async Task<ActionResult> AssignRole([FromBody] EditRoleDTO editRoleDTO)
        {
            var user = await _userManager.FindByIdAsync(editRoleDTO.UserId);
            await _userManager.AddToRolesAsync(user, editRoleDTO.RoleNames);
            return CreatedAtRoute("getUserRoles", new { id = editRoleDTO.UserId }, editRoleDTO);
        }

    }
}