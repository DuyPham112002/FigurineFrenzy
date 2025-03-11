using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Admin
{
    public interface IAdminRepository: IRepository<DBAccess.Entites.Admin>
    {
        void Update(DBAccess.Entites.Admin admin);
    }
}
