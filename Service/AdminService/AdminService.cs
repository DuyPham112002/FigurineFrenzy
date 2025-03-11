using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Account;
using FigurineFrenzeyViewModel.Admin;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AdminService
{
    public interface IAdminService
    {
        Task<RESPONSECODE> CreateAsync(CreateAdminViewModel admin, string accId);
        Task<RESPONSECODE> UpdateAsync(CreateAdminViewModel admin, string accId);
        Task<Admin> GetAsync(string accId);
    }
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _uow;
        public AdminService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<RESPONSECODE> CreateAsync(CreateAdminViewModel admin, string accId)
        {
            try
            {
                Admin acc = await _uow.Admin.GetFirstOrDefaultAsync(a => a.Email == admin.Email);
                if (acc == null)
                {
                    Admin newAdmin = new Admin
                    {
                        Id = Guid.NewGuid().ToString(),
                        AccountId = accId,
                        Email = admin.Email
                    };

                    await _uow.Admin.AddAsync(newAdmin);
                    await _uow.SaveAsync();

                    return RESPONSECODE.OK;
                }
                else return RESPONSECODE.INTERNALERROR;
            }
            catch(Exception ex)
            {
                return RESPONSECODE.ERROR;
            }
        }

        public async Task<Admin> GetAsync(string accId)
        {
            try
            {
                Admin adminInfo = await _uow.Admin.GetFirstOrDefaultAsync(a => a.AccountId == accId);
                if (adminInfo != null)
                {
                    return adminInfo;
                }
                else return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<RESPONSECODE> UpdateAsync(CreateAdminViewModel admin, string accId)
        {
            try
            {
                Admin adminInfo = await _uow.Admin.GetFirstOrDefaultAsync(a => a.AccountId == accId);
                List<Admin> ListAdminInfo = await _uow.Admin.GetAllAsync(a => a.Email == admin.Email);
                if (adminInfo != null && ListAdminInfo.Count < 1)
                {
                    adminInfo.Email = admin.Email;

                    _uow.Admin.Update(adminInfo);
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
