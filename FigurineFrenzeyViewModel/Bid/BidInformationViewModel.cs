using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.Bid
{
    public class BidInformationViewModel
    {
        public string FullName { get; set; }
        public string AuctionId { get; set; }   
        public string ImageUrl { get; set; }
        public double? Amount { get; set; }
    }
}
