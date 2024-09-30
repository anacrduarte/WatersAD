using Microsoft.AspNetCore.Mvc.Rendering;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        /// <summary>
        /// Retrieves all clients, including their associated localities and water meters.
        /// </summary>
        /// <returns>An IEnumerable of clients with their localities and water meters.</returns>
        IEnumerable<Client> GetAllWithLocalitiesAndWaterMeter();

        /// <summary>
        /// Retrieves a specific client by their ID, along with their associated locality and city.
        /// </summary>
        /// <param name="clientId">The ID of the client to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation, with the client and associated locality and city data.</returns>
        Task<Client> GetClientAndLocalityAndCityAsync(int clientId);

        /// <summary>
        /// A Task representing the asynchronous operation, with the client and associated locality and city data.
        /// </summary>
        /// <param name="clientId">The ID of the client to retrieve.</param>
        /// <returns>A Task representing the asynchronous operation, with the client and their water meters.</returns>
        Task<Client> GetClientWithWaterMeter(int clientId);

        /// <summary>
        /// Retrieves all inactive clients, including their associated localities and water meters.
        /// </summary>
        /// <returns>An IEnumerable of inactive clients with their localities and water meters.</returns>
        IEnumerable<Client> GetAllWithLocalitiesAndWaterMeterInactive();

        /// <summary>
        /// Retrieves a list of clients to be used in dropdown selection controls.
        /// </summary>
        /// <returns>An IEnumerable of SelectListItem objects representing clients.</returns>
        IEnumerable<SelectListItem> GetComboClients();

        Task<Client> GetClientByUserEmailAsync(string email);


    }
}
