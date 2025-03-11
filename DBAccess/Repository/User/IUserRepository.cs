using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.User
{
    public interface IUserRepository : IRepository<DBAccess.Entites.User>
    {
        void Update(DBAccess.Entites.User user);
    }
}
