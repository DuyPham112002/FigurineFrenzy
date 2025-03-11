using DBAccess.Entites;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Admin
{
    public class AdminRepository: Repository<DBAccess.Entites.Admin>, IAdminRepository
    {
        private readonly FigurineFrenzyContext _context;

        public AdminRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Entites.Admin admin)
        {
            _context.Admins.Update(admin);
        }
    }
}
