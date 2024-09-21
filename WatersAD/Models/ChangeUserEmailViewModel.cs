using WatersAD.Data.Entities;

namespace WatersAD.Models
{
    public class ChangeUserEmailViewModel
    {
        
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public string Email { get; set; }

        public string OldEmail { get; set; }
    }
}
