using DBAccess.Entites;
using DBAccess.UnitOfWork;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImgSet
{
    public interface IImgSetService
    {
        Task<RESPONSECODE> CreateAsync(string imgSetId);
        Task<ImageSet> GetAsync(string imgSetId);
    }
    public class ImgSetService : IImgSetService
    {
        private readonly IUnitOfWork _uow;
        public ImgSetService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RESPONSECODE> CreateAsync(string imgSetId)
        {
            try
            {
                var newImgSet = new ImageSet
                {
                    Id = imgSetId,
                    IsActive = true
                };
                await _uow.ImageSet.AddAsync(newImgSet);
                await _uow.SaveAsync();

                return RESPONSECODE.OK;
            }
            catch
            {
                return RESPONSECODE.INTERNALERROR;
            }    
        }

        public async Task<ImageSet> GetAsync(string imgSetId)
        {
            try
            {
                var isExist = await _uow.ImageSet.GetFirstOrDefaultAsync(a=>a.Id == imgSetId);
                if(isExist != null && isExist.IsActive == true)
                {
                    return isExist;
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
