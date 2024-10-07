using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Repository;

namespace WatersAD.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync();
            if (this.User.IsInRole("Admin"))
            {
                return View(notifications.Where(n => n.IsRead));
            }
            else
            {
                return View(notifications.Where(n => !n.IsRead));
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int id)
        {
            await _notificationRepository.MarkAsReadAsync(id);  
            return RedirectToAction("Index");  
        }
    }
}
