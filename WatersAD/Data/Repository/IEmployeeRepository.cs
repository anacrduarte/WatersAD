using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        /// <summary>
        /// Asynchronously retrieves an employee along with the associated locality and city data.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to retrieve.</param>
        /// <returns>A <see cref="Task{Employee}"/> representing the asynchronous operation, which upon completion returns the employee along with locality and city information.</returns>
        Task<Employee> GetEmployeeAndLocalityAndCityAsync(int employeeId);

        /// <summary>
        /// Retrieves all active employees.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Employee}"/> of active employees.</returns>
        IEnumerable<Employee> GetAllActive();

        /// <summary>
        /// Retrieves all inactive employees.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Employee}"/> of inactive employees.</returns>
        IEnumerable<Employee> GetAllInactive();
    }
}
