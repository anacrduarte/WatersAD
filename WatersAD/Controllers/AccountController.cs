using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Vereyon.Web;
using WatersAD.Enum;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IFlashMessage _flashMessage;

        public AccountController(IUserHelper userHelper, IConverterHelper converterHelper, IImageHelper imageHelper, IFlashMessage flashMessage)
        {
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _flashMessage = flashMessage;
        }
        /// <summary>
        /// Show the page
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            if (User!.Identity!.IsAuthenticated)
            {

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// To do login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                //TODO tirar ou arranjar solução para a imagem do user
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);
                ViewBag.ImageUser = user.ImageUrl;


                //tirar
                var result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {


                    if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(this.Request.Query["ReturnUrl"].First());
                    }

                    return this.RedirectToAction("Index", "Home");
                }
            }

            this.ModelState.AddModelError(string.Empty, "Failed to login");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Register()
        {
            var model = new RegisterNewUserViewModel
            {
                Roles = _userHelper.GetComboTypeRole() 
            };


            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);

                if (user == null)
                {
                   

                    var path = string.Empty;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "user");
                    }

                    user = _converterHelper.ToUser(model, path);

                    var result = await _userHelper.AddUserAsync(user, model.Password);

                    if (!string.IsNullOrEmpty(model.SelectedRole))
                    {
                        await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);
                    }

                    if (result.Succeeded)
                    {
                        _flashMessage.Info("Utilizador adicionado com sucesso.");
                        return RedirectToAction("Register");

                    }

                }
                _flashMessage.Info("Utilizador já existe");
            }
            return View(model);

        }
        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);

            var model = new ChangeUserViewModel();
            if (user != null)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.ImageUrl = user.ImageFullPath;
               
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(ChangeUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                  
                    var oldPath = user.ImageUrl;

                    var path = oldPath;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        path = await _imageHelper.UploadImageAsync(model.ImageFile, "user", oldPath);
                    }

                    var convertUser = _converterHelper.ToUser(model, path);

                    user.FirstName = convertUser.FirstName;
                    user.LastName = convertUser.LastName;
                    user.ImageUrl = convertUser.ImageUrl;

                    var response = await _userHelper.UpdateUserAsync(user);
                    if (response.Succeeded)
                    {

                        _flashMessage.Confirmation("User updated!");
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                    }
                }

            }

            return View(model);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(this.User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return this.RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        this.ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "User not found");
                }
            }
            return this.View(model);
        }

        public async Task<IActionResult> ChangeRole(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                return BadRequest("Role not specified.");
            }

            var roleType = _converterHelper.ConvertRoleToUserType(role);
            var model = new ChangeRoleViewModel
            {
               CurrentRole = roleType,
               User = await _userHelper.GetUsersWithRole(roleType)
            };
            return View(model);
        }

        public async Task<IActionResult> EditRole(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var user = await _userHelper.GetUserByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }
            var model = new ChangeRoleViewModel()
            {
                UserName = user.Email,
                CurrentRole = user.UserType,
                Roles = _userHelper.GetComboTypeRole(),
            };


            return View(model);

           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(ChangeRoleViewModel model, string currentRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if (user != null)
                {
                    user.UserType = _converterHelper.ConvertRoleToUserType(model.SelectedRole);
                }

                if (!string.IsNullOrEmpty(model.SelectedRole))
                {
                    await _userHelper.AddUserToRoleAsync(user, model.SelectedRole);
                }

                var response = await _userHelper.UpdateUserAsync(user);
                if (response.Succeeded)
                {

                    _flashMessage.Confirmation("User updated!");
                    return RedirectToAction("ChangeRole", new { role = model.CurrentRole });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Errors.FirstOrDefault().Description);
                }
            }

            return View(model);
        }


    }

}
