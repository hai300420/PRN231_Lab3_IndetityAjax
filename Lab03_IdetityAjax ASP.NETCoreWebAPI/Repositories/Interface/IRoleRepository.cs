using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IRoleRepository
    {
        List<Role> GetRoles();
        Role GetRoleById(int id);
        void SaveRole(Role role);
        void UpdateRole(Role role);
        void DeleteRole(int id);
    }
}
