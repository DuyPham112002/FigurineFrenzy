using DBAccess.Entites;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Role
{
    public class RoleRepository : Repository<DBAccess.Entites.Role>, IRoleRepository
    {
        private readonly FigurineFrenzyContext _context;

        public RoleRepository(FigurineFrenzyContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Entites.Role role)
        {
            _context.Roles.Update(role);
        }
    }
}
