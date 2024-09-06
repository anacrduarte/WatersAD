using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        public RegisterNewUserViewModel ToRegisterNewUserViewModel(User user)
        {
            throw new NotImplementedException();
        }

        public User ToUser(RegisterNewUserViewModel model, string path, bool isNew)
        {
            return new User
            {
                
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Username,
                UserName = model.Username,
                Address = model.Address,
                ImageUrl = path,

            };
        }

        public User ToUser(ChangeUserViewModel model, string path, bool isNew)
        {
            return new User
            {

                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageUrl = path,

            };
        }
    }
}
