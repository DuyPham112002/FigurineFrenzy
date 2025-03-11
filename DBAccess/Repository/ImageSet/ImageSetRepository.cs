using DBAccess.Entites;
using DBAccess.Repository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DBAccess.Repository.ImageSet
{
    public class ImageSetRepository : Repository<DBAccess.Entites.ImageSet>, IImageSetRepository
    {
        private readonly FigurineFrenzyContext _context;

        public ImageSetRepository(FigurineFrenzyContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Entites.ImageSet imageSet)
        {
            _context.ImageSets.Update(imageSet);
        }
    }
}
