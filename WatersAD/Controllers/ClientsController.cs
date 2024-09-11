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

        public ClientsController(
            IClientRepository clientRepository,
            IUserHelper userHelper,
            IFlashMessage flashMessage,
            ICountryRepository countryRepository,
            IConverterHelper converterHelper
            )
        {

            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
            _converterHelper = converterHelper;
        }

        // GET: Clients
        public IActionResult Index()
        {
            return View(_clientRepository.GetAllWithLocalities());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _clientRepository.GetByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
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
            };
            return View(model);
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                var client = _converterHelper.ToCliente(model);

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

            var client = await _clientRepository.GetByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }
            var model = await _converterHelper.ToClientViewModelAsync(client);

            return View(model);
        }



        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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

                    await _clientRepository.UpdateAsync(client);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Erro ao atualizar o cliente: " + ex.Message);
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

            var client = await _clientRepository.GetByIdAsync(id.Value);

            if (client == null)
            {
                return NotFound();
            }

         
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);

            if (client != null)
            {
                await _clientRepository.DeleteAsync(client);
            }


            return RedirectToAction(nameof(Index));
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
