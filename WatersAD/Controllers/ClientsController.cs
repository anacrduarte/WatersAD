using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;
        private readonly ICountryRepository _countryRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IWaterMeterRepository _waterMeterRepository;

        public ClientsController(
            IClientRepository clientRepository,
            IUserHelper userHelper,
            IFlashMessage flashMessage,
            ICountryRepository countryRepository,
            IConverterHelper converterHelper,
            IWaterMeterRepository waterMeterRepository
            )
        {

            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
            _converterHelper = converterHelper;
            _waterMeterRepository = waterMeterRepository;
        }

        // GET: Clients
        public IActionResult Index()
        {
            return View(_clientRepository.GetAllWithLocalitiesAndWaterMeter());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetClientAndLocalityAndCityAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToClientViewModel(client);
          

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Clients/Create
        public IActionResult Create()
        {
            var model = new ClientViewModel
            {
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(0),
                Localities = _countryRepository.GetComboLocalities(0),
                WaterMeterServices = _waterMeterRepository.GetComboWaterMeterServices(),
            };
            return View(model);
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                
                var client = _converterHelper.ToCliente(model, locality);

                client.Locality = locality;               

                var associatedUser = await _userHelper.GetUserByEmailAsync(client.Email);

                if (associatedUser == null)
                {
                    //TODO enviar notificação ao cliente ou email para activar conta
                    var newUser = new User
                    {
                        FirstName = client.FirstName,
                        LastName = client.LastName,
                        Email = client.Email,
                        UserName = client.Email,
                        UserType = Enum.UserType.Customer,
                        Address = client.Address,
                        PhoneNumber = client.PhoneNumber,
                        
                    };

                    
                    var result = await _userHelper.AddUserAsync(newUser, "123456"); 

                    if (!result.Succeeded)
                    {
                       
                        _flashMessage.Danger("Erro ao criar utilizador.");
                        return View(client);
                    }

                   
                    await _userHelper.AddUserToRoleAsync(newUser, Enum.UserType.Customer.ToString());

                    
                    client.User = newUser;
                }
                else
                {

                    if (associatedUser.UserType != Enum.UserType.Customer)
                    {
                        associatedUser.UserType = Enum.UserType.Customer;
                        await _userHelper.UpdateUserAsync(associatedUser);
                    }

                    client.User = associatedUser;

                    if (!await _userHelper.IsUserInRoleAsync(associatedUser, Enum.UserType.Customer.ToString()))
                    {
                        await _userHelper.AddUserToRoleAsync(associatedUser, Enum.UserType.Customer.ToString());
                    }
                }
                await _clientRepository.CreateAsync(client);
                

                return RedirectToAction(nameof(Index));
            }

            return View(model);

           
        }



        // GET: Clients/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetClientAndLocalityAndCityAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }
            var model = _converterHelper.ToClientViewModel(client);
            model.Countries = _countryRepository.GetComboCountries();
            model.Cities = _countryRepository.GetComboCities(model.CountryId);
            model.Localities = _countryRepository.GetComboLocalities(model.CityId);

            return View(model);
        }



        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientViewModel model)
        {

            if (ModelState.IsValid)
            {

               

                try
                {
                    var client = await _clientRepository.GetByIdAsync(model.ClientId);

                    if (client == null)
                    {
                        return NotFound();
                    }

                    client.FirstName = model.FirstName;
                    client.LastName = model.LastName;
                    client.Address = model.Address;
                    client.Email = model.Email;
                    client.PhoneNumber = model.PhoneNumber;
                    client.NIF = model.NIF;
                    client.User = model.User;
                    client.LocalityId = model.LocalityId;
                    client.HouseNumber = model.HouseNumber;
                    client.PostalCode = model.PostalCode;
                    client.RemainPostalCode = model.RemainPostalCode;

                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"Erro a actualizar o cliente " + ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Clients/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var client = await _clientRepository.GetClientWithWaterMeter(id.Value);

            if(client == null)
            { 
                return NotFound(); 
            }

            if (client.WaterMeters != null && client.WaterMeters.Any())
            {
                _flashMessage.Warning("Tem que desativar primeiro os contadores antes de remover o cliente");
                return RedirectToAction(nameof(Index));

            }
            client.IsActive = false;
            await _clientRepository.UpdateAsync(client);

            return RedirectToAction(nameof(Index));
        }

     



        [Authorize(Roles = "Admin")]
        // GET: Clients/AddWaterMeterToClient
        public async Task<IActionResult> AddWaterMeterToClient(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetClientAndLocalityAndCityAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            var model = new WaterMeterViewModel
            {
                ClientId = client.Id,
                Address = client.Address,
                HouseNumber = client.HouseNumber,
                PostalCode = client.PostalCode,
                RemainPostalCode = client.RemainPostalCode,
                LocalityId = client.Locality.Id,
                CityId = client.Locality.City.Id,
                CountryId = client.Locality.City.Country.Id,
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(client.Locality.City.Country.Id),
                Localities = _countryRepository.GetComboLocalities(client.Locality.City.Id),
                WaterMeterServices = _waterMeterRepository.GetComboWaterMeterServices(),
            };

            return View(model);
        }

        // POST: Clients/AddWaterMeterToClient

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddWaterMeterToClient(WaterMeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = await _clientRepository.GetClientWithWaterMeter(model.ClientId);


                if (client == null)
                {
                    return NotFound();
                }

                if (client.WaterMeters == null)
                {
                    client.WaterMeters = new List<WaterMeter>();
                }

                var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(model.WaterMeterServicesId);


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

                _flashMessage.Confirmation("Contador adicionado com sucesso");

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        
        // GET: 
        public IActionResult FormerClients()
        {
            return View(_clientRepository.GetAllWithLocalitiesAndWaterMeterInactive());
        }

        public async Task<IActionResult> AddClientAgain(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);
            if (client != null)
            {
                client.IsActive = true;

                await _clientRepository.UpdateAsync(client);
            }


            return RedirectToAction(nameof(FormerClients));
        }




        [HttpPost]
        [Route("Clients/GetCitiesAsync")]
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
        [Route("Clients/GetLocalitiesAsync")]
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
