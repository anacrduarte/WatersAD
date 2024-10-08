using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml;
using WatersAD.Data.Repository;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class DashboardController : Controller
    {
        private readonly INotificationRepository _notificationRepository;

        public DashboardController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
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
    }
}
