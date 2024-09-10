using Microsoft.AspNetCore.Mvc.Rendering;
using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Data.Repository
{
    public interface ICountryRepository : IGenericRepository<Country>
    {
        IQueryable GetCountriesWithCities();

        IQueryable GetCitiesWithLocalities();

        Task<Country> GetCountryWithCitiesAsync(int id);

        Task<City> GetCitiesWithLocalitiesAsync(int id);

        Task<City> GetCityAsync(int id);

        Task<Locality> GetLocalityAsync(int id);

        Task AddCityAsync(CityViewModel model);

        Task AddLocalityAsync(LocalityViewModel model);

        Task<int> UpdateCityAsync(City city);

        Task<int> UpdateLocalityAsync(Locality locality);

        Task<int> DeleteCityAsync(City city);

        Task<int> DeleteLocalityAsync(Locality locality);

        IEnumerable<SelectListItem> GetComboCountries();

        IEnumerable<SelectListItem> GetComboCities(int countryId);

        IEnumerable<SelectListItem> GetComboLocalities(int cityId);

        Task<Country> GetCountryAsync(City city);
    }
}
