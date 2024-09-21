﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class WaterMetersController : Controller
    {
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly ICountryRepository _countryRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IConsumptionRepository _consumptionRepository;

        public WaterMetersController(IWaterMeterRepository waterMeterRepository, IFlashMessage flashMessage, ICountryRepository countryRepository, IClientRepository clientRepository,
            IConsumptionRepository consumptionRepository)
        {

            _waterMeterRepository = waterMeterRepository;
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
            _clientRepository = clientRepository;
            _consumptionRepository = consumptionRepository;
        }

        // GET: WaterMeters
        public async Task<IActionResult> Index()
        {
            return View( await _waterMeterRepository.GetWaterMeterWithClients());
        }

        // GET: WaterMetersServices
        public IActionResult DetailsWaterServices()
        {
            return View(_waterMeterRepository.GetWaterMeterServices());
        }
        // GET: WaterMeters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }


            try
            {

                var water = await _waterMeterRepository.GetClientAndLocalityWaterMeterAsync(id.Value);
                if (water == null)
                {
                    return new NotFoundViewResult("WaterMeterNotFound");
                }
                var model = new WaterMeterViewModel
                {
                    Address = water.Address,
                    HouseNumber = water.HouseNumber,
                    Client = water.Client,
                    Locality = water.Locality,
                    InstallationDate = water.InstallationDate,
                    Country = water.Locality.City.Country,
                    City = water.Locality.City,
                    WaterMeterService = water.WaterMeterService,


                };

                return View(model);
            }
            catch (Exception ex)
            {

                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: WaterMeters/Create
        public IActionResult Create()
        {

            try
            {
                var model = new WaterMeterViewModel
                {
                    Clients = _clientRepository.GetAll().Select(c => new SelectListItem
                    {
                        Text = c.FullName,
                        Value = c.Id.ToString(),
                    }),
                    Countries = _countryRepository.GetComboCountries(),
                    Cities = _countryRepository.GetComboCities(0),
                    Localities = _countryRepository.GetComboLocalities(0),

                };
                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: WaterMeters/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WaterMeterViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = await _clientRepository.GetByIdAsync(model.SelectedClientId);

                    if (client == null)
                    {
                        return new NotFoundViewResult("ClientNotFound");
                    }

                    client.WaterMeters ??= new List<WaterMeter>();
                    

                    var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                    if(locality == null)
                    {
                        _flashMessage.Warning("Localidade não encontrada.");
                        return View(model);
                    }

                    var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(id);


                    if (waterMeterService == null)
                    {
                        _flashMessage.Warning("Contador não encontrado.");
                        return View(model);
                    }

                    waterMeterService.Available = false;
                    //TODO ver se ao usar isto a lista de consumption ATENÇAO
                    await _waterMeterRepository.UpdateWaterServiceAsync(waterMeterService);

                    var waterMeter = new WaterMeter
                    {
                        ClientId = client.Id,
                        LocalityId = locality.Id,
                        WaterMeterServiceId = waterMeterService.Id,
                        Address = model.Address,
                        HouseNumber = model.HouseNumber,
                        InstallationDate = model.InstallationDate,
                        PostalCode = model.PostalCode,
                        RemainPostalCode = model.RemainPostalCode,
                        Consumptions = new List<Consumption>(),
                    };

                    await _waterMeterRepository.CreateAsync(waterMeter);

                    var consumption = new Consumption
                    {
                        ConsumptionDate = DateTime.UtcNow,
                        ConsumptionValue = 0,
                        WaterMeter = waterMeter,
                    };

                    await _consumptionRepository.CreateAsync(consumption);

                    waterMeter.Consumptions.Add(consumption);

                    await _waterMeterRepository.UpdateAsync(waterMeter);

                    waterMeter.WaterMeterService = waterMeterService;
                    waterMeter.Locality = locality;
                    waterMeter.Client = client;

                    client.WaterMeters.Add(waterMeter);

                    await _clientRepository.UpdateAsync(client);

                    return RedirectToAction(nameof(DetailsWaterServices));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"Ocorreu um erro. {ex.Message}");
                    return View(model);
                }
            }
            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);
        }

        // GET: WaterMeters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }
            try
            {
                var waterMeter = await _waterMeterRepository.GetWaterMeterWithCityAndCountryAsync(id.Value);

                if (waterMeter == null)
                {
                    return new NotFoundViewResult("WaterMeterNotFound");
                }

                var model = new WaterMeterViewModel
                {
                    Address = waterMeter.Address,
                    HouseNumber = waterMeter.HouseNumber,
                    LocalityId = waterMeter.Locality.Id,
                    CityId = waterMeter.Locality.City.Id,
                    CountryId = waterMeter.Locality.City.Country.Id,
                    Countries = _countryRepository.GetComboCountries(),
                    Cities = _countryRepository.GetComboCities(waterMeter.Locality.City.Country.Id),
                    Localities = _countryRepository.GetComboLocalities(waterMeter.Locality.City.Id),
                    PostalCode = waterMeter.PostalCode,
                    RemainPostalCode = waterMeter.RemainPostalCode,
                };

                return View(model);
            }
            catch (Exception ex)
            {

                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: WaterMeters/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(WaterMeterViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var waterMeter = await _waterMeterRepository.GetByIdAsync(model.Id);
                    if(waterMeter == null)
                    {
                        return new NotFoundViewResult("WaterMeterNotFound");
                    }

                    var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                    if (locality == null)
                    {
                        _flashMessage.Warning("Localidade não encontrada.");
                        return View(model);
                    }

                    waterMeter.Address = model.Address;
                    waterMeter.HouseNumber = model.HouseNumber;
                    waterMeter.LocalityId = model.LocalityId;
                    waterMeter.Locality = locality;

                    await _waterMeterRepository.UpdateAsync(waterMeter);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                    return RedirectToAction(nameof(Index));

                }
               
            }
            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);
        }

        // GET: WaterMeters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }

            try
            {
                var waterMeter = await _waterMeterRepository.GetByIdAsync(id.Value);

                if (waterMeter == null)
                {
                    return new NotFoundViewResult("WaterMeterNotFound");
                }

                return View(waterMeter);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: WaterMeters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var waterMeter = await _waterMeterRepository.GetByIdAsync(id);

                if (waterMeter == null)
                {
                    return new NotFoundViewResult("WaterMeterNotFound");
                }

                waterMeter.IsActive = false;
                await _waterMeterRepository.UpdateAsync(waterMeter);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }



     
        public async Task<IActionResult> DeleteWaterMeterService(int? id)
        {
            if(id == null)
            {
                return new NotFoundViewResult("WaterMeterNotFound");
            }

            try
            {
                var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(id.Value);

                if (waterMeterService == null)
                {
                    _flashMessage.Warning("Contador não encontrado.");
                    return RedirectToAction(nameof(Index));

                }
                await _waterMeterRepository.DeleteWaterServiceAsync(waterMeterService);

                return RedirectToAction(nameof(DetailsWaterServices));
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: WaterMeters/CreateWaterMeterService
        public IActionResult CreateWaterMeterService()
        {

            return View();
        }

        // POST: WaterMeters/CreateWaterMeterService

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWaterMeterService(WaterMeterService meterService)
        {
            if (ModelState.IsValid)
            {
                var waterMeters = new List<WaterMeterService>();

                for (int i = 0; i < meterService.Quantity; i++)
                {
                   
                    var serialNumber = Guid.NewGuid().ToString().Substring(0, 8); 

                    var waterMeter = new WaterMeterService
                    {
                        SerialNumber = serialNumber
                    };

                    waterMeters.Add(waterMeter);
                }

                foreach (var waterMeter in waterMeters)
                {
                    try
                    {
                        await _waterMeterRepository.AddWaterMeterAsync(waterMeter);
                    }
                    catch (Exception ex)
                    {
                        _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                        return RedirectToAction(nameof(Index));
                    }
                }
                _flashMessage.Info($"{meterService.Quantity} contadore(s) adicionado com sucesso.");
                return RedirectToAction(nameof(DetailsWaterServices));
            }

            return View(meterService);
        }
    }
}
