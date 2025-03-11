using DBAccess.Entites;
using DBAccess.UnitOfWork;
using FigurineFrenzeyViewModel.Account;
using Service.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AccountService
{
    public interface IAccountService
    {
        Task<RESPONSECODE> CreateAsync(CreateAccountViewModel create, string accId, string roleInfo);
        Task<Account> CheckLogin(LoginInfoViewModel loginInfo, string roleId);

    }
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _uow;
        public AccountService(IUnitOfWork uwo)
        {
            _uow = uwo;
        }

        public async Task<Account> CheckLogin(LoginInfoViewModel loginInfo, string roleId)
        {
            Account accInfo = await _uow.Account.GetFirstOrDefaultAsync(a => a.Phone == loginInfo.Phone && a.Password == loginInfo.Password
            && a.IsActive && a.RoleId == roleId);

            if (accInfo != null)
            {
                return accInfo;
            }
            else return null;
        }

        public async Task<RESPONSECODE> CreateAsync(CreateAccountViewModel create, string accId, string roleInfo)
        {
            Account acc = await _uow.Account.GetFirstOrDefaultAsync(a=>a.Phone == create.Phone);
            if (acc == null)
            {
                Account newAcc = new Account()
                {
                    AccountId = accId,
                    Phone = create.Phone,
                    Password = create.Password,
                    CreateAt = DateTime.Now,
                    RoleId = roleInfo,
                    IsActive = true,

                };
                try
                {
                    await _uow.Account.AddAsync(newAcc);
                    await _uow.SaveAsync();
                    return RESPONSECODE.OK;

                }
                catch (Exception ex)
                {
                    return RESPONSECODE.INTERNALERROR;
                }
            }
            else return RESPONSECODE.BADREQUEST;
        }
    }
}
