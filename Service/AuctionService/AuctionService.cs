using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Auction;
using FigurineFrenzy.Enum;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AuctionService
{
    public interface IAuctionService
    {
        public Task<Auction> CreateAsync(string accId, CreateAuctionViewModel auctionView, string cateId);
        public Task<RESPONSECODE> UpdateAsync(string accId, CreateAuctionViewModel auctionView, string Id, string categoryId);
        public Task<Auction> GetAsync(string auctionId, string accId);
        public Task<List<Auction>> GetAllAsync( string accId);
    }
    public class AuctionService: IAuctionService
    {
        private readonly IUnitOfWork _uow;
        public AuctionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Auction> CreateAsync(string Id, CreateAuctionViewModel auctionView, string categoryId)
        {
            try
            {
                Auction newAuction = new Auction
                {
                    AuctionId = Guid.NewGuid().ToString(),
                    StarPrice = auctionView.StarPrice,
                    CurrentPrice = auctionView.CurrentPrice,
                    FinalizePrice = auctionView.FinalizePrice,
                    StartTime = auctionView.StartTime,
                    EndTime = auctionView.EndTime,
                    Status = STATUS.Live.ToString(),
                    OwnerId = Id,
                    CategoryId = categoryId,
                    CreateAt = DateTime.Now
                           
                };

                if (newAuction.EndTime <= DateTime.Today && newAuction.StartTime <= DateTime.Today)
                {
                    if (newAuction.StartTime <= newAuction.EndTime && newAuction.StarPrice < newAuction.CurrentPrice)
                    {
                        await _uow.Auction.AddAsync(newAuction);
                        await _uow.SaveAsync();

                        return newAuction;
                    }
                    else
                    {
                        return null;
                    }
                }
                else return null;

                
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Auction>> GetAllAsync(string accId)
        {
            try
            {
                var isExistListAuction = await _uow.Auction.GetAllAsync(a => a.OwnerId == accId);
                if (isExistListAuction != null)
                {
                    return isExistListAuction;
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Auction> GetAsync(string auctionId, string accId)
        {
            try
            {
                var isExistAuction = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == auctionId && a.OwnerId == accId);
                if (isExistAuction != null)
                {
                    Auction view = new Auction()
                    {
                        AuctionId = isExistAuction.AuctionId,
                        StarPrice = isExistAuction.StarPrice,
                        CurrentPrice = isExistAuction.CurrentPrice,
                        CategoryId = isExistAuction.CategoryId,
                        StartTime = isExistAuction.StartTime,
                        EndTime = isExistAuction.EndTime,
                        FinalizePrice = isExistAuction.FinalizePrice,
                        OwnerId = isExistAuction.OwnerId,
                        Status = isExistAuction.Status,
                        WinnerId = isExistAuction.Status,
                        CreateAt = isExistAuction.CreateAt

                    };

                    return view;

                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

     

        public async Task<RESPONSECODE> UpdateAsync(string accId, CreateAuctionViewModel auctionView, string Id, string categoryId)
        {
            try
            {
                var isExistAuction = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == Id && a.OwnerId == accId);
                if (isExistAuction.Status == STATUS.Live.ToString())
                {
                    isExistAuction.StarPrice = auctionView.StarPrice;
                    isExistAuction.CurrentPrice = auctionView.CurrentPrice;
                    isExistAuction.FinalizePrice = auctionView.FinalizePrice;
                    isExistAuction.StartTime = auctionView.StartTime;
                    isExistAuction.EndTime = auctionView.EndTime;
                    isExistAuction.Status = STATUS.Live.ToString();
                    isExistAuction.CategoryId = categoryId;

                    _uow.Auction.Update(isExistAuction);
                    await _uow.SaveAsync();

                    return RESPONSECODE.OK;
                }return RESPONSECODE.INTERNALERROR;

            }
            catch
            {
                return RESPONSECODE.ERROR;
            }
        }
    }
}
