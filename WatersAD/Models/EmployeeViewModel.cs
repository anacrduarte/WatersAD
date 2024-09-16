using WatersAD.Data.Entities;
using WatersAD.Helpers;

namespace WatersAD.Models
{
    public class EmployeeViewModel : BasicInformationHelper
    {
        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }
    }
}
