using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Token
{
    public interface ITokenRespository : IRepository<DBAccess.Entites.Token>
    {
        void Update(DBAccess.Entites.Token token);
    }
}
