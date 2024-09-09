using WatersAD.Data.Entities;
using WatersAD.Enum;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public interface IConverterHelper
    {
        User ToUser(RegisterNewUserViewModel model, string path);

        RegisterNewUserViewModel ToRegisterNewUserViewModel(User user);

        User ToUser(ChangeUserViewModel model, string path);

        UserType ConvertRoleToUserType(string roleName);
    }
}
