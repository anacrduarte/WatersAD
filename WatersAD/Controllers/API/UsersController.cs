using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using WatersAD.Data.Entities;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IMailHelper _mailHelper;

        public UsersController(IUserHelper userHelper, IImageHelper imageHelper, IConverterHelper converterHelper, IMailHelper mailHelper)
        {
            _userHelper = userHelper;
            _imageHelper = imageHelper;
            _converterHelper = converterHelper;
            _mailHelper = mailHelper;
        }
        // POST users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {


                var result = await _userHelper.LoginAsync(model);

                if (!result.Succeeded)
                {
                    return Unauthorized("Dados inválidos.");
                }

                var currentUser = await _userHelper.GetUserByEmailAsync(model.UserName);

                if (currentUser == null)
                {
                    return NotFound("Utilizador não encontrado.");
                }


                var jwt = _userHelper.GenerateJwtToken(currentUser);

                return new ObjectResult(new
                {
                    accesstoken = jwt,
                    tokentype = "bearer",
                    userid = currentUser.Id,
                    username = currentUser.Email,
                    firstname = currentUser.FirstName,
                    lastname = currentUser.LastName,
 
                    imageurl = currentUser.ImageUrl,
                   
                });
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error logging in.", error = ex.Message });
            }
        }

        [HttpPut("changeuser")]
        public async Task<IActionResult> ChangeUser([FromForm] ChangeUserViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Por favor, corrija os erros no formulário.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                var user = await _userHelper.GetUserByEmailAsync(model.userName);

                if (user == null)
                {
                    return NotFound(new { Success = false, Message = "Utilizador não encontrado." });
                }

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
                    return Ok(new { FirstName = user.FirstName,
                    LastName = user.LastName,
                    ImageUrl = user.ImageUrl});
                }
                else
                {
                    return BadRequest(new { Success = false, Message = response.Errors.FirstOrDefault()?.Description });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Erro ao atualizar o utilizador: {ex.Message}" });
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Por favor, corrija os erros no formulário.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                if (model.OldPassword == model.NewPassword)
                {
                    return BadRequest(new { Success = false, Message = "Deve inserir uma palavra-passe diferente." });
                }

                var user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);

                    if (result.Succeeded)
                    {
                       

                            return Ok(new
                            {
                                Success = true,
                                Message = "Palavra-passe alterada com sucesso!"
                            });
                       
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            Success = false,
                            Message = result.Errors.FirstOrDefault()?.Description
                        });
                    }
                }
                else
                {
                    return NotFound(new { Success = false, Message = "Utilizador não encontrado." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Erro ao alterar a palavra-passe: {ex.Message}" });
            }
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Por favor, corrija os erros no formulário.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }
            try
            {

                User user = await _userHelper.GetUserByEmailAsync(model.UserName);

                if (user != null)
                {

                    IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token!, model.Password!);

                    if (result.Succeeded)
                    {
                        return Ok(new
                        {
                            Success = true,
                            Message = "Palavra-passe alterada com sucesso."
                        });
                    }

                    return BadRequest(new
                    {
                        Success = false,
                        Message = "Erro ao alterar palavra-passe.",
                        Errors = result.Errors.Select(e => e.Description)
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Utilizador não encontrado."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Ocorreu um erro ao tentar alterar a palavra-passe: {ex.Message}"
                });
            }
        }

        [HttpPost("recoverpassword")]
        public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Por favor, corrija os erros no formulário.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            try
            {
                User user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (user == null)
                {
                    return NotFound(new
                    {
                        Success = false,
                        Message = "O email não corresponde ao email registado."
                    });
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);

                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken },
                    protocol: HttpContext.Request.Scheme);

                Response response = await _mailHelper.SendMail(
                    $"{user.FirstName} {user.LastName}",
                    model.Email!,
                    "Water AD - Recuperação da Palavra-passe",
                    $"<h1>Águas Duarte - Recuperação da Palavra-passe</h1>" +
                    $"Para recuperar a palavra-passe clique no link:" +
                    $"<p><a href=\"{link}\">Reset Password</a></p>");

                if (response.IsSuccess)
                {
                    return Ok(new
                    {
                        Success = true,
                        Message = "As instruções para recuperar a sua palavra-passe foram enviadas para o seu correio."
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        Success = false,
                        Message = "Erro ao enviar email. Tente novamente mais tarde."
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = $"Erro ao tentar recuperar a palavra-passe: {ex.Message}"
                });
            }
        }



        [HttpGet("userdetails")]
        public async Task<IActionResult> GetUserDetails()
        {

            var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            var user = await _userHelper.GetUserByEmailAsync(userEmail);

            if (user == null)
                return NotFound("Utilizador não encontrado");

            var userData = new
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
            };

            return Ok(userData);
        }

        [HttpPost("logout")] 
        public async Task<IActionResult> Logout()
        {
            try
            {
               
                await _userHelper.LogoutAsync();

              
                return Ok(new { message = "Logout realizado com sucesso." });
            }
            catch (Exception ex)
            {
                

              
                return BadRequest(new { message = $"Erro ao realizar logout. {ex.Message}"});
            }
        }
    }
}
