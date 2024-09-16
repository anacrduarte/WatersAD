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
    public class EmployeesController : Controller
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly ICountryRepository _countryRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;

        public EmployeesController(IEmployeeRepository employeeRepository,
            IUserHelper userHelper,
            IFlashMessage flashMessage,
            ICountryRepository countryRepository,
            IConverterHelper converterHelper)
        {

            _employeeRepository = employeeRepository;
            _flashMessage = flashMessage;
            _countryRepository = countryRepository;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
        }

        // GET: Employees
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAllActive());
        }

        public IActionResult GetAllEmployees()
        {
            return View(_employeeRepository.GetAll());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeAndLocalityAndCityAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEmployeeViewModel(employee);

            return View(model);
          

           
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees/Create
        public IActionResult Create()
        {
            var model = new EmployeeViewModel
            {
                Countries = _countryRepository.GetComboCountries(),
                Cities = _countryRepository.GetComboCities(0),
                Localities = _countryRepository.GetComboLocalities(0)
            };
            return View(model);
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var locality = await _countryRepository.GetLocalityAsync(model.LocalityId);

                var employee = _converterHelper.ToEmployee(model, locality);

                employee.Locality = locality;

                var associatedUser = await _userHelper.GetUserByEmailAsync(employee.Email);

                if (associatedUser == null)
                {
                    //TODO enviar notificação ao funcionario ou email para activar conta
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


                    employee.User = newUser;
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

            return View(model);


        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetEmployeeAndLocalityAndCityAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToEmployeeViewModel(employee);
            model.Countries = _countryRepository.GetComboCountries();
            model.Cities = _countryRepository.GetComboCities(model.CountryId);
            model.Localities = _countryRepository.GetComboLocalities(model.CityId);

            return View(model);
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
                        return NotFound();
                    }

                    employee.FirstName = model.FirstName;
                    employee.LastName = model.LastName;
                    employee.Address = model.Address;
                    employee.Email = model.Email;
                    employee.PhoneNumber = model.PhoneNumber;
                    employee.NIF = model.NIF;
                    employee.User = model.User;
                    employee.LocalityId = model.LocalityId;
                    employee.HouseNumber = model.HouseNumber;
                    employee.PostalCode = model.PostalCode;
                    employee.RemainPostalCode = model.RemainPostalCode;

                    await _employeeRepository.UpdateAsync(employee);
                }
                catch (Exception)
                {
                    _flashMessage.Danger("Erro ao actualizar!");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);

        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee != null)
            {
                employee.IsActive = false;

                await _employeeRepository.UpdateAsync(employee);
            }


            return RedirectToAction(nameof(Index));
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
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee != null)
            {
                employee.IsActive = true;

                await _employeeRepository.UpdateAsync(employee);
            }


            return RedirectToAction(nameof(Index));
        }
    }
}
