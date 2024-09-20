using Microsoft.EntityFrameworkCore;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public class TierRepository :GenericRepository<Tier>, ITierRepository
    {
        private readonly DataContext _context;

        public TierRepository(DataContext context): base(context) 
        {
            _context = context;
        }

        public async Task<List<Tier>> GetAllAsync()
        {
            return await _context.Set<Tier>().AsNoTracking().ToListAsync();
        }

        public async Task<Tier?> GetMatchingTierAsync(double consumptionValue)
        {
            return await _context.Tiers
                .Where(t => consumptionValue <= t.UpperLimit)
                .OrderBy(t => t.UpperLimit)
                .FirstOrDefaultAsync();
        }
    }
}
