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
        /// Asynchronously retrieves a water meter along with its associated consumptions.
        /// </summary>
        /// <param name="waterMeterId">The ID of the water meter to retrieve.</param>
        /// <returns>A <see cref="Task{WaterMeter}"/> representing the asynchronous operation, returning the water meter with its associated consumptions.</returns>
        Task<WaterMeter> GetWaterMeterWithConsumptionsAsync(int waterMeterId);

        /// <summary>
        /// Asynchronously adds a new water meter request to the system.
        /// </summary>
        /// <param name="request">The <see cref="RequestWaterMeter"/> object representing the new request to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddRequestWaterMeterAsync(RequestWaterMeter request);

        /// <summary>
        /// Asynchronously retrieves all water meters associated with a specific client, including their consumption records.
        /// </summary>
        /// <param name="id">The unique identifier of the client.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="WaterMeter"/> objects, each associated with consumption data for the client.</returns>
        Task<IEnumerable<WaterMeter>> GetWaterMetersWithConsumptionsByClientAsync(int id);

        /// <summary>
        /// Asynchronously retrieves all water meters associated with a specific client.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="WaterMeter"/> objects for the specified client.</returns>
        Task<IEnumerable<WaterMeter>> GetWaterMeterClientAsync(int clientId);

        /// <summary>
        /// Asynchronously retrieves a specific water meter request based on the request ID.
        /// </summary>
        /// <param name="id">The unique identifier of the request.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="RequestWaterMeter"/> object for the specified request.</returns>
        Task<RequestWaterMeter> GetRequestWaterMeter(int id);

        /// <summary>
        /// Asynchronously retrieves a random water meter service.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="WaterMeterService"/> object randomly selected from the available services.</returns>
        Task<WaterMeterService> GetWaterServiceRandom();



    }
}
