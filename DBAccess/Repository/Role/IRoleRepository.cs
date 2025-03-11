using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Role
{
    public interface IRoleRepository : IRepository<DBAccess.Entites.Role>
    {
        void Update(DBAccess.Entites.Role role);
    }
}
