using DBAccess.Entites;
using DBAccess.Repository.Auction;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Bid
{
    public class BidRepository : Repository<DBAccess.Entites.Bid>, IBidRepository
    {
        private readonly FigurineFrenzyContext _context;

        public BidRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Entites.Bid bid)
        {
            _context.Bids.Update(bid);
        }
    }
}
