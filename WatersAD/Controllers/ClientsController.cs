using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;

namespace WatersAD.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserHelper _userHelper;
        private readonly IFlashMessage _flashMessage;

        public ClientsController(
            IClientRepository clientRepository,
            IUserHelper userHelper,
            IFlashMessage flashMessage
            )
        {

            _clientRepository = clientRepository;
            _userHelper = userHelper;
            _flashMessage = flashMessage;
        }

        // GET: Clients
        public IActionResult Index()
        {
            return View(_clientRepository.GetAll().OrderBy(p => p.FirstName));
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
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {

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

            return View(client);

           
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

            return View(client);
        }



        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Client client)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    //TODO FALTA ALTERAR O EMAIL NO EDITAR
                    client.User = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                    await _clientRepository.UpdateAsync(client);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _clientRepository.ExistAsync(client.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(client);
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


    }
}
