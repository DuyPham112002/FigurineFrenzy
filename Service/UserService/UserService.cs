using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.User;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.UserService
{
    public interface IUserService
    {
        Task<RESPONSECODE> CreateAsync(CreateUserViewModel user, string accId);
        Task<User> GetAsync(string accId);
        Task<RESPONSECODE> UpdateAsync(CreateUserViewModel user, string accId);
        
    }
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _uow;

        public UserService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<RESPONSECODE> CreateAsync(CreateUserViewModel user, string accId)
        {
            User getUser = await _uow.User.GetFirstOrDefaultAsync(a=>a.Email == user.Email);
            if (getUser == null)
            {
                User newUser = new User()
                {
                    Id = Guid.NewGuid().ToString(),
                    AccountId = accId,
                    Address = user.Address,
                    Email = user.Email,
                    FullName = user.FullName,
                    DateOfBirth = user.DateOfBirth,
                };

                try
                {
                    await _uow.User.AddAsync(newUser);
                    await _uow.SaveAsync();
                    return RESPONSECODE.OK;

                }
                catch (Exception ex)
                {
                    return RESPONSECODE.INTERNALERROR;
                }
            }
            return RESPONSECODE.BADREQUEST;
        }

        public async Task<User> GetAsync(string accId)
        {
            try
            {
                User userInfo = await _uow.User.GetFirstOrDefaultAsync(a => a.AccountId == accId);
                if (userInfo != null)
                {
                    User viewModel = new User
                    {
                        FullName = userInfo.FullName,
                        Address = userInfo.Address,
                        DateOfBirth = userInfo.DateOfBirth.Value,
                        Email = userInfo.Email
                    };
                    return viewModel;
                }
                else return null;
            }catch (Exception ex)
            {
                return null;
            }
           
        }

        public async Task<RESPONSECODE> UpdateAsync(CreateUserViewModel user, string accId)
        {
            try
            {
                User userInfo = await _uow.User.GetFirstOrDefaultAsync(a => a.AccountId==accId);
                if (userInfo != null)
                {
                    userInfo.FullName = user.FullName;
                    userInfo.Address = user.Address;
                    userInfo.Email = user.Email;
                    userInfo.DateOfBirth = user.DateOfBirth;

                    _uow.User.Update(userInfo);
                    await _uow.SaveAsync();

                    return RESPONSECODE.OK;
                }
                else return RESPONSECODE.INTERNALERROR;

            }catch (Exception ex)
            {
                return RESPONSECODE.ERROR;
            }
        }
    }
}
