using DBAccess.Entites;
using DBAccess.Repository.Base;
using DBAccess.Repository.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DBAccess.Repository.Item
{
    public class ItemRepository : Repository<DBAccess.Entites.Item>, IItemRepository
    {
        private readonly FigurineFrenzyContext _context;

        public ItemRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Entites.Item item)
        {
            _context.Items.Update(item);
        }
    }
}
