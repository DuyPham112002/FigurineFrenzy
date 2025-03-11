using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.Auction
{
    public class CreateAuctionViewModel
    {
        [Required]
        public double StarPrice { get; set; }
        [Required]
        public double CurrentPrice { get; set; }
        [AllowNull]
        public double FinalizePrice { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        
    }
}
