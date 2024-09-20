using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using WatersAD.Data.Entities;
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
        private readonly IMailHelper _mailHelper;

        public AccountController(IUserHelper userHelper, IConverterHelper converterHelper, IImageHelper imageHelper, IFlashMessage flashMessage, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
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

                if (result.IsLockedOut)
                {
                    _flashMessage.Warning(string.Empty, "Foi superado o número máximo de tentativas, a sua conta está bloqueada, tente novamente mais tarde.");
                }
                else
                {
                    _flashMessage.Warning(string.Empty, "Email e palavra-passe incorretos.");
                }

            }

          
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

                    string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                    string? tokenLink = Url.Action("ConfirmEmail", "Account", new
                    {
                        userid = user.Id,
                        token = myToken
                    }, protocol: HttpContext.Request.Scheme);

                    Response response = _mailHelper.SendMail(
                   $"{model.FirstName} {model.LastName}", model.Username!,
                   "Water AD - Confirmação de Email",
                   $"<h1>SalesCodeSpace 2024 - Confirmação de Email</h1>" +
                       $"Clique no link para poder entrar como utilizador:, " +
                       $"<p><a href = \"{tokenLink}\">Confirmar Email</a></p>");

                    if (response.IsSuccess)
                    {
                        _flashMessage.Info("As instruções para poder entrar foram enviadas para o seu email.");
                        return RedirectToAction("Index", "Home");
                    }



                }
                _flashMessage.Info("Utilizador já existe");
            }
            return View(model);

        }
        public async Task<IActionResult> ChangeUser()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);

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
                var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
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
                        _flashMessage.Warning(response.Errors.FirstOrDefault().Description);
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
                if (model.OldPassword == model.NewPassword)
                {
                    _flashMessage.Danger("Deve inserir uma palavra-passe diferente.");
                    return View(model);
                }

                var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        _flashMessage.Danger(result.Errors.FirstOrDefault()!.Description);
                    }
                }
                else
                {
                    _flashMessage.Danger("Utilizador não encontrado.");
                }
            }

            return View(model);
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
                    _flashMessage.Danger(response.Errors.FirstOrDefault()!.Description);
                }
            }

            return View(model);
        }


        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User? user = await _userHelper.GetUserAsync(new Guid(userId));

            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    _flashMessage.Warning("O email não corresponde ao email registado.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                string? link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(
                    $"{user.Email}",//alterar para nome
                    model.Email!,
                    "Water AD - Recuperação da Palavra-passe",
                    $"<h1>Shopping - Recuperação da Palavra-passe</h1>" +
                    $"Para recuperar a palavra-passe clique no link:" +
                    $"<p><a href = \"{link}\">Reset Password</a></p>");
                _flashMessage.Info("As instruções para recuperar a sua palavra-passe foram enviadas para o seu correio.");
                return View();
            }
            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User? user = await _userHelper.GetUserByEmailAsync(model.UserName);

            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token!, model.Password!);
                if (result.Succeeded)
                {
                    _flashMessage.Info("Palavra-passe alterada com êxito.");
                    return View();
                }

                _flashMessage.Info("Erro ao trocar a palavra-passe.");
                return View(model);
            }
            _flashMessage.Info("Utilizador não encontrado.");
            return View(model);
        }

    }

}
