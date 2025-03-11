using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.ImageSet
{
    public interface IImageSetRepository : IRepository<DBAccess.Entites.ImageSet>
    {
        void Update(DBAccess.Entites.ImageSet imageSet);
    }
}
