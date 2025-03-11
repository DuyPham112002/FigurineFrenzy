using DBAccess.Entites;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Token
{
    public class TokenRepository : Repository<DBAccess.Entites.Token>, ITokenRespository
    {
        private readonly FigurineFrenzyContext _context;

        public TokenRepository(FigurineFrenzyContext context): base(context)
        {
            _context = context;
        }
        public void Update(Entites.Token token)
        {
            _context.Tokens.Update(token);
        }
    }
}
