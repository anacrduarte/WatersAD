using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IFlashMessage _flashMessage;

        public NotificationController(INotificationRepository notificationRepository, IFlashMessage flashMessage)
        {
            _notificationRepository = notificationRepository;
            _flashMessage = flashMessage;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync();
            var request = await _notificationRepository.GetRequestWaterMeterAsync();
            if (this.User.IsInRole("Admin"))
            {
                return View(request.SelectMany(r => r.Notifications).Where(n => n.IsRead));
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
    }
}
