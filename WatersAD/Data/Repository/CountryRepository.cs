using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Data.Repository
{
    public class CountryRepository : GenericRepository<Country>, ICountryRepository
    {
        private readonly DataContext _context;

        public CountryRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddCityAsync(CityViewModel model)
        {
            var country = await this.GetCountryWithCitiesAsync(model.CountryId);
            if (country == null)
            {
                return;
            }

            country.Cities.Add(new City { Name = model.Name, CountryId = country.Id });
            _context.Countries.Update(country);
            await _context.SaveChangesAsync();
        }

        public async Task AddLocalityAsync(LocalityViewModel model)
        {
            var city = await this.GetCitiesWithLocalitiesAsync(model.CityId);
            if (city == null)
            {
                return;
            }
            city.Localities.Add(new Locality { Name = model.Name, PostalCode = model.PostalCode, RemainPostalCode = model.RemainPostalCode });
            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteCityAsync(City city)
        {
            var country = await _context.Countries
                .Where(c => c.Cities.Any(ci => ci.Id == city.Id))
                .FirstOrDefaultAsync();

            if (country == null)
            {
                return 0;
            }

            _context.Cities.Remove(city);
            await _context.SaveChangesAsync();
            return country.Id;
        }

        public async Task<int> DeleteLocalityAsync(Locality locality)
        {
            var city = await _context.Cities
                .Where(c =>c.Localities.Any(l => l.Id == locality.Id))
                .FirstOrDefaultAsync();

            if(city == null)
            {
                return 0;
            }

            _context.Localities.Remove(locality);
            await _context.SaveChangesAsync();
            return city.Id;
        }

        public IQueryable GetCitiesWithLocalities()
        {
            return _context.Cities
              .Include(c => c.Localities)
              .OrderBy(c => c.Name);
        }

        public async Task<City> GetCitiesWithLocalitiesAsync(int id)
        {
            return await _context.Cities
             .Include(c => c.Localities)
             .Where(c => c.Id == id)
             .FirstOrDefaultAsync();
        }

        public async Task<City> GetCityAsync(int id)
        {
            return await _context.Cities.FindAsync(id);
        }

       
        public IQueryable GetCountriesWithCities()
        {
            return _context.Countries
                .Include(c => c.Cities)
                .OrderBy(c => c.Name);
        }

        public async Task<Country> GetCountryAsync(City city)
        {

            return await _context.Countries
                .Where(c => c.Cities.Any(ci => ci.Id == city.Id))
                .FirstOrDefaultAsync();
        }

        public async Task<Country> GetCountryWithCitiesAsync(int id)
        {

            return await _context.Countries
                .Include(c => c.Cities)
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Locality> GetLocalityAsync(int id)
        {
            return await _context.Localities.FindAsync(id);
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            var country = await _context.Countries
                .Where(c => c.Cities.Any(ci => ci.Id == city.Id)).FirstOrDefaultAsync();

            if (country == null)
            {
                return 0;
            }

            _context.Cities.Update(city);
            await _context.SaveChangesAsync();
            return country.Id;

        }

        public async Task<int> UpdateLocalityAsync(Locality locality)
        {
            var city = await _context.Cities
                .Where(c => c.Localities.Any(l => l.Id == locality.Id)).FirstOrDefaultAsync();

            if(city == null)
            {
                return 0;
            }
            _context.Localities.Update(locality);
            await _context.SaveChangesAsync();
            return city.Id;
        }

    
        public IEnumerable<SelectListItem> GetComboCities(int countryId)
        {
            var country = _context.Countries.Find(countryId);

            var list = new List<SelectListItem>();

            if (country != null)
            {
                list = _context.Cities.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString(),

                }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a City...)",
                    Value = "0",
                });

            }

            return list;

        }

        public IEnumerable<SelectListItem> GetComboCountries()
        {
            var list = _context.Countries.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString(),

            }).OrderBy(l => l.Text).ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "(Select a Country...)",
                Value = "0",
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboLocalities(int cityId)
        {
            var city = _context.Cities.Find(cityId);

            var list = new List<SelectListItem>();
            if (city != null)
            {
                list = _context.Localities
                 .Where(l => l.CityId == cityId)
                 .Select(c => new SelectListItem
                 {
                     Text = c.Name,
                     Value = c.Id.ToString(),

                 }).OrderBy(l => l.Text).ToList();

                list.Insert(0, new SelectListItem
                {
                    Text = "(Select a locality...)",
                    Value = "0",
                });
            }


            return list;
        }

    }

}
