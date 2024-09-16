using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee> GetEmployeeAndLocalityAndCityAsync(int employeeId);

        IEnumerable<Employee> GetAllActive();

        IEnumerable<Employee> GetAllInactive();
    }
}
