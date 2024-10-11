using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IMailHelper _mailHelper;

        public NotificationController(INotificationRepository notificationRepository, IFlashMessage flashMessage, IMailHelper mailHelper)
        {
            _notificationRepository = notificationRepository;
            _flashMessage = flashMessage;
            _mailHelper = mailHelper;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync();
            var request = await _notificationRepository.GetRequestWaterMeterAsync();
            if (this.User.IsInRole("Admin"))
            {
                return View(request.Where(r=> !r.Decline).SelectMany(r => r.Notifications).Where(n => n.IsRead));
            }
            else
            {
                return View(notifications.Where(n => !n.IsRead));
            }
           
        }

        public async Task<IActionResult> AcceptRequest(int? id)
        {
            var notification = await _notificationRepository.GetNotificationAndRequestByIdAsync(id.Value);
            if (notification == null)
            {
                return NotFound();
            }
            var request =  await _notificationRepository.GetRequestWaterMeterByIdAsync(notification.RequestWaterMeter.Id);
            if(request == null)
            {
                return NotFound();
            }
            var model = new NotificationViewModel
            { 
                RequestWaterMeterId = request.Id,
                 NotificationId = notification.Id,
            };
            
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AcceptRequest(NotificationViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    var request = await _notificationRepository.GetRequestWaterMeterByIdAsync(model.RequestWaterMeterId);

                    if (request == null)
                    {
                        return NotFound();
                    }
                    var notification = await _notificationRepository.GetByIdAsync(model.NotificationId);

                    if (notification == null)
                    {
                        return NotFound();
                    }

                    request.InstallationDate = model.InstallationDate;
                    await _notificationRepository.UpadateDateRequestAsync(request);
                    await _notificationRepository.MarkAsReadAsync(notification.Id);

                    _flashMessage.Info("Pedido actualizado e enviado com sucesso.");
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                    return RedirectToAction("Index", "Notification");
                }
            }
            _flashMessage.Info("Erro no formulário.");
            return RedirectToAction(nameof(AcceptRequest));

        }

        [HttpPost]
        public async Task<IActionResult> DeclineRequest(int? id)
        {
            try
            {
                var notification = await _notificationRepository.GetNotificationAndRequestByIdAsync(id.Value);
                if (notification == null)
                {
                    return NotFound();
                }
                var request = await _notificationRepository.GetRequestWaterMeterByIdAsync(notification.RequestWaterMeter.Id);
                if (request == null)
                {
                    return NotFound();
                }
                await _notificationRepository.MarkAsReadAsync(notification.Id);

                request.Decline = true;

                await _notificationRepository.UpadateDateRequestAsync(request);

                string subject = "Waters AD - Recusa de pedido de contador";
                string body = $"<h1>Waters AD - Recusa</h1>" +
                              $"Vimos por este meio informar que o seu pedido de contador foi recusado." +
                              $"<p>Obrigado</p>";

                var response = await _mailHelper.SendMail($"{request.FirstName} {request.LastName}", request.Email, subject, body);

                if (response.IsSuccess)
                {
                    _flashMessage.Info("Pedido recusado com sucesso.");
                    return RedirectToAction("Index", "Dashboard");
                }
                _flashMessage.Info("Erro a recusar pedido.");
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Erro ao processar a solicitação: {ex.Message}");
                return RedirectToAction("Index", "Dashboard");
            }

        }


    }
}
