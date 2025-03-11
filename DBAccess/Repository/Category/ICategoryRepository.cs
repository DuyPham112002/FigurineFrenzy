using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Category
{
    public interface ICategoryRepository: IRepository<DBAccess.Entites.Category>
    {
        void Update(DBAccess.Entites.Category cate);
    }
}
