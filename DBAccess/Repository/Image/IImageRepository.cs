using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Image
{
    public interface IImageRepository : IRepository<DBAccess.Entites.Image>
    {
        void Update(DBAccess.Entites.Image image);
    }
}
