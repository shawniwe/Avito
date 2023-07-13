using Avito2.Domains;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Avito2.Data.Repositories
{
    public class MessagesRepository : SqlRepository<Message>
    {
        public MessagesRepository(ApplicationDbContext context) : base(context) { }

        public override IEnumerable<Message> ReadList()
        {
            return _context.Messages.Include(x => x.Advertisement).AsEnumerable();
        }

        public override Message Read(long? Id)
        {
            return _context.Messages
                .Where(x => x.Id == Id)
                .Include(x => x.Advertisement)
                .FirstOrDefault();
        }
    }
}
