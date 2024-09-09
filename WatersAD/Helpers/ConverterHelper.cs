using WatersAD.Data.Entities;
using WatersAD.Enum;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public RegisterNewUserViewModel ToRegisterNewUserViewModel(User user)
        {
            throw new NotImplementedException();
        }

        public User ToUser(RegisterNewUserViewModel model, string path)
        {
            UserType userType = ConvertRoleToUserType(model.SelectedRole);

            return new User
            {
                
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Username,
                UserName = model.Username,
                Address = model.Address,
                ImageUrl = path,
                UserType = userType,

            };
        }

        public User ToUser(ChangeUserViewModel model, string path)
        {
            return new User
            {

                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageUrl = path,

            };
        }

        public UserType ConvertRoleToUserType(string roleName)
        {
            switch (roleName)
            {
                case "Admin":
                    return UserType.Admin;
                case "Customer":
                    return UserType.Customer;
                case "Employee":
                    return UserType.Employee;
                default:
                    throw new ArgumentException($"Role inválida: {roleName}");
            }
        }
    }
}
