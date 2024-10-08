using Microsoft.AspNetCore.Mvc.Rendering;
using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Data.Repository
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        /// <summary>
        /// Retrieves all countries along with their associated cities.
        /// </summary>
        /// <returns>An IQueryable of countries with their cities. </returns>
        IQueryable GetCountriesWithCities();


        /// <summary>
        /// Retrieves all cities along with their associated localities.
        /// </summary>
        /// <returns>An IQueryable of cities with their localities</returns>
        IQueryable GetCitiesWithLocalities();

        /// <summary>
        /// Retrieves a country by its ID, including its associated cities.
        /// </summary>
        /// <param name="id">The ID of the country.</param>
        /// <returns> The task result contains the country with its cities.</returns>
        Task<Country> GetCountryWithCitiesAsync(int id);

        /// <summary>
        /// Retrieves a city by its ID, including its associated localities.
        /// </summary>
        /// <param name="id">The ID of the city.</param>
        /// <returns>The task result contains the city with its localities</returns>
        Task<City> GetCitiesWithLocalitiesAsync(int id);

        /// <summary>
        /// Retrieves a city by its ID.
        /// </summary>
        /// <param name="id">The ID of the city.</param>
        /// <returns>The task result contains the city.</returns>
        Task<City> GetCityAsync(int id);

        /// <summary>
        /// Retrieves a locality by its ID.
        /// </summary>
        /// <param name="id">The ID of the locality.</param>
        /// <returns>The task result contains the locality.</returns>
        Task<Locality> GetLocalityAsync(int id);

        /// <summary>
        /// Retrieves the country associated with a specific city.
        /// </summary>
        /// <param name="city">The city object for which the country is to be retrieved</param>
        /// <returns> The task result contains the country associated with the specified city.</returns>
        Task<Country> GetCountryAsync(City city);

        /// <summary>
        ///  Adds a new city based on the provided view model.
        /// </summary>
        /// <param name="model">The view model containing the data to create the new city.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddCityAsync(CityViewModel model);

        /// <summary>
        /// Adds a new locality based on the provided view model.
        /// </summary>
        /// <param name="model">The view model containing the data to create the new locality.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddLocalityAsync(LocalityViewModel model);

        /// <summary>
        /// Updates an existing city.
        /// </summary>
        /// <param name="city">The city object with updated information.</param>
        /// <returns>CountryId</returns>
        Task<int> UpdateCityAsync(City city);

        /// <summary>
        /// Updates an existing locality.
        /// </summary>
        /// <param name="locality">The locality object with updated information</param>
        /// <returns>CityId</returns>
        Task<int> UpdateLocalityAsync(Locality locality);

        /// <summary>
        /// Deletes a city.
        /// </summary>
        /// <param name="city">The city object to be deleted.</param>
        /// <returns>CountryId</returns>
        Task<int> DeleteCityAsync(City city);

        /// <summary>
        /// Deletes a locality.
        /// </summary>
        /// <param name="locality">The locality object to be deleted.</param>
        /// <returns>CityId</returns>
        Task<int> DeleteLocalityAsync(Locality locality);

        /// <summary>
        ///  Retrieves a collection of countries formatted for use in a dropdown list.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{SelectListItem}"/> of countries for selection in a user interface</returns>
        IEnumerable<SelectListItem> GetComboCountries();

        /// <summary>
        /// Retrieves a collection of cities formatted for use in a dropdown list, based on the specified country ID.
        /// </summary>
        /// <param name="countryId">The ID of the country whose cities are to be retrieved.</param>
        /// <returns>An <see cref="IEnumerable{SelectListItem}"/> of cities for selection in a user interface.</returns>
        IEnumerable<SelectListItem> GetComboCities(int countryId);

        /// <summary>
        /// Retrieves a collection of localities formatted for use in a dropdown list, based on the specified city ID.
        /// </summary>
        /// <param name="cityId">The ID of the city whose localities are to be retrieved.</param>
        /// <returns>An <see cref="IEnumerable{SelectListItem}"/> of localities for selection in a user interface.</returns>
        IEnumerable<SelectListItem> GetComboLocalities(int cityId);

    }
}
