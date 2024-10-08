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


        public IEnumerable<Client> GetAllWithLocalitiesAndWaterMeter()
        {
            return _context.Clients
                           .Include(c => c.Locality)
                           .ThenInclude(c => c.City)
                           .Include(c => c.WaterMeters)
                           .Where(c => c.IsActive)
                           .OrderBy(c => c.FirstName)
                           .ToList();
        }

        public IEnumerable<Client> GetAllWithLocalitiesAndWaterMeterInactive()
        {
            return _context.Clients
                            .Include(c => c.Locality)
                           .ThenInclude(c => c.City)
                           .Include(c => c.Locality)
                           .Include(c => c.WaterMeters)
                           .Where(c => !c.IsActive)
                           .OrderBy(c => c.FirstName)
                           .ToList();
        }

        public async Task<Client> GetClientAndLocalityAndCityAsync(int clientId)
        {
            return await _context.Clients
                .Include(l => l.Locality)
                 .ThenInclude(ci => ci.City)
                 .ThenInclude(co => co.Country)
                .FirstOrDefaultAsync(c => c.Id == clientId);
        }

        public async Task<Client> GetClientWithWaterMeter(int clientId)
        {
            return await _context.Clients
                .Include(c => c.WaterMeters)
                .FirstOrDefaultAsync(c => c.Id == clientId);
        }

        public async Task<Client> GetClientByUserEmailAsync(string email)
        {
            return await _context.Clients
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Email == email);
        }


    }
}
