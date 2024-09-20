using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Data.Repository
{
    public interface IConsumptionRepository : IGenericRepository<Consumption>
    {
        IEnumerable<Consumption> GetAllWaterMeterAndClient();

        Task CreateConsumptionAndInvoiceAsync(ConsumptionViewModel model, WaterMeter waterMeter, Tier matchingTier, Consumption previousConsumption);

        Consumption? GetPreviousConsumption(WaterMeter waterMeter);

    }
}
