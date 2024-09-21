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
        private readonly IConsumptionRepository _consumptionRepository;
        private readonly IMailHelper _mailHelper;

        public ClientsController(
            IClientRepository clientRepository,
            IUserHelper userHelper,
            IFlashMessage flashMessage,
            ICountryRepository countryRepository,
            IConverterHelper converterHelper,
            IWaterMeterRepository waterMeterRepository,
            IConsumptionRepository consumptionRepository,
            IMailHelper mailHelper
            )
        {

            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
            _converterHelper = converterHelper;
            _waterMeterRepository = waterMeterRepository;
            _consumptionRepository = consumptionRepository;
            _mailHelper = mailHelper;
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
                return new NotFoundViewResult("ClientNotFound");
            }

            var client = await _clientRepository.GetClientAndLocalityAndCityAsync(id.Value);

            if (client == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            var model = _converterHelper.ToClientViewModel(client);
          

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Clients/Create
        public IActionResult Create()
        {

            try
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
            catch (Exception ex)
            {

                _flashMessage.Danger("Ocorreu um erro ao carregar as localidades: " + ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
              
                try
                {
                    var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);
                    if (locality == null)
                    {
                        _flashMessage.Danger("Localidade inválida.");
                        return View(model);
                    }

                    var client = _converterHelper.ToCliente(model, locality);

                    client.Locality = locality;

                    var associatedUser = await _userHelper.GetUserByEmailAsync(client.Email);

                    if (associatedUser == null)
                    {
                        
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
                            return View(model);
                        }


                        await _userHelper.AddUserToRoleAsync(newUser, Enum.UserType.Customer.ToString());

                        Response response = await SendConfirmationEmailAsync(newUser, client.Email);


                        if (!response.IsSuccess)
                        {
                            _flashMessage.Danger("Erro ao enviar email de confirmação.");
                        }
                        else
                        {
                            _flashMessage.Info("Instruções de confirmação de email foram enviadas para o email do cliente.");
                            client.User = newUser;
                        }


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
                catch (Exception ex)
                {
                    _flashMessage.Danger("Ocorreu um erro ao criar o cliente: " + ex.Message);
                    
                    return View(model);
                }
            }

            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);
           
        }



        // GET: Clients/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            var client = await _clientRepository.GetClientAndLocalityAndCityAsync(id.Value);

            if (client == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }
            try
            {
                var model = _converterHelper.ToClientViewModel(client);
                model.Countries = _countryRepository.GetComboCountries();
                model.Cities = _countryRepository.GetComboCities(model.CountryId);
                model.Localities = _countryRepository.GetComboLocalities(model.CityId);

                return View(model);
            }
            catch (Exception ex)
            {

                _flashMessage.Danger("Ocorreu um erro ao carregar as localidades: " + ex.Message);

                return RedirectToAction(nameof(Index));
            }
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
                        return new NotFoundViewResult("ClientNotFound");
                    }

                    client.FirstName = model.FirstName;
                    client.LastName = model.LastName;
                    client.Address = model.Address;
                    client.PhoneNumber = model.PhoneNumber;
                    client.NIF = model.NIF;
                    client.User = model.User;
                    client.LocalityId = model.LocalityId;
                    client.HouseNumber = model.HouseNumber;
                    client.PostalCode = model.PostalCode;
                    client.RemainPostalCode = model.RemainPostalCode;

                    await _clientRepository.UpdateAsync(client);
                    _flashMessage.Confirmation("Cliente atualizado com sucesso!");

                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"Erro a actualizar o cliente " + ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(nameof(Index));
        }

        // GET: Clients/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }
            try
            {
               
                var client = await _clientRepository.GetClientWithWaterMeter(id.Value);

                if (client == null)
                {
                    return new NotFoundViewResult("ClientNotFound");
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
            catch (Exception ex)
            {
                _flashMessage.Danger($"Erro ao desativar o cliente: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }

            
        }


        [Authorize(Roles = "Admin")]
        // GET: Clients/AddWaterMeterToClient
        public async Task<IActionResult> AddWaterMeterToClient(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            try
            {
                var client = await _clientRepository.GetClientAndLocalityAndCityAsync(id.Value);

                if (client == null)
                {
                    return new NotFoundViewResult("ClientNotFound");
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

                if (!model.Countries.Any() || !model.Cities.Any() || !model.Localities.Any())
                {
                    _flashMessage.Warning("Não foi possível carregar as listas de Países, Cidades ou Localidades.");
                    return RedirectToAction(nameof(Index));
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Erro ao carregar os dados do cliente: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }

            
        }

        // POST: Clients/AddWaterMeterToClient

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> AddWaterMeterToClient(WaterMeterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = await _clientRepository.GetClientWithWaterMeter(model.ClientId);


                    if (client == null)
                    {
                        return new NotFoundViewResult("ClientNotFound");
                    }
                    client.WaterMeters ??= new List<WaterMeter>();
                  
                    var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                    if (locality == null)
                    {
                        _flashMessage.Warning("Localidade não encontrada.");
                        return View(model);
                    }

                    var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(model.WaterMeterServicesId);


                  
                    if (waterMeterService == null)
                    {
                        _flashMessage.Warning("Contador não encontrado.");
                        return View(model);
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
                        Consumptions = new List<Consumption>(),
                    };

                    await _waterMeterRepository.CreateAsync(waterMeter);

                    var consumption = new Consumption
                    {
                        ConsumptionDate = DateTime.UtcNow,
                        RegistrationDate = DateTime.UtcNow,
                        ConsumptionValue = 0,
                        WaterMeter = waterMeter,
                    };

                    await _consumptionRepository.CreateAsync(consumption);

                    waterMeter.Consumptions.Add(consumption);
                    waterMeter.WaterMeterService = waterMeterService;
                    waterMeter.Locality = locality;
                    waterMeter.Client = client;


                    client.WaterMeters.Add(waterMeter);

                    await _clientRepository.UpdateAsync(client);

                    _flashMessage.Confirmation("Contador adicionado com sucesso");

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    _flashMessage.Danger($"Erro ao atualizar o banco de dados: {dbEx.Message}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"Erro ao processar a solicitação: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }

            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);
        }

        
         
        public IActionResult FormerClients()
        {
            return View(_clientRepository.GetAllWithLocalitiesAndWaterMeterInactive());
        }

        public async Task<IActionResult> AddClientAgain(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            try
            {
                var client = await _clientRepository.GetByIdAsync(id.Value);

                if (client == null)
                {
                    return new NotFoundViewResult("ClientNotFound");
                }

                if (client.IsActive)
                {
                    _flashMessage.Info("O cliente já está ativo.");
                    return RedirectToAction(nameof(FormerClients));
                }
                
                
                    client.IsActive = true;

                    await _clientRepository.UpdateAsync(client);


                _flashMessage.Confirmation("Cliente reativado com sucesso.");
                return RedirectToAction(nameof(FormerClients));
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Erro ao reativar o cliente: {ex.Message}");
                return RedirectToAction(nameof(FormerClients));
            }
        }

        public async Task<IActionResult> UpdateClientEmail(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);

            if (client == null)
            {
                return new NotFoundViewResult("ClientNotFound");
            }

            var model = new ChangeUserEmailViewModel { ClientId = id.Value, OldEmail = client.Email };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateClientEmail(ChangeUserEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var client = await _clientRepository.GetByIdAsync(model.ClientId);

                    if (client == null)
                    {
                        return new NotFoundViewResult("ClientNotFound");
                    }

                    var user = await _userHelper.GetUserByEmailAsync(client.Email);

                    if (user == null)
                    {
                        _flashMessage.Warning("Email do cliente não encontrado no sistema!");
                        return View(model);
                    }


                    var associatedUser = await _userHelper.GetUserByEmailAsync(model.Email);

                    if (associatedUser != null && associatedUser.Id != user.Id)
                    {
                        _flashMessage.Danger("O novo email já está associado a outro utilizador.");
                        return View(model);
                    }

                    user.Email = model.Email;
                    user.UserName = model.Email;
                    var updateUserResult = await _userHelper.UpdateUserAsync(user);

                    if (!updateUserResult.Succeeded)
                    {
                        _flashMessage.Danger("Erro ao atualizar o e-mail do usuário.");
                        return View(model);
                    }


                    client.OldEmail = model.OldEmail;
                    client.Email = model.Email;
                    await _clientRepository.UpdateAsync(client);

                    _flashMessage.Confirmation("E-mail do cliente atualizado com sucesso.");

                    Response response = await SendConfirmationEmailAsync(user, model.Email);
                    if (!response.IsSuccess)
                    {
                        _flashMessage.Danger("Erro ao enviar email de confirmação.");
                    }
                    else
                    {
                        _flashMessage.Info("Instruções de confirmação de email foram enviadas para o email do cliente.");

                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"Erro ao processar a solicitação: {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }
            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);
        }

        private async Task<Response> SendConfirmationEmailAsync(User user, string email)
        {
            string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);

            string? tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userid = user.Id,
                token = myToken
            }, protocol: HttpContext.Request.Scheme);

            string subject = "Waters AD - Confirmação de Email";
            string body = $"<h1>Waters AD - Confirmação de Email</h1>" +
                          $"Clique no link para confirmar seu email e entrar como utilizador:" +
                          $"<p><a href = \"{tokenLink}\">Confirmar Email</a></p>";

            return _mailHelper.SendMail($"{user.FirstName} {user.LastName}", email, subject, body);
        }



    }
}
