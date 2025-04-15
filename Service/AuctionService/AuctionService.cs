using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Auction;
using FigurineFrenzeyViewModel.Image;
using FigurineFrenzeyViewModel.Item;
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
        public Task<GetInfroAuctionViewModel> GetAsync(string auctionId);
        public Task<List<Auction>> GetAllByAccIdAsync(string accId);
        public Task<List<Auction>> GetAllAsync();  
        public Task<List<GetInfroAuctionViewModel>> GetAllLiveAuctionAsync();
        public Task<RESPONSECODE> DeleteAsync(string accId, string AuctionId);
        public Task<string> CompletedAsync(string AuctionId);
        public Task<string> StartAsync(string AuctionId);
        public Task<RESPONSECODE> UpdateCurrentPriceAsync(string auctionId, double currentPrice);
        public Task<RESPONSECODE> DeleteAysc(string auctionId);
    }
    public class AuctionService : IAuctionService
    {
        private readonly IUnitOfWork _uow;
        public AuctionService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<string> StartAsync(string AuctionId)
        {
            var isExsitAuctionStart = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == AuctionId);
            if (isExsitAuctionStart != null && DateTime.Now >= isExsitAuctionStart.StartTime && isExsitAuctionStart.StartTime < isExsitAuctionStart.EndTime
                && isExsitAuctionStart.Status == STATUS.NotStart.ToString())
            {
                try
                {
                    isExsitAuctionStart.Status = STATUS.Live.ToString();
                    _uow.Auction.Update(isExsitAuctionStart);
                    await _uow.SaveAsync();

                    return isExsitAuctionStart.Status;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }


            }
            else return isExsitAuctionStart.Status;
        }

        public async Task<string> CompletedAsync(string AuctionId)
        {
            var expriedAuction = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == AuctionId);
            if (expriedAuction != null && expriedAuction.EndTime < DateTime.Now && expriedAuction.EndTime > expriedAuction.StartTime && expriedAuction.Status == STATUS.Live.ToString())
            {
                try
                {
                    expriedAuction.Status = STATUS.Ended.ToString();
                    _uow.Auction.Update(expriedAuction);
                    await _uow.SaveAsync();

                    return expriedAuction.Status;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }


            }
            else return "This Auction is not End";
        }

        public async Task<Auction> CreateAsync(string Id, CreateAuctionViewModel auctionView, string categoryId)
        {
            //If Step Price = 0 means user not type StepPrice ==> seem Null
            try
            {
                //Case 1: Use Default and Exist Finalize Price and No StartPrice
                if (!auctionView.CustomStepPrice && auctionView.FinalizePrice != 0 && auctionView.StepPrice == 0)
                {
                    Auction newAuction = new Auction
                    {
                        AuctionId = Guid.NewGuid().ToString(),
                        FinalizePrice = auctionView.FinalizePrice,
                        StartTime = auctionView.StartTime,
                        EndTime = auctionView.EndTime,
                        Status = STATUS.NotStart.ToString(),
                        OwnerId = Id,
                        CategoryId = categoryId,
                        CreateAt = DateTime.Now,
                        CustomStepPrice = auctionView.CustomStepPrice

                    };
                
                    if (newAuction.EndTime >= DateTime.Now && newAuction.StartTime < newAuction.EndTime)
                    {
                       
                        await _uow.Auction.AddAsync(newAuction);
                        await _uow.SaveAsync();

                        return newAuction;

                    }
                    else throw new ArgumentException("Please check Time Start and Time End of Auction are correct ?");



                }
                //Case 2: Use Default and Exist StartPrice and No FinalPrice
                else if (!auctionView.CustomStepPrice && auctionView.FinalizePrice == 0 && auctionView.StartPrice != null && auctionView.StepPrice == 0)
                {
                    Auction newAuction = new Auction
                    {
                        AuctionId = Guid.NewGuid().ToString(),
                        StartTime = auctionView.StartTime,
                        EndTime = auctionView.EndTime,
                        Status = STATUS.NotStart.ToString(),
                        OwnerId = Id,
                        CategoryId = categoryId,
                        CreateAt = DateTime.Now,
                        CustomStepPrice = auctionView.CustomStepPrice,
                        StartPrice = auctionView.StartPrice
                    };
                    if (newAuction.EndTime >= DateTime.Now && newAuction.StartTime < newAuction.EndTime)
                    {

                        await _uow.Auction.AddAsync(newAuction);
                        await _uow.SaveAsync();

                        return newAuction;

                    }
                    else throw new ArgumentException("Please check Time Start and Time End of Auction are correct ?");
                }
                //Case 3: Use Custom Step Price and Exist Finalize Price and Step Price, Start Price (Required)
                else if (auctionView.CustomStepPrice && auctionView.FinalizePrice != null && auctionView.StepPrice != null)
                {
                    if (ValidStarPrice(auctionView.StartPrice, auctionView.FinalizePrice) && ValidStepPrice(auctionView.StepPrice, auctionView.StartPrice)) //Step Price should be max 50% of Finalize Price
                        throw new ArgumentException("Step Price should be max 50% of Finalize Price");
                    Auction newAuction = new Auction
                    {
                        AuctionId = Guid.NewGuid().ToString(),
                        StartPrice = auctionView.StartPrice,
                        FinalizePrice = auctionView.FinalizePrice,
                        StartTime = auctionView.StartTime,
                        EndTime = auctionView.EndTime,
                        Status = STATUS.NotStart.ToString(),
                        OwnerId = Id,
                        CategoryId = categoryId,
                        CreateAt = DateTime.Now,
                        StepPrice = auctionView.StepPrice,
                        CustomStepPrice = auctionView.CustomStepPrice,
                    };
                   
                    if (newAuction.EndTime >= DateTime.Now && newAuction.StartTime < newAuction.EndTime)
                    {
                       
                        await _uow.Auction.AddAsync(newAuction);
                        await _uow.SaveAsync();

                        return newAuction;

                    }
                    else throw new ArgumentException("Please check Time Start and Time End of Auction are correct ?");

                }else
                {
                    Auction newAuction = new Auction
                    {
                        AuctionId = Guid.NewGuid().ToString(),
                        StartPrice = auctionView.StartPrice,
                        FinalizePrice = auctionView.FinalizePrice,
                        StartTime = auctionView.StartTime,
                        EndTime = auctionView.EndTime,
                        Status = STATUS.NotStart.ToString(),
                        OwnerId = Id,
                        CategoryId = categoryId,
                        CreateAt = DateTime.Now,
                        CustomStepPrice = auctionView.CustomStepPrice
                    };
                  
                    if (newAuction.EndTime >= DateTime.Now && newAuction.StartTime < newAuction.EndTime)
                    {

                        await _uow.Auction.AddAsync(newAuction);
                        await _uow.SaveAsync();
                        return newAuction;
                    }
                    else throw new ArgumentException("Please check Time Start and Time End of Auction are correct ?");
                }


                    return null;    
            }
            catch(Exception ex)
            {
                throw new AggregateException("Can't Create Auction");
            }
        }

        //Validate StarPirce
        private bool ValidStarPrice(double StartPrice, double FinalizePrice)
        {
            if (StartPrice <= FinalizePrice * 50 / 100 && StartPrice >= 0)
                return true;
            else return false;
        }

        //Validate StepPrice
        private bool ValidStepPrice(double? StartPrice, double StepPrice)
        {
            if (StepPrice >= StartPrice / 100 && StepPrice <= StartPrice * 50 / 100)
            {
                return true;
            }
            return false;
        }

        public async Task<RESPONSECODE> DeleteAsync(string accId, string AuctionId)
        {
            try
            {
                var isExistAuction = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == AuctionId && a.OwnerId == accId);
                if (isExistAuction != null)
                {
                    isExistAuction.Status = STATUS.Deleted.ToString();
                    _uow.Auction.Update(isExistAuction);
                    await _uow.SaveAsync();
                    return RESPONSECODE.OK;
                }
                else return RESPONSECODE.INTERNALERROR;
            }
            catch
            {
                return RESPONSECODE.ERROR;
            }
        }

        public async Task<List<GetInfroAuctionViewModel>> GetAllLiveAuctionAsync()
        {
            try
            {
                var isExistListAuction = await _uow.Auction.GetAllAsync(a => (a.Status == STATUS.Live.ToString() || a.Status == STATUS.NotStart.ToString())  && a.EndTime > DateTime.Now, null, "Category,Owner.Users,Items,Items.ImageSet.Images");
                if (isExistListAuction != null)
                {
                    var result = isExistListAuction.Select(a => new GetInfroAuctionViewModel
                    {
                        AuctionId = a.AuctionId,
                        StartPrice = a.StartPrice,
                        CurrentPrice = a.CurrentPrice,
                        CategoryId = a.CategoryId,
                        CategoryName = a.Category?.Name,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        OwnerId = a.OwnerId,
                        Status = a.Status,
                        CreatAt = a.CreateAt,
                        OwnerName = a.Owner?.Users.Select(a => a.FullName).FirstOrDefault(),
                        Items = a.Items?.Select(i => new GetinfoItemViewModel
                        {
                            ItemId = i.ItemId,
                            NameOfProduct = i.NameOfProduct,
                            Description = i.Description,
                            ImageSetId = i.ImageSetId

                        }).ToList(),
                        Images = a.Items?.Select(i => i.ImageSet).SelectMany(i => i.Images).Select(i => new InfoImageViewModel
                        {
                            Id = i.ImageSetId,
                            ImgUrl = i.ImageUrl
                        }).ToList()
                    }).ToList();

                    return result;
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<Auction>> GetAllByAccIdAsync(string accId)
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

        public async Task<GetInfroAuctionViewModel> GetAsync(string auctionId)
        {
            try
            {
                var isExistAuction = await _uow.Auction.GetAllAsync(a => a.AuctionId == auctionId, null, "Category,Owner.Users,Items,Items.ImageSet.Images");
                if (isExistAuction != null)
                {
                    var result = isExistAuction.Select(a => new GetInfroAuctionViewModel
                    {
                        AuctionId = a.AuctionId,
                        StartPrice = a.StartPrice,
                        CurrentPrice = a.CurrentPrice,
                        CategoryId = a.CategoryId,
                        CategoryName = a.Category?.Name,
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        OwnerId = a.OwnerId,
                        Status = a.Status,
                        CreatAt = a.CreateAt,
                        FinalizePrice = a.FinalizePrice,
                        StepPrice = a.StepPrice,
                        OwnerName = a.Owner?.Users.Select(a => a.FullName).FirstOrDefault(),
                        Items = a.Items?.Select(i => new GetinfoItemViewModel
                        {
                            ItemId = i.ItemId,
                            NameOfProduct = i.NameOfProduct,
                            Description = i.Description,
                            ImageSetId = i.ImageSetId

                        }).ToList(),
                        Images = a.Items?.Select(i => i.ImageSet).SelectMany(i => i.Images).Select(i => new InfoImageViewModel
                        {
                            Id = i.ImageSetId,
                            ImgUrl = i.ImageUrl
                        }).ToList()
                    }).FirstOrDefault();

                    return result;

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
                    isExistAuction.StartPrice = auctionView.StartPrice;
                    isExistAuction.FinalizePrice = auctionView.FinalizePrice;
                    isExistAuction.StartTime = auctionView.StartTime;
                    isExistAuction.EndTime = auctionView.EndTime;
                    isExistAuction.Status = STATUS.Live.ToString();
                    isExistAuction.CategoryId = categoryId;

                    _uow.Auction.Update(isExistAuction);
                    await _uow.SaveAsync();

                    return RESPONSECODE.OK;
                }
                return RESPONSECODE.INTERNALERROR;

            }
            catch
            {
                return RESPONSECODE.ERROR;
            }
        }

        public async Task<RESPONSECODE> UpdateCurrentPriceAsync(string auctionId, double currentPrice)
        {
            try
            {
                var isExistAuction = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == auctionId);
                if (isExistAuction != null)
                {
                    isExistAuction.CurrentPrice = currentPrice;
                    _uow.Auction.Update(isExistAuction);
                    await _uow.SaveAsync();
                    return RESPONSECODE.OK;
                }
                else return RESPONSECODE.INTERNALERROR;
            }
            catch
            {
                return RESPONSECODE.ERROR;
            }
        }

        public async Task<List<Auction>> GetAllAsync()
        {
            try
            {
                var isExistListAuction = await _uow.Auction.GetAllAsync();
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

        public async Task<RESPONSECODE> DeleteAysc(string auctionId)
        {
            try
            {
                var isExistAuction = await _uow.Auction.GetFirstOrDefaultAsync(a => a.AuctionId == auctionId);
                if (isExistAuction != null)
                {
                    _uow.Auction.Remove(isExistAuction);
                    await _uow.SaveAsync();
                    return RESPONSECODE.OK;
                }
                else return RESPONSECODE.INTERNALERROR;
            }
            catch
            {
                return RESPONSECODE.ERROR;
            }
        }
    }
}
