using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.Item
{
    public class CreateItemViewModel
    {
        public string NameOfProduct { get; set; }
        public string Description { get; set; }
        public List<IFormFile> Files { get; set; }
        public string AuctionId { get; set; }
    }
}
