using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;

namespace WatersAD.Controllers
{
    public class EmployeesController : Controller
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;

        public EmployeesController(IEmployeeRepository employeeRepository,
            IUserHelper userHelper,
            IFlashMessage flashMessage)
        {

            _employeeRepository = employeeRepository;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
        }

        // GET: Employees
        public IActionResult Index()
        {
            return View(_employeeRepository.GetAll().OrderBy(p => p.FirstName));
        }


        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {

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

            return View(employee);


        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Employee employee)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    //TODO falta aqui a informação do user se quiser meter
                    await _employeeRepository.UpdateAsync(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _employeeRepository.ExistAsync(employee.Id))
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
            return View(employee);

        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeRepository.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee != null)
            {
                await _employeeRepository.DeleteAsync(employee);
            }


            return RedirectToAction(nameof(Index));
        }


    }
}
