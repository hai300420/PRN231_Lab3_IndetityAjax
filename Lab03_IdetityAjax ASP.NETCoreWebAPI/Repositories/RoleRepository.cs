using BusinessObjects;
using DataAccess.DAO;
using Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public List<Role> GetRoles() => RoleDAO.GetRoles();

        public Role GetRoleById(int id) => RoleDAO.GetRoleById(id);

        public void SaveRole(Role role) => RoleDAO.SaveRole(role);

        public void UpdateRole(Role role) => RoleDAO.UpdateRole(role);

        public void DeleteRole(int id) => RoleDAO.DeleteRole(id);
    }
}
