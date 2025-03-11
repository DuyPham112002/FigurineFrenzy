using DBAccess.Entites;
using DBAccess.Repository.Base;
using DBAccess.Repository.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DBAccess.Repository.Image
{
    public class ImageRepository : Repository<DBAccess.Entites.Image>, IImageRepository
    {
        private readonly FigurineFrenzyContext _context;

        public ImageRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Entites.Image image)
        {
            _context.Images.Update(image);
        }
    }
}
