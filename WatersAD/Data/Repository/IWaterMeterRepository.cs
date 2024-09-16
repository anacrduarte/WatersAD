using Microsoft.AspNetCore.Mvc.Rendering;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IWaterMeterRepository : IGenericRepository<WaterMeter>
    {
        Task AddWaterMeterAsync(WaterMeterService meterService);

        IQueryable GetWaterMeterServices();

        Task<WaterMeterService> GetWaterServiceByIdAsync(int id);

        Task DeleteWaterServiceAsync(WaterMeterService meterService);

        IEnumerable<SelectListItem> GetComboWaterMeterServices();

        Task<List<WaterMeter>> GetWaterMeterWithClients();

        Task<WaterMeter> GetWaterMeterWithCityAndCountryAsync(int waterMeterId);

        Task<WaterMeter> GetClientAndLocalityWaterMeterAsync(int waterMeterId);

        Task UpdateWaterServiceAsync(WaterMeterService waterMeter);

       
    }
}
