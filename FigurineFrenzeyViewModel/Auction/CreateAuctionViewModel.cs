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
        public double StartPrice { get; set; }
        [AllowNull]
        public double FinalizePrice { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public bool CustomStepPrice { get; set; } //This field used for check if user want to use custom step price or not 0: Default, 1: Custom
        public double? StepPrice { get; set; } 
    }
}
