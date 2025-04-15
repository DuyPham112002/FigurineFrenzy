using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Item;
using Microsoft.AspNetCore.Mvc;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ItemService
{
    public interface IItemService
    {
        public Task<RESPONSECODE> CreateAsync(CreateItemViewModel item, string imageSetId);
        public Task<List<GetinfoItemViewModel>> GetAllAsync(string AuctionId);
        public Task<RESPONSECODE> DeleteAsync(string ItemId);
    }
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _uow;
        private readonly FigurineFrenzyContext _context;
        public ItemService(IUnitOfWork uow, FigurineFrenzyContext context)
        {
            _uow = uow;
            _context = context;
        }

        public async Task<RESPONSECODE> CreateAsync(CreateItemViewModel item, string imageSetId)
        {
            try
            {
                var newItem = new Item()
                {
                    ItemId = Guid.NewGuid().ToString(),
                    NameOfProduct = item.NameOfProduct,
                    Description = item.Description,
                    ImageSetId = imageSetId,
                    AuctionId = item.AuctionId
                };


                
                await _uow.Item.AddAsync(newItem);
                await _uow.SaveAsync();

                return RESPONSECODE.OK;
            }
            catch
            {
                return RESPONSECODE.ERROR; 
            }
        }

        public async Task<RESPONSECODE> DeleteAsync(string ItemId)
        {
            try
            {
                var getItem = await _uow.Item.GetFirstOrDefaultAsync(a => a.ItemId == ItemId);
                if(getItem != null)
                {
                    _uow.Item.Remove(getItem);
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

        public async Task<List<GetinfoItemViewModel>> GetAllAsync(string AuctionId)
        {
            try
            {
                var items = await _uow.Item.GetAllAsync(a => a.AuctionId == AuctionId);
                if (items != null && items.Count > 0)
                {
                    return items.Select(a => new GetinfoItemViewModel
                    {
                        ItemId = a.ItemId,
                        NameOfProduct = a.NameOfProduct,
                        Description = a.Description,
                        ImageSetId = a.ImageSetId
                    }).ToList();
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
