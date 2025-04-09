using FigurineFrenzeyViewModel.Image;
using FigurineFrenzeyViewModel.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FigurineFrenzeyViewModel.Auction
{
    public class GetAllInfoAuction
    {
        public GetinfoItemViewModel Auction { get; set; }
        public List<GetinfoItemViewModel> ListItem { get; set; }
        public List<ImageViewModel> ListImage { get; set; }
    }
}
