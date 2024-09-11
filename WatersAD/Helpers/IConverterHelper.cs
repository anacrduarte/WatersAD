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

        /// <summary>
        /// Converts a <see cref="ClientViewModel"/> object to a <see cref="Client"/> object.
        /// </summary>
        /// <param name="client">The <see cref="ClientViewModel"/> object to be converted.</param>
        /// <returns>A <see cref="Client"/> object that represents the converted data from the <see cref="ClientViewModel"/>.</returns>
        Client ToCliente(ClientViewModel client);

        /// <summary>
        /// Converts a <see cref="Client"/> object to a <see cref="ClientViewModel"/> object asynchronously.
        /// </summary>
        /// <param name="client">The <see cref="Client"/> object to be converted.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="ClientViewModel"/> object that represents the converted data from the <see cref="Client"/>.</returns>
        Task<ClientViewModel> ToClientViewModelAsync(Client client);
    }
}
