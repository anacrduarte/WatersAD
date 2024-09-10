using Microsoft.EntityFrameworkCore;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public class ClientRepository : GenericRepository<Client>, IClientRepository
    {
        private readonly DataContext _context;

        public ClientRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<Client> GetAllWithLocalities()
        {
            return _context.Clients
                           .Include(c => c.Locality) 
                           .OrderBy(c => c.FirstName)
                           .ToList();
        }
    }
}
