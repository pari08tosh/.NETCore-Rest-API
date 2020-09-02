using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class EditRoleDTO
    {
        [Required]
        public List<string> RoleNames { get; set; }

        [Required]
        public string UserId { get; set; }
    }
}