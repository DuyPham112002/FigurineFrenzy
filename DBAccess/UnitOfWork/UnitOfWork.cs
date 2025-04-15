using DBAccess.Entites;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FigurineFrenzyContext _context;

        public IAccountRepository Account { get; private set; }

        public IRoleRepository Role { get; private set; }

        public ITokenRespository Token { get; private set; }

        public IUserRepository User { get; private set; }

        public IAdminRepository Admin { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IAuctionRepository Auction { get; private set; }
        public IBidRepository Bid { get; private set; }
        public IItemRepository Item { get; private set; }

        public IImageSetRepository ImageSet { get; private set; }

        public IImageRepository Image { get; private set; }

        public UnitOfWork(FigurineFrenzyContext context)
        {
            _context = context;
            Account = new AccountRepository(context);
            Role = new RoleRepository(context);
            Admin = new AdminRepository(context);
            Token = new TokenRepository(context);   
            User = new UserRepository(context);
            Category = new CategoryRepository(context);
            Auction = new AuctionRepository(context);
            Bid = new BidRepository(context);
            Item = new ItemRepository(context);
            Image = new ImageRepository(context);
            ImageSet = new ImageSetRepository(context);
        }



        public void Dispose()
        {
            _context.Dispose();
        }

        public void Detach()
        {
            _context.ChangeTracker.Clear();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
