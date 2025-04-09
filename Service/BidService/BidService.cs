using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Bid;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.BidService
{
    public interface IBidService
    {
        Task<RESPONSECODE> Create(CreateBidViewModel createBidView, string accId);
    }
    public class BidService : IBidService
    {
        private readonly IUnitOfWork _uow;
        public BidService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<RESPONSECODE> Create(CreateBidViewModel createBidView, string accId)
        {
            try
            {
               var verifyAuction = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == createBidView.AuctionId && a.OwnerId == accId);
                if (verifyAuction == null)
                {
                    var newBid = new Bid()
                    {
                        Id = Guid.NewGuid().ToString(),
                        AuctionId = createBidView.AuctionId,
                        Bidder = accId,
                        BidAmount = createBidView.BidAmount,
                        BidTime = DateTime.Now
                    };

                    await _uow.Bid.AddAsync(newBid);
                    await _uow.SaveAsync();

                    return RESPONSECODE.OK;
                }else return RESPONSECODE.INTERNALERROR;

            }
            catch
            {
                return RESPONSECODE.ERROR;
            }
        }
    }
}
