using DBAccess.Entites;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Account
{
    public class AccountRepository : Repository<DBAccess.Entites.Account>, IAccountRepository
    {
        private readonly FigurineFrenzyContext _context;
        
        public AccountRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Entites.Account account)
        {
            _context.Accounts.Update(account);
        }
    }
}
