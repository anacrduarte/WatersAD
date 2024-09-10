using WatersAD.Data.Entities;
using WatersAD.Enum;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public interface IConverterHelper
    {
        /// <summary>
        /// Converts a RegisterNewUserViewModel to a User object
        /// </summary>
        /// <param name="model">The view model containing the data</param>
        /// <param name="path">Path for profile picture</param>
        /// <returns>User</returns>
        User ToUser(RegisterNewUserViewModel model, string path);

        /// <summary>
        /// Converts a User to a ToRegisterNewUserViewModel object
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>Register New User View Model</returns>
        RegisterNewUserViewModel ToRegisterNewUserViewModel(User user);

        /// <summary>
        ///  Converts a ChangeUserViewModel to a User object
        /// </summary>
        /// <param name="model">The view model containing the data</param>
        /// <param name="path"></param>
        /// <returns>User</returns>
        User ToUser(ChangeUserViewModel model, string path);

        /// <summary>
        /// Converts a role name into a corresponding enumeration value.
        /// </summary>
        /// <param name="roleName">The name of the role to be converted</param>
        /// <returns>A value that corresponds to the specified role name.</returns>
        UserType ConvertRoleToUserType(string roleName);

        Client ToCliente(ClientViewModel client);

        Task<ClientViewModel> ToClientViewModel(Client client);
    }
}
