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
            return View(notifications); 
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationRepository.MarkAsReadAsync(id);  
            return RedirectToAction("Index");  
        }
    }
}
