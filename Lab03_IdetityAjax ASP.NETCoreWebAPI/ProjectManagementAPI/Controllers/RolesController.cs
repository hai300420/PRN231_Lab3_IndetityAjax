using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interface;
using Repositories;
using DataAccess.DTO.RoleDTOs;

namespace ProjectManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleRepository repository = new RoleRepository();

        [HttpGet]
        public ActionResult<IEnumerable<Role>> GetRoles()
        {
            var roles = repository.GetRoles();
            var result = roles.Select(r => new RoleDTO
            {
                RoleId = r.RoleId,
                RoleName = r.RoleName
            });

            return Ok(result);
        }

        [HttpPost]
        public IActionResult PostRole([FromBody] RoleCreateDTO dto)
        {
            var role = new Role
            {
                RoleName = dto.RoleName
            };

            repository.SaveRole(role);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Role created successfully." 
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateRole(int id, [FromBody] RoleUpdateDTO dto)
        {
            var role = repository.GetRoleById(id);
            if (role == null) return NotFound();

            role.RoleName = dto.RoleName;
            repository.UpdateRole(role);

            return Ok(new 
            { 
                Status = "Success", 
                Message = "Role updated successfully." 
            });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteRole(int id)
        {
            var role = repository.GetRoleById(id);
            if (role == null) return NotFound();

            repository.DeleteRole(id);
            return Ok(new 
            { 
                Status = "Success", 
                Message = "Role deleted successfully." 
            });
        }
    }
}
