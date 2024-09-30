using Microsoft.AspNetCore.Mvc.Rendering;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IWaterMeterRepository : IGenericRepository<WaterMeter>
    {
        /// <summary>
        /// Asynchronously adds a new water meter service to the system.
        /// </summary>
        /// <param name="meterService">The <see cref="WaterMeterService"/> to add.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task AddWaterMeterAsync(WaterMeterService meterService);

        /// <summary>
        /// Retrieves all available water meter services.
        /// </summary>
        /// <returns>An <see cref="IQueryable"/> representing the water meter services.</returns>
        IQueryable GetWaterMeterServices();

        /// <summary>
        /// Asynchronously retrieves a water meter service by its ID.
        /// </summary>
        /// <param name="id">The ID of the water meter service to retrieve.</param>
        /// <returns>A <see cref="Task{WaterMeterService}"/> representing the asynchronous operation, returning the water meter service with the specified ID.</returns>
        Task<WaterMeterService> GetWaterServiceByIdAsync(int id);

        /// <summary>
        ///  Asynchronously deletes a water meter service.
        /// </summary>
        /// <param name="meterService">The <see cref="WaterMeterService"/> to delete.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task DeleteWaterServiceAsync(WaterMeterService meterService);

        /// <summary>
        /// Retrieves a collection of water meter services for use in a dropdown list.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SelectListItem}"/> representing water meter services for dropdown selection.</returns>
        IEnumerable<SelectListItem> GetComboWaterMeterServices();

        /// <summary>
        ///  Asynchronously retrieves a list of water meters along with their associated clients.
        /// </summary>
        /// <returns>A <see cref="Task{List{WaterMeter}}"/> representing the asynchronous operation, returning the list of water meters with associated clients.</returns>
        Task<List<WaterMeter>> GetWaterMeterWithClients();

        /// <summary>
        /// Asynchronously retrieves a water meter along with its associated city and country.
        /// </summary>
        /// <param name="waterMeterId">The ID of the water meter to retrieve.</param>
        /// <returns>A <see cref="Task{WaterMeter}"/> representing the asynchronous operation, returning the water meter with city and country information.</returns>
        Task<WaterMeter> GetWaterMeterWithCityAndCountryAsync(int waterMeterId);

        /// <summary>
        ///  Asynchronously retrieves a water meter along with its associated client and locality.
        /// </summary>
        /// <param name="waterMeterId">The ID of the water meter to retrieve.</param>
        /// <returns>A <see cref="Task{WaterMeter}"/> representing the asynchronous operation, returning the water meter with client and locality information.</returns>
        Task<WaterMeter> GetClientAndLocalityWaterMeterAsync(int waterMeterId);

        /// <summary>
        /// Asynchronously updates a water meter service.
        /// </summary>
        /// <param name="waterMeter">The <see cref="WaterMeterService"/> to update.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task UpdateWaterServiceAsync(WaterMeterService waterMeter);

        /// <summary>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SelectListItem}"/> representing water meters for dropdown selection.</returns>
        IEnumerable<SelectListItem> GetComboWaterMeter();

        /// <summary>
        /// Asynchronously retrieves a water meter along with its associated consumptions.
        /// </summary>
        /// <param name="waterMeterId">The ID of the water meter to retrieve.</param>
        /// <returns>A <see cref="Task{WaterMeter}"/> representing the asynchronous operation, returning the water meter with its associated consumptions.</returns>
        Task<WaterMeter?> GetWaterMeterWithConsumptionsAsync(int waterMeterId);

        Task AddRequestWaterMeterAsync(RequestWaterMeter request);

        Task<IEnumerable<WaterMeter>> GetWaterMetersWithConsumptionsByClientAsync(int id);

  
    }
}
