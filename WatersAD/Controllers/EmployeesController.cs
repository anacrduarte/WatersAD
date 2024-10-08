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
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly ICountryRepository _countryRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;
        private readonly IUserHelper _userHelper;

        public EmployeesController(IEmployeeRepository employeeRepository,
            IUserHelper userHelper,
            IFlashMessage flashMessage,
            ICountryRepository countryRepository,
            IConverterHelper converterHelper, 
            IMailHelper mailHelper)
        {

            _employeeRepository = employeeRepository;
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
            _userHelper = userHelper;
        }

        // GET: Employees
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAllActive());
        }


        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            try
            {
                var employee = await _employeeRepository.GetEmployeeAndLocalityAndCityAsync(id.Value);

                if (employee == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                var user = await _userHelper.GetUserByEmailAsync(employee.Email);
                employee.User = user;
                var model = _converterHelper.ToEmployeeViewModel(employee);

                return View(model);
            }
            catch (Exception ex)
            {

                _flashMessage.Warning($"Erro! {ex.Message}");

                return RedirectToAction(nameof(Index));
            }
          

           
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees/Create
        public IActionResult Create()
        {
            try
            {
                var model = new EmployeeViewModel
                {
                    Countries = _countryRepository.GetComboCountries(),
                    Cities = _countryRepository.GetComboCities(0),
                    Localities = _countryRepository.GetComboLocalities(0)
                };
                return View(model);
            }
            catch (Exception ex)
            {

                _flashMessage.Warning($"Erro! {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                    var employee = _converterHelper.ToEmployee(model, locality);

                    employee.Locality = locality;

                    var associatedUser = await _userHelper.GetUserByEmailAsync(employee.Email);

                    if (associatedUser == null)
                    {

                        var newUser = new User
                        {
                            FirstName = employee.FirstName,
                            LastName = employee.LastName,
                            Email = employee.Email,
                            UserName = employee.Email,
                            UserType = Enum.UserType.Employee,
                            Address = employee.Address,
                            PhoneNumber = employee.PhoneNumber,
                        };


                        var result = await _userHelper.AddUserAsync(newUser, "123456");

                        if (!result.Succeeded)
                        {

                            _flashMessage.Danger("Erro ao criar utilizador.");
                            return View(employee);
                        }

                        await _userHelper.AddUserToRoleAsync(newUser, Enum.UserType.Employee.ToString());

                        Response response = await SendConfirmationEmailAsync(newUser, model.Email);
                        if (!response.IsSuccess)
                        {
                            _flashMessage.Danger("Erro ao enviar email de confirmação.");
                        }
                        else
                        {
                            _flashMessage.Info("Instruções de confirmação de email foram enviadas para o email do funcionário.");

                        }

                   

                    }
                    else
                    {

                        if (associatedUser.UserType != Enum.UserType.Employee)
                        {
                            associatedUser.UserType = Enum.UserType.Employee;
                            await _userHelper.UpdateUserAsync(associatedUser);
                        }

                        employee.User = associatedUser;

                        if (!await _userHelper.IsUserInRoleAsync(associatedUser, Enum.UserType.Employee.ToString()))
                        {
                            await _userHelper.AddUserToRoleAsync(associatedUser, Enum.UserType.Employee.ToString());
                        }
                    }

                    await _employeeRepository.CreateAsync(employee);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("Ocorreu um erro ao criar o funcionário: " + ex.Message);

                    return View(model);
                }
            }

            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);


        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            try
            {
                var employee = await _employeeRepository.GetEmployeeAndLocalityAndCityAsync(id.Value);
                if (employee == null)
                {
                    return new NotFoundViewResult("EmployeeNotFound");
                }

                var model = _converterHelper.ToEmployeeViewModel(employee);
                model.Countries = _countryRepository.GetComboCountries();
                model.Cities = _countryRepository.GetComboCities(model.CountryId);
                model.Localities = _countryRepository.GetComboLocalities(model.CityId);

                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("Ocorreu um erro " + ex.Message);

                return RedirectToAction(nameof(Index));
            }
        }
        

    // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel model)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    var employee = await _employeeRepository.GetByIdAsync(model.EmployeeId);

                    if (employee == null)
                    {
                        return new NotFoundViewResult("EmployeeNotFound");
                    }

                    employee.FirstName = model.FirstName;
                    employee.LastName = model.LastName;
                    employee.Address = model.Address;
                    employee.PhoneNumber = model.PhoneNumber;
                    employee.NIF = model.NIF;
                    employee.User = model.User;
                    employee.LocalityId = model.LocalityId;
                    employee.HouseNumber = model.HouseNumber;
                    employee.PostalCode = model.PostalCode;
                    employee.RemainPostalCode = model.RemainPostalCode;

                    await _employeeRepository.UpdateAsync(employee);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger("Ocorreu um erro ao atualizar o funcionário: " + ex.Message);
                    return RedirectToAction(nameof(Index));
                }
                
            }
            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);

        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id.Value);
                if (employee != null)
                {
                    employee.IsActive = false;

                    await _employeeRepository.UpdateAsync(employee);
                }


                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Danger("Ocorreu um erro ao apagar o funcionário: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Employees/FormerEmployees
        public IActionResult FormerEmployees()
        {
            return View(_employeeRepository.GetAllInactive());
        }

        public async Task<IActionResult> AddEmployeeAgain(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id.Value);
                if (employee != null)
                {
                    employee.IsActive = true;

                    await _employeeRepository.UpdateAsync(employee);
                }


                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                _flashMessage.Danger("Ocorreu um erro ao adicionar o funcionário: " + ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> UpdateEmployeeEmail(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);

            if (employee == null)
            {
                return new NotFoundViewResult("EmployeeNotFound");
            }

            var model = new ChangeUserEmailViewModel { EmployeeId = id.Value, OldEmail = employee.Email, FullName = employee.FullName };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeEmail(ChangeUserEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var employee = await _employeeRepository.GetByIdAsync(model.EmployeeId);

                    if (employee == null)
                    {
                        return new NotFoundViewResult("EmployeeNotFound");
                    }

                    var user = await _userHelper.GetUserByEmailAsync(employee.Email);

                    if (user == null)
                    {
                        _flashMessage.Warning("Email do funcionário não encontrado no sistema!");
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


                    employee.OldEmail = model.OldEmail;
                    employee.Email = model.Email;
                    await _employeeRepository.UpdateAsync(employee);

                    _flashMessage.Confirmation("E-mail do cliente atualizado com sucesso.");

                    Response response = await SendConfirmationEmailAsync(user, model.Email);
                    if (!response.IsSuccess)
                    {
                        _flashMessage.Danger("Erro ao enviar email de confirmação.");
                    }
                    else
                    {
                        _flashMessage.Info("Instruções de confirmação de email foram enviadas para o email do funcionário.");

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

            return await _mailHelper.SendMail($"{user.FirstName} {user.LastName}", email, subject, body);
        }
    }
}
