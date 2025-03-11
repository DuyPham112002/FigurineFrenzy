using DBAccess.Repository.Account;
using DBAccess.Repository.Admin;
using DBAccess.Repository.Auction;
using DBAccess.Repository.Bid;
using DBAccess.Repository.Category;
using DBAccess.Repository.Image;
using DBAccess.Repository.ImageSet;
using DBAccess.Repository.Item;
using DBAccess.Repository.Role;
using DBAccess.Repository.Token;
using DBAccess.Repository.User;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        Task SaveAsync();
        IAccountRepository Account {  get; }
        IRoleRepository Role {  get; }
        ITokenRespository Token { get; }
        IUserRepository User {  get; }
        IAdminRepository Admin {  get; }
        ICategoryRepository Category { get; }
        IAuctionRepository Auction { get; }
        IItemRepository Item { get; }
        IBidRepository Bid { get; }
        IImageSetRepository ImageSet { get; }
        IImageRepository Image { get; }

    }
}
