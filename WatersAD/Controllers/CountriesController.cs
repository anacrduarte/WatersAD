﻿using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View(_countryRepository.GetCountriesWithCities());
        }

        // GET: Countries/Details/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
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
        [Authorize(Roles = "Admin")]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _countryRepository.UpdateAsync(country);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Erro ao processar a solicitação: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(country);
        }

        // GET: Countries/Delete/5
        [Authorize(Roles = "Admin")]
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
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    _flashMessage.Warning($"{country.Name} está em uso!!");
                    _flashMessage.Warning($"{country.Name} Não pode ser eliminado sem eliminar as cidades primeiro."); 
                     

                }
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");

                return RedirectToAction(nameof(Index));
            }

        }

        // GET: Countries/DetailsCity/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

            var model = new CityViewModel { CountryId = country.Id, CountryName = country.Name };
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
        [Authorize(Roles = "Admin")]
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
                try
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
                        return this.RedirectToAction("Details", new { id = model.CountryId });
                    }
                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Erro ao processar a solicitação: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }

            return this.View(model);
        }

        // GET: Countries/DeleteCity/5
        [Authorize(Roles = "Admin")]
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
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    _flashMessage.Warning($"{city.Name} está em uso!!");
                    _flashMessage.Warning($"{city.Name} Não pode ser eliminado sem eliminar as cidades primeiro.");


                }
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");

                var countryId = city.CountryId;

                return this.RedirectToAction("Details", new { id = countryId });
            }

        }


        // GET: Countries/AddLocality
        [Authorize(Roles = "Admin")]
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

            var model = new LocalityViewModel { CityId = city.Id, CityName = city.Name };
            return View(model);
        }

        // POST: Countries/AddLocality
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocality(LocalityViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _countryRepository.AddLocalityAsync(model);
                    return RedirectToAction("DetailsCity", new { id = model.CityId });
                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Erro ao processar a solicitação: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }

            return this.View(model);
        }

        // GET: Countries/EditLocality
        [Authorize(Roles = "Admin")]
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

            var model = new LocalityViewModel { LocalityId = locality.Id, Name = locality.Name, CityId = locality.CityId };
            return View(model);
        }

        // POST: Countries/EditLocality
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLocality(LocalityViewModel model)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);
                    if (locality == null)
                    {
                        return NotFound();
                    }

                    locality.Name = model.Name;


                    var cityId = await _countryRepository.UpdateLocalityAsync(locality);
                    if (cityId != 0)
                    {
                        return this.RedirectToAction("DetailsCity", new { id = cityId });
                    }
                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Erro ao processar a solicitação: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }

            return this.View(model);
        }



        // GET: Countries/DeleteLocality/5
        [Authorize(Roles = "Admin")]
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
            try
            {

                var cityId = await _countryRepository.DeleteLocalityAsync(locality);
                return this.RedirectToAction("DetailsCity", new { id = cityId });
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null && ex.InnerException.Message.Contains("DELETE"))
                {
                    _flashMessage.Warning($"{locality.Name} está em uso!!");
                    _flashMessage.Warning($"{locality.Name} Não pode ser eliminado sem eliminar todas as dependências.");


                }
                var cityId = locality.CityId;
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return this.RedirectToAction("DetailsCity", new { id = cityId });
            }
        }

        [HttpPost]
        [Route("Countries/GetCitiesAsync")]
        public async Task<JsonResult> GetCitiesAsync(int countryId)
        {
            var country = await _countryRepository.GetCountryWithCitiesAsync(countryId);

            //return Json(country.Cities.OrderBy(c => c.Name));
            var cities = country.Cities.Select(c => new
            {
                id = c.Id,
                name = c.Name
            }).OrderBy(c => c.name);

            return Json(cities);
        }

        [HttpPost]
        [Route("Countries/GetLocalitiesAsync")]
        public async Task<JsonResult> GetLocalitiesAsync(int cityId)
        {
            var city = await _countryRepository.GetCitiesWithLocalitiesAsync(cityId);

            //return Json(country.Cities.OrderBy(c => c.Name));
            var localities = city.Localities.Select(l => new
            {
                id = l.Id,
                name = l.Name
            }).OrderBy(l => l.name);

            return Json(localities);
        }

    }
}
