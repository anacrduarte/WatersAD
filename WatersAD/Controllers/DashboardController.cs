using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;
using System.Xml;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;
using static WatersAD.Controllers.DashboardController;

namespace WatersAD.Controllers
{
    [Authorize(Roles = "Employee,Admin")]
    public class DashboardController : Controller
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IConsumptionRepository _consumptionRepository;

        public DashboardController(INotificationRepository notificationRepository, IClientRepository clientRepository, IWaterMeterRepository waterMeterRepository,
            IInvoiceRepository invoiceRepository, IConsumptionRepository consumptionRepository)
        {
            _notificationRepository = notificationRepository;
            _clientRepository = clientRepository;
            _waterMeterRepository = waterMeterRepository;
            _invoiceRepository = invoiceRepository;
            _consumptionRepository = consumptionRepository;
        }
        public async Task<IActionResult> Index()
        {
            var notifications = await _notificationRepository.GetUnreadNotificationsAsync();
            var request = await _notificationRepository.GetRequestWaterMeterAsync();
            var consumptions = _consumptionRepository.GetAll();

            var monthlyConsumption = consumptions.GroupBy(c => new { Year = c.ConsumptionDate.Year, Month = c.ConsumptionDate.Month })
                .Select(g => new ChartDataHelper
                {
                    month = CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    sales = g.Sum(c => c.ConsumptionValue)
                }).ToList();

            var waterMeters = _waterMeterRepository.GetAll();

            var metersInstalledPerYear = waterMeters
                .GroupBy(wm => wm.InstallationDate.Year)
                .Select(g => new LineDataHelper
                {
                    x = g.Key,
                    y = g.Count()
                })
                .ToList();

            var invoices = _invoiceRepository.GetAll();

            var totalInvoicesYear = invoices
                .GroupBy(i => i.InvoiceDate.Year)
                .Select(g => new InvoiceYearHelper
                {
                    Year = g.Key,
                    TotalAmount = g.Sum(i => i.TotalAmount),
                });

            var model = new ChartDataViewModel();


            model.ChartData = monthlyConsumption;
            model.Consumptions = consumptions;
            model.LineData = metersInstalledPerYear;
            model.WaterMeters = waterMeters;
            model.TotalAmount = totalInvoicesYear; 
            if (this.User.IsInRole("Admin"))
            {
                
                model.UnreadNotifications = request.SelectMany(r => r.Notifications).Where(n => n.IsRead);
            }
            else
            {
                
                model.UnreadNotifications = notifications.Where(n => !n.IsRead);
            }

      


            model.PieData= new List<PieDataHelper>
                {
                    new PieDataHelper { x = "Em espera", y = 37, text = "38%" },
                    new PieDataHelper { x = "Em resolução", y = 17, text = "19%" },
                    new PieDataHelper { x= "Resolvidas", y = 19, text = "25%" },
                    new PieDataHelper { x = "Alteração de contador",y = 4, text = "5%" },
                    new PieDataHelper { x = "Outras", y = 11, text = "13%" },
                    
                };

            model.CellSpacing = new double[] { 10, 10 };

            model.Clients = _clientRepository.GetAll();

            model.Invoices = invoices;
            

            return View(model);
        }

       
   

      

      
    }
}
