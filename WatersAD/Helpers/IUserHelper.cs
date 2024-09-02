using Microsoft.AspNetCore.Identity;
using WatersAD.Data.Entities;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);


        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();
    }
}
