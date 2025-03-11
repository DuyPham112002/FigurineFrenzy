using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Account
{
    public interface IAccountRepository : IRepository<DBAccess.Entites.Account>
    {
        void Update(DBAccess.Entites.Account account);

    }
}
