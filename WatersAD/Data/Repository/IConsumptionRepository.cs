using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Data.Repository
{
    public interface IConsumptionRepository : IGenericRepository<Consumption>
    {
        /// <summary>
        ///  Retrieves all consumption records, including the associated water meter and client data.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Consumption}"/> of consumption records with water meter and client information.</returns>
        IEnumerable<Consumption> GetAllWaterMeterAndClient();

        /// <summary>
        /// Asynchronously creates a new consumption record and generates an associated invoice based on the provided model, water meter, tier, and previous consumption.
        /// </summary>
        /// <param name="model">The <see cref="ConsumptionViewModel"/> containing the consumption data to be recorded.</param>
        /// <param name="waterMeter">The <see cref="WaterMeter"/> associated with the consumption.</param>
        /// <param name="matchingTier">The <see cref="Tier"/> used to calculate the consumption charge.</param>
        /// <param name="previousConsumption">The previous <see cref="Consumption"/> record for the water meter, used for calculation purposes.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task CreateConsumptionAndInvoiceAsync(ConsumptionViewModel model, WaterMeter waterMeter, Tier matchingTier, Consumption previousConsumption);

        /// <summary>
        /// Retrieves the previous consumption record for a given water meter, if available.
        /// </summary>
        /// <param name="waterMeter">The <see cref="WaterMeter"/> for which to retrieve the previous consumption record.</param>
        /// <returns>The previous <see cref="Consumption"/> record, or null if no prior consumption exists.</returns>
        Consumption? GetPreviousConsumption(WaterMeter waterMeter);

    }
}
