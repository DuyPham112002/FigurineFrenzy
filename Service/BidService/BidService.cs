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
        Task<RESPONSECODE> CreateAsync(CreateBidViewModel createBidView, string accId);

        Task<Bid> GetAsync(string auctionId);
        Task<List<Bid>> GetAllAsync();  
        Task<List<string>> GetAllAsyncById(string accountId);
    }
    public class BidService : IBidService
    {
        private readonly IUnitOfWork _uow;
        public BidService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<RESPONSECODE> CreateAsync(CreateBidViewModel createBidView, string accId)
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

        public async Task<List<Bid>> GetAllAsync()
        {
            try
            {
                var getAll = await _uow.Bid.GetAllAsync();
                if(getAll !=null)
                {
                    return getAll.ToList();
                }
                else
                {
                    throw new Exception("No bid found");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving bidders: " + ex.Message);
            }
        }

        public async Task<List<string>> GetAllAsyncById(string accountId)
        {
            try
            {
                var isExistBid = (await _uow.Bid.GetAllAsync(b => b.Bidder == accountId)).Select(b => b.AuctionId).Distinct().ToList();
                if (isExistBid != null)
                {

                    return isExistBid;
                }
                else
                {
                    throw new Exception("No bid found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving bidders: " + ex.Message);
            }
        }
           

        public async Task<Bid> GetAsync(string auctionId)
        {
            try
            {
                var bidder = await _uow.Bid.GetFirstOrDefaultAsync(b => b.AuctionId == auctionId);
                if(bidder != null)
                {
                    return bidder;
                }
                else
                {
                   throw new Exception("Bidder not found");
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Error retrieving bidder: " + ex.Message);
            }
        }
    }
}
