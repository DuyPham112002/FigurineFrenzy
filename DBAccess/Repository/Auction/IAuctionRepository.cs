using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Auction
{
    public interface IAuctionRepository : IRepository<DBAccess.Entites.Auction>
    {
        void Update(DBAccess.Entites.Auction auc);
    }
}
