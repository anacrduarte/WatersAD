using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class WaterMetersController : Controller
    {
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly ICountryRepository _countryRepository;
        private readonly IClientRepository _clientRepository;

        public WaterMetersController(IWaterMeterRepository waterMeterRepository, IFlashMessage flashMessage, ICountryRepository countryRepository, IClientRepository clientRepository)
        {

            _waterMeterRepository = waterMeterRepository;
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
            _clientRepository = clientRepository;
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
                return NotFound();
            }

         

            var water = await _waterMeterRepository.GetClientAndLocalityWaterMeterAsync(id.Value);
            if (water == null)
            {
                return NotFound();
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

        // GET: WaterMeters/Create
        public IActionResult Create()
        {
            
            var model = new WaterMeterViewModel
            {
                Clients = _clientRepository.GetAll().Select(c=> new SelectListItem
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

        // POST: WaterMeters/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WaterMeterViewModel model, int id)
        {
            if (ModelState.IsValid)
            {
               

                var client = await _clientRepository.GetByIdAsync(model.SelectedClientId);

                

                if(client == null) 
                { 
                    return NotFound();
                }

                if(client.WaterMeters == null)
                {
                    client.WaterMeters = new List<WaterMeter>();
                }

                var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(id);


                if (waterMeterService == null)
                {
                    return NotFound();
                }

                waterMeterService.Available = false;

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
                };

                await _waterMeterRepository.CreateAsync(waterMeter);


                waterMeter.WaterMeterService = waterMeterService;
                waterMeter.Locality = locality;
                waterMeter.Client = client;



                client.WaterMeters.Add(waterMeter);

                await _clientRepository.UpdateAsync(client);

                return RedirectToAction(nameof(DetailsWaterServices));
            }

            return View(model);
        }

        // GET: WaterMeters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var waterMeter = await _waterMeterRepository.GetWaterMeterWithCityAndCountryAsync(id.Value);
          
            if (waterMeter == null)
            {
                return NotFound();
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

                    var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                    waterMeter.Address = model.Address;
                    waterMeter.HouseNumber = model.HouseNumber;
                    waterMeter.LocalityId = model.LocalityId;
                    waterMeter.Locality = locality;

                    await _waterMeterRepository.UpdateAsync(waterMeter);
                }
                catch (DbUpdateConcurrencyException)
                {


                    throw;

                }
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: WaterMeters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var waterMeter = await _waterMeterRepository.GetByIdAsync(id.Value);

            if (waterMeter == null)
            {
                return NotFound();
            }

            return View(waterMeter);
        }

        // POST: WaterMeters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var waterMeter = await _waterMeterRepository.GetByIdAsync(id);
            if (waterMeter != null)
            {
                waterMeter.IsActive = false;
                await _waterMeterRepository.UpdateAsync(waterMeter);
            }


            return RedirectToAction(nameof(Index));
        }



     
        public async Task<IActionResult> DeleteWaterMeterService(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(id.Value);

            if (waterMeterService == null)
            {
                return NotFound();
               
            }
            await _waterMeterRepository.DeleteWaterServiceAsync(waterMeterService);

            return RedirectToAction(nameof(DetailsWaterServices));
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
                    // Gerar um SerialNumber aleatório ou automático
                    var serialNumber = Guid.NewGuid().ToString().Substring(0, 8); // Serial gerado automaticamente

                    var waterMeter = new WaterMeterService
                    {
                        SerialNumber = serialNumber
                    };

                    waterMeters.Add(waterMeter);
                }

                foreach (var waterMeter in waterMeters)
                {
                    await _waterMeterRepository.AddWaterMeterAsync(waterMeter);
                }

                return RedirectToAction(nameof(DetailsWaterServices));
            }

            return View(meterService);
        }
    }
}
