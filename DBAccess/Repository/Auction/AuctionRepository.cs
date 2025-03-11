using DBAccess.Entites;
using DBAccess.Repository.Base;
using DBAccess.Repository.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DBAccess.Repository.Auction
{
    class AuctionRepository : Repository<DBAccess.Entites.Auction>, IAuctionRepository
    {

        private readonly FigurineFrenzyContext _context;

        public AuctionRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Entites.Auction auc)
        {
            _context.Auctions.Update(auc);
        }
    }
}
