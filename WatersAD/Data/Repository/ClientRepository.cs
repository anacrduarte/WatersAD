using Microsoft.AspNetCore.Mvc.Rendering;
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

        //public IQueryable<Client> GetAllActive()
        //{
        //    return _context.Clients.AsNoTracking().Where(c => c.IsActive == true);
        //}

        //public IQueryable<Client> GetAllInactive()
        //{
        //    return _context.Clients.AsNoTracking().Where(c => c.IsActive == false);
        //}
        public IEnumerable<Client> GetAllWithLocalitiesAndWaterMeter()
        {
            return _context.Clients
                           .Include(c => c.Locality)
                           .Where(c => c.IsActive)
                           .Include(c => c.WaterMeters)
                           .OrderBy(c => c.FirstName)
                           .ToList();
        }

        public async Task<Client> GetClientAndLocalityAndCityAsync(int clientId)
        {
            return await _context.Clients
                .Include(l => l.Locality)
                 .ThenInclude(ci => ci.City)
                 .ThenInclude(co => co.Country)
                .Where(c => c.Id == clientId)
                .FirstOrDefaultAsync();
        }

        public async Task<Client> GetClientWithWaterMeter(int clientId)
        {
            return await _context.Clients
                .Include(c=> c.WaterMeters)
                .Where(c => c.Id == clientId)
                .FirstOrDefaultAsync();
        }
    }
}
