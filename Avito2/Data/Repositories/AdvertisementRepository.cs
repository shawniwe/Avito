using Avito2.Domains;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Avito2.Data.Repositories
{
    public class AdvertisementRepository : SqlRepository<Advertisement>
    {
        public AdvertisementRepository(ApplicationDbContext context) : base(context) { }

        public override IEnumerable<Advertisement> ReadList()
        {
            return _context.Advertisements
                .Include(x => x.Photos)
                .Include(x => x.Statement)
                .Include(x => x.Category);
        }

        public override Advertisement Read(long? Id)
        {
            return _context.Advertisements
                .Where(x => x.Id == Id)
                .Include(x => x.Photos)
                .Include(x => x.Category)
                .Include(x => x.Statement)
                .FirstOrDefault();
        }
    }
}
