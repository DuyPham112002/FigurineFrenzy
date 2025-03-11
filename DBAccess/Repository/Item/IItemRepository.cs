using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Item
{
    public interface IItemRepository : IRepository<DBAccess.Entites.Item>
    {
        void Update(DBAccess.Entites.Item item);
    }
}
