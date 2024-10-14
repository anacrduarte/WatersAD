using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WatersAD.Data;
using WatersAD.Data.Entities;
using WatersAD.Enum;
using WatersAD.Models;

namespace WatersAD.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;
        private readonly IConverterHelper _converterHelper;

        public UserHelper(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, DataContext dataContext, IConverterHelper converterHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
            _converterHelper = converterHelper;
        }
        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.Email),
                                new Claim("ImageUrl", user.ImageUrl),
                                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                            };

                await _userManager.AddClaimsAsync(user, claims);
            }

            return result;
        }

        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string odlPassword, string newPassword)
        {
            if (user.MustChangePassword)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);

            }

            return await _userManager.ChangePasswordAsync(user, odlPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);

            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = roleName,
                });
            }
        }

        public IEnumerable<SelectListItem> GetComboTypeRole()
        {

            var roles = _roleManager.Roles.Select(r => new SelectListItem
            {
                Value = r.Name,
                Text = r.Name,
            })
                        .OrderBy(l => l.Text).ToList();


            roles.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "Selecione um Role"
            });

            return roles;

        }
        public async Task<User> GetUserAsync(Guid userId)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == userId.ToString());
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.UserName,
                model.Password,
                model.RememberMe,
                true);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, user.Email),
                                new Claim("ImageUrl", user.ImageUrl),
                                new Claim("FullName", $"{user.FirstName} {user.LastName}"),
                            };

                var existingClaims = await _userManager.GetClaimsAsync(user);
                await _userManager.RemoveClaimsAsync(user, existingClaims);


                await _userManager.AddClaimsAsync(user, claims);
            }
            return result;
        }

        public async Task UpdateUserClaimsAsync(User user)
        {
            await _signInManager.RefreshSignInAsync(user);
        }

        public async Task<IEnumerable<User>> GetUsersWithRole(UserType roleName)
        {

            return await _userManager.Users
                .Where(u => u.UserType == roleName)
                .OrderBy(u => u.FirstName).ToListAsync();
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password)
        {
            return await _userManager.ResetPasswordAsync(user, token, password);
        }




    }
}
