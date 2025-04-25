using FigurineFrenzeyViewModel.Bid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.Auction
{
    public class JoinAuctionGroup
    {
        public string AccountId { get; set; } 
        public List<CreateBidViewModel> Bids { get; set; }
    }
}
