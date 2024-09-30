using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Data.Repository
{
    public class WaterMeterRepository : GenericRepository<WaterMeter>, IWaterMeterRepository
    {
        private readonly DataContext _context;

        public WaterMeterRepository(DataContext context) : base(context)
        {
            _context = context;
        }
        public async Task AddWaterMeterAsync(WaterMeterService meterService)
        {
            await _context.Set<WaterMeterService>().AddAsync(meterService);

            await _context.SaveChangesAsync();


        }

        public async Task AddRequestWaterMeterAsync(RequestWaterMeter request)
        {
            await _context.Set<RequestWaterMeter>().AddAsync(request);

            await _context.SaveChangesAsync();


        }
        public IQueryable GetWaterMeterServices()
        {
            return _context.Set<WaterMeterService>().Where(wm=> wm.Available).AsNoTracking();

        }

        public async Task<List<WaterMeter>> GetWaterMeterWithClients()
        {
            return await _context.WaterMeters
                .Include(c => c.Client)
                .Include(l=> l.Locality)
                .ToListAsync();

        }

        public async Task<WaterMeter> GetWaterMeterWithCityAndCountryAsync(int waterMeterId)
        {
            return await _context.WaterMeters
                .Include(wm => wm.Locality)
                    .ThenInclude(l => l.City)
                        .ThenInclude(c => c.Country) 
                .Where(wm => wm.Id == waterMeterId)
                .FirstOrDefaultAsync();
        }

        public async Task<WaterMeter> GetClientAndLocalityWaterMeterAsync(int waterMeterId)
        {
            return await _context.WaterMeters
                .Include(s=> s.WaterMeterService)
                .Include(c => c.Client)
                .Include(l => l.Locality)
                 .ThenInclude(ci=> ci.City)
                 .ThenInclude(co => co.Country)
                .Where(wm => wm.Id == waterMeterId)
                .FirstOrDefaultAsync();
        }

        public async Task<WaterMeterService> GetWaterServiceByIdAsync(int id)
        {
            return await _context.Set<WaterMeterService>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task DeleteWaterServiceAsync(WaterMeterService meterService)
        {
            _context.Set<WaterMeterService>().Remove(meterService);

            await _context.SaveChangesAsync();
        }

        public async Task<WaterMeter?> GetWaterMeterWithConsumptionsAsync(int waterMeterId)
        {
            return await _context.WaterMeters
                .Include(wm => wm.Consumptions)
                .FirstOrDefaultAsync(wm => wm.Id == waterMeterId);
        }


        public IEnumerable<SelectListItem> GetComboWaterMeterServices()
        {
            var list = _context.WaterMeterServices.Select(c => new SelectListItem
            {
                Text = c.SerialNumber,
                Value = c.Id.ToString(),

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Water Meter...)",
                Value = "0",
            });

            return list;
        }


        public async Task UpdateWaterServiceAsync(WaterMeterService waterMeter)
        {
            _context.Set<WaterMeterService>().Update(waterMeter);

            await _context.SaveChangesAsync();
        }

        //TODO arranjar outra maneira de mostar os water meter
        public IEnumerable<SelectListItem> GetComboWaterMeter()
        {
            var list = _context.WaterMeters.Select(c => new SelectListItem
            {
                Text = c.Address,
                Value = c.Id.ToString(),

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Water Meter...)",
                Value = "0",
            });

            return list;
        }

        public async Task<IEnumerable<WaterMeter>> GetWaterMetersWithConsumptionsByClientAsync(int id)
        {
            return await _context.WaterMeters
                .Include(wm => wm.Consumptions)
                .Where(wm => wm.Client.Id == id) 
                .ToListAsync();
        }

  
    }
}
