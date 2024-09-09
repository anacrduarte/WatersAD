using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using WatersAD.Data.Entities;
using WatersAD.Enum;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public interface IUserHelper
    {
        /// <summary>
        /// Retrieves a user based on their email address.
        /// </summary>
        /// <param name="email">email address</param>
        /// <returns>The user object associated with the specified email</returns>
        Task<User> GetUserByEmailAsync(string email);

        /// <summary>
        /// Adds a new user to the system with the specified password.
        /// </summary>
        /// <param name="user">The user object</param>
        /// <param name="password">The password</param>
        /// <returns>Success ou failure</returns>
        Task<IdentityResult> AddUserAsync(User user, string password);

        /// <summary>
        /// Attempts to log in a user
        /// </summary>
        /// <param name="model">The login view model</param>
        /// <returns>Success ou failure</returns>
        Task<SignInResult> LoginAsync(LoginViewModel model);

        /// <summary>
        /// Logs out the currently authenticated user
        /// </summary>
        /// <returns> A task that represents the asynchronous logout operation.</returns>
        Task LogoutAsync();

        /// <summary>
        /// Updates the specified user's information 
        /// </summary>
        /// <param name="user">The user object</param>
        /// <returns>Success ou failure</returns>
        Task<IdentityResult> UpdateUserAsync(User user);

        /// <summary>
        ///  Changes the password for the specified user.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="odlPassword">Current password</param>
        /// <param name="newPassword">New password</param>
        /// <returns>Success ou failure</returns>
        Task<IdentityResult> ChangePasswordAsync(User user, string odlPassword, string newPassword);

        /// <summary>
        /// Checks if a specified role exists in the system
        /// </summary>
        /// <param name="roleName">The name of the role</param>
        /// <returns> The task completes when the role check is performed, but it does not return a result.</returns>
        Task CheckRoleAsync(string roleName);

        /// <summary>
        /// Adds the specified user to the given role
        /// </summary>
        /// <param name="user">The user to be added</param>
        /// <param name="roleName">The name of the role</param>
        /// <returns>Success ou failure</returns>
        Task AddUserToRoleAsync(User user, string roleName);

        /// <summary>
        /// Checks if the specified user is in the given role.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="roleName">The name of the role</param>
        /// <returns>The task result is a boolean indicating whether the user is in the specified role (true) or not (false).</returns>
        Task<bool> IsUserInRoleAsync(User user, string roleName);

        /// <summary>
        /// Retrieves a collection of role types formatted for use in a dropdown list.
        /// </summary>
        /// <returns>The available role types for selection in a user interface</returns>
        IEnumerable<SelectListItem> GetComboTypeRole();

        /// <summary>
        /// Retrieves a collection of users who are assigned to the specified role.
        /// </summary>
        /// <param name="roleName">The name of the role </param>
        /// <returns>Users who are assigned to the specified role.</returns>
        Task<IEnumerable<User>> GetUsersWithRole(UserType roleName);
    }
}
