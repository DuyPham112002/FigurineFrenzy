using DBAccess.Entites;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBAccess.Repository.Category
{
    public class CategoryRepository : Repository<DBAccess.Entites.Category>, ICategoryRepository
    {
        private readonly FigurineFrenzyContext _context;

        public CategoryRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Entites.Category cate)
        {
            _context.Categories.Update(cate);
        }
    }
}
