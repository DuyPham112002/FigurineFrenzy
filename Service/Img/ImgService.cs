using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Img
{
    public interface IImgService
    {
        Task<bool> CreateImageBase(ImageViewModel image);
    }
    public class ImgService : IImgService
    {
        private readonly IUnitOfWork _uow;
        public ImgService(IUnitOfWork uow)
        {
            _uow = uow;
        }



        public async Task<bool> CreateImageBase(ImageViewModel image)
        {
            try
            {
      
                Image newImage = new Image()
                {
                    Id = Guid.NewGuid().ToString(),
                    ImageSetId = image.ImageSetId,
                    ImageUrl = image.ImageUrl,
                    IsActive = true
                };
                await _uow.Image.AddAsync(newImage);
                await _uow.SaveAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
