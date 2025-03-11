using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Category;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CategoryService
{
    public interface ICategoryService
    {
        Task<RESPONSECODE> CreateAsync(CategoryViewModel category);
        Task<List<Category>> GetAllAsync();
        Task<RESPONSECODE> DeleteAsync(string Id);
        Task<RESPONSECODE> ActivateAsync(string Id);
        Task<Category> GetAsync(string Id);
        Task<RESPONSECODE> UpdateAsync(string Id, CategoryViewModel categoryView);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _uow;
        
        public CategoryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RESPONSECODE> ActivateAsync(string Id)
        {
            try
            {
                Category cateInfo = await _uow.Category.GetFirstOrDefaultAsync(a => a.Id == Id && a.IsActive == false);
                if (cateInfo != null)
                {
                    cateInfo.IsActive = true;

                    _uow.Category.Update(cateInfo);
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

        public async Task<RESPONSECODE> CreateAsync(CategoryViewModel category)
        {
            try
            {
                Category cateInfo = await _uow.Category.GetFirstOrDefaultAsync(a => a.Name == category.Name);
                if (cateInfo == null)
                {
                    Category newCate = new Category
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = category.Name,
                        IsActive = true
                    };

                    await _uow.Category.AddAsync(newCate);
                    await _uow.SaveAsync();

                    return RESPONSECODE.OK;
                }
                else return RESPONSECODE.INTERNALERROR;
                

            }catch
            {
                return RESPONSECODE.ERROR;
            }
        }

        public async Task<RESPONSECODE> DeleteAsync(string Id)
        {
            try
            {
                Category cateInfo = await _uow.Category.GetFirstOrDefaultAsync(a => a.Id == Id && a.IsActive == true);
                if (cateInfo != null)
                {
                    cateInfo.IsActive = false;

                    _uow.Category.Update(cateInfo);
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

        public async Task<List<Category>> GetAllAsync()
        {
            try
            {
                List<Category> getAllCate = await _uow.Category.GetAllAsync();
                if (getAllCate != null)
                {
                    return getAllCate;
                }
                else return null;
                
            }
            catch { 
                return null;
            }
        }

        public async Task<Category> GetAsync(string Id)
        {
            try
            {
                Category getCateInfo = await _uow.Category.GetFirstOrDefaultAsync(a => a.Id == Id);
                if (getCateInfo != null)
                {
                    return getCateInfo;
                }
                else return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<RESPONSECODE> UpdateAsync(string Id, CategoryViewModel categoryView)
        {
            try
            {
                Category getCateInfo = await _uow.Category.GetFirstOrDefaultAsync(a => a.Id == Id);
                List<Category> ListCate = await _uow.Category.GetAllAsync(i => i.Name == categoryView.Name);
                if (getCateInfo != null && ListCate.Count == 0)
                {
                    getCateInfo.Name = categoryView.Name;

                    _uow.Category.Update(getCateInfo);
                    await _uow.SaveAsync();

                    return RESPONSECODE.OK;

                }
                else return RESPONSECODE.INTERNALERROR;
            }
            catch (Exception ex)
            {
                return RESPONSECODE.ERROR;
            }
        }
    }
}
