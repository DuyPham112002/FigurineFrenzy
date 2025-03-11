using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Item;
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
    }
    public class ItemService : IItemService
    {
        private readonly IUnitOfWork _uow;
        public ItemService(IUnitOfWork uow)
        {
            _uow = uow;
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
    }
}
