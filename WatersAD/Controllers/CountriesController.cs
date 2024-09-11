using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class CountriesController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IFlashMessage _flashMessage;

        public CountriesController(ICountryRepository countryRepository, IFlashMessage flashMessage)
        {
            _countryRepository = countryRepository;
            _flashMessage = flashMessage;
        }

        // GET: Countries
        public IActionResult Index()
        {
            return View(_countryRepository.GetCountriesWithCities());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetCountryWithCitiesAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }


        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _countryRepository.CreateAsync(country);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    _flashMessage.Danger("This country already exists!");
                }

                return View(country);
            }

            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.UpdateAsync(country);
                return RedirectToAction(nameof(Index));
            }

            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }
            try
            {
                await _countryRepository.DeleteAsync(country);

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                _flashMessage.Warning($"{country.Name} in use!!");
                _flashMessage.Warning($"{country.Name} It cannot be deleted since there are orders that contain the product.</br></br>" +
                    $"Try deleting all the orders that are using it, " +
                    $"and try deleting it again.");

                return RedirectToAction(nameof(Index));
            }

        }

        // GET: Countries/DetailsCity/5
        public async Task<IActionResult> DetailsCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _countryRepository.GetCitiesWithLocalitiesAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // GET: Countries/AddCity
        public async Task<IActionResult> AddCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var country = await _countryRepository.GetByIdAsync(id.Value);
            if (country == null)
            {
                return NotFound();
            }

            var model = new CityViewModel { CountryId = country.Id };
            return View(model);
        }

        // POST: Countries/AddCity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(CityViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _countryRepository.AddCityAsync(model);
                    return RedirectToAction("Details", new { id = model.CountryId });
                }
                catch (Exception)
                {

                    _flashMessage.Danger("Esta cidade já existe!");
                }
                return this.View(model);
            }

            return this.View(model);
        }

        // GET: Countries/EditCity
        public async Task<IActionResult> EditCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            var model = new CityViewModel { CityId = city.Id, Name = city.Name, CountryId = city.CountryId };
            return View(model);
        }

        // POST: Countries/EditCity
        [HttpPost]
        public async Task<IActionResult> EditCity(CityViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var city = await _countryRepository.GetCityAsync(model.CityId);
                if (city == null)
                {
                    return NotFound();
                }
                city.Name = model.Name;

                var countryId = await _countryRepository.UpdateCityAsync(city);
                if (countryId != 0)
                {
                    return this.RedirectToAction("Details", new { id = countryId });
                }
            }

            return this.View(model);
        }

        // GET: Countries/DeleteCity/5
        public async Task<IActionResult> DeleteCity(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            try
            {
                var countryId = await _countryRepository.DeleteCityAsync(city);
                return this.RedirectToAction("Details", new { id = countryId });
            }
            catch (DbUpdateException)
            {

                _flashMessage.Warning($"{city.Name} in use!!");
                _flashMessage.Warning($"{city.Name} It cannot be deleted since there are orders that contain the product.</br></br>" +
                    $"Try deleting all the orders that are using it, " +
                    $"and try deleting it again.");

                var countryId = city.CountryId;

                //TODO alterar para view de erro ou para a do pais
                return this.RedirectToAction("Details", new { id = countryId });
            }
            
        }


        // GET: Countries/AddLocality
        public async Task<IActionResult> AddLocality(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var city = await _countryRepository.GetCityAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            var model = new LocalityViewModel { CityId = city.Id };
            return View(model);
        }

        // POST: Countries/AddLocality
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocality(LocalityViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _countryRepository.AddLocalityAsync(model);
                return RedirectToAction("DetailsCity", new { id = model.CityId });
            }

            return this.View(model);
        }

        // GET: Countries/EditLocality
        public async Task<IActionResult> EditLocality(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locality = await _countryRepository.GetLocalityAsync(id.Value);
            if (locality == null)
            {
                return NotFound();
            }

            var model = new LocalityViewModel { LocalityId = locality.Id, Name = locality.Name, PostalCode = locality.PostalCode, RemainPostalCode = locality.RemainPostalCode, CityId = locality.CityId };
            return View(model);
        }

        // POST: Countries/EditLocality
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLocality(LocalityViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);
                if (locality == null)
                {
                    return NotFound();
                }

                locality.Name = model.Name;
                locality.PostalCode = model.PostalCode;
                locality.RemainPostalCode = model.RemainPostalCode;

                var cityId = await _countryRepository.UpdateLocalityAsync(locality);
                if (cityId != 0)
                {
                    return this.RedirectToAction("DetailsCity", new { id = cityId });
                }
            }

            return this.View(model);
        }

   

        // GET: Countries/DeleteLocality/5
        public async Task<IActionResult> DeleteLocality(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locality = await _countryRepository.GetLocalityAsync(id.Value);
            if (locality == null)
            {
                return NotFound();
            }

            var cityId = await _countryRepository.DeleteLocalityAsync(locality);
            return this.RedirectToAction("DetailsCity", new { id = cityId });
        }

    }
}
