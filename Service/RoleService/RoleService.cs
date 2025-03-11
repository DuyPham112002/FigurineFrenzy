using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RoleService
{
    public interface IRoleService
    {
        Task<RoleViewModel> GetRoleIdByNameAsync(string name);
    }
    public class RoleService: IRoleService
    {
        private readonly IUnitOfWork _uow;
        public RoleService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<RoleViewModel> GetRoleIdByNameAsync(string name)
        {
            var roleName = await _uow.Role.GetFirstOrDefaultAsync(a => a.Name == name);
            if (roleName != null)
            {
                return new RoleViewModel { Name = roleName.Name, Id = roleName.RoleId};
            }
            else return null;
        }

        
    }
}
