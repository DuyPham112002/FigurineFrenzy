using FigurineFrenzeyViewModel.Image;
using FigurineFrenzeyViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.Auction
{
    public class GetInfroAuctionViewModel
    {
        public string AuctionId { get; set; }
        public double? StartPrice { get; set; } 
        public double? CurrentPrice { get; set; }
        public double? FinalizePrice { get; set; }
        public string OwnerId { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Status { get; set; }
        public string? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string OwnerName { get; set; }
        public double? StepPrice { get; set; }   
        public DateTime? CreatAt { get; set; }

        public List<GetinfoItemViewModel> Items { get; set; }
        public List<InfoImageViewModel> Images { get; set; }

    }
}
