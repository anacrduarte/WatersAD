using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public interface IConverterHelper
    {
        User ToUser(RegisterNewUserViewModel model, string path, bool isNew);

        RegisterNewUserViewModel ToRegisterNewUserViewModel(User user);

        User ToUser(ChangeUserViewModel model, string path, bool isNew);
    }
}
