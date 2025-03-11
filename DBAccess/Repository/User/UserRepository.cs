using DBAccess.Entites;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.User
{
    public class UserRepository : Repository<DBAccess.Entites.User>, IUserRepository
    {
        private readonly FigurineFrenzyContext _context;
        public UserRepository(FigurineFrenzyContext context) : base(context) 
        {
            _context = context;
        }

        public void Update(Entites.User user)
        {
            _context.Users.Update(user);    
        }
    }
}
