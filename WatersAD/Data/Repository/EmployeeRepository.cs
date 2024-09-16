using Microsoft.EntityFrameworkCore;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee> GetEmployeeAndLocalityAndCityAsync(int employeeId)
        {
            return await _context.Employees
                .Include(l => l.Locality)
                 .ThenInclude(ci => ci.City)
                 .ThenInclude(co => co.Country)
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public IEnumerable<Employee> GetAllActive()
        {
            return _context.Employees
                           .Where(e => e.IsActive)
                           .OrderBy(c => c.FirstName)
                           .ToList();
        }

        public IEnumerable<Employee> GetAllInactive()
        {
            return _context.Employees
                           .Where(e => !e.IsActive )
                           .OrderBy(c => c.FirstName)
                           .ToList();
        }
    }
}
