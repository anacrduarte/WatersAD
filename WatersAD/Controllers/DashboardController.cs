using Microsoft.AspNetCore.Mvc;

namespace WatersAD.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.IsHomePage = true;
            return View();
        }
    }
}
