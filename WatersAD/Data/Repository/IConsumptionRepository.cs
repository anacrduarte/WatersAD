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
        Consumption GetPreviousConsumption(WaterMeter waterMeter);

        /// <summary>
        /// Asynchronously retrieves all consumption invoices associated with the specified client ID.
        /// </summary>
        /// <param name="id">The unique identifier of the client.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="Consumption"/> objects representing the invoices for the client.</returns>
        Task<ICollection<Consumption>> GetAllInvoicesForClientAsync(int id);

        /// <summary>
        /// Asynchronously retrieves a <see cref="Consumption"/> record, including the associated water meter and client information, based on the specified consumption ID.
        /// </summary>
        /// <param name="consumptionId">The unique identifier of the consumption record.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Consumption"/> object with the associated water meter and client details.</returns>
        Task<Consumption> GetWaterMeterAndClientAsync(int consumptionId);

        /// <summary>
        /// Asynchronously retrieves all consumption records associated with a specific water meter ID.
        /// </summary>
        /// <param name="waterMeterId">The unique identifier of the water meter.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="Consumption"/> objects for the specified water meter.</returns>
        Task<IEnumerable<Consumption>> GetAllConsumptionForWaterMeter(int waterMeterId);

        /// <summary>
        /// Asynchronously retrieves a specific consumption record based on the provided invoice ID.
        /// </summary>
        /// <param name="invoiceId">The unique identifier of the invoice.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Consumption"/> object associated with the given invoice ID.</returns>
        Task<Consumption> GetConsumptionAsync(int invoiceId);
    }
}
