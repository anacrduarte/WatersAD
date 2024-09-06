using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
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

        public IActionResult Register()
        {
            return View();
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

                    user = _converterHelper.ToUser(model, path, true);


                    var result = await _userHelper.AddUserAsync(user, model.Password);
                    if (result != IdentityResult.Success)
                    {
                        _flashMessage.Info("Erro no loggin");
                        return View(model);
                    }

                    var loginViewModel = new LoginViewModel
                    {
                        Password = model.Password,
                        RememberMe = false,
                        UserName = model.Username,
                    };

                    var result2 = await _userHelper.LoginAsync(loginViewModel);

                    if (result2.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    ModelState.AddModelError(string.Empty, "The user couldn´t be logged.");

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

                    var convertUser = _converterHelper.ToUser(model, path, false);

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
    }

}
