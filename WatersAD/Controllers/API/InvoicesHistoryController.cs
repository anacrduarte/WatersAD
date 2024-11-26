using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers.API
{
	[Route("api/[controller]")]
	[ApiController]
	public class InvoicesHistoryController : Controller
	{
		private readonly IInvoiceRepository _invoiceRepository;
		private readonly IConsumptionRepository _consumptionRepository;
		private readonly IWaterMeterRepository _waterMeterRepository;
		private readonly ITierRepository _tierRepository;
		private readonly IUserHelper _userHelper;
		private readonly IClientRepository _clientRepository;

		public InvoicesHistoryController(IInvoiceRepository invoiceRepository, IConsumptionRepository consumptionRepository, IWaterMeterRepository waterMeterRepository,
			ITierRepository tierRepository, IUserHelper userHelper, IClientRepository clientRepository)
		{
			_invoiceRepository = invoiceRepository;
			_consumptionRepository = consumptionRepository;
			_waterMeterRepository = waterMeterRepository;
			_tierRepository = tierRepository;
			_userHelper = userHelper;
			_clientRepository = clientRepository;
		}
		
		[HttpGet("details/{id}")]
		public async Task<ActionResult> GetDetails(int id)
		{
			try
			{
				var invoice = await _invoiceRepository.GetDetailsInvoiceAsync(id);
				if (invoice == null)
				{
					return NotFound(new { message = "Invoice not found" });
				}

				var consumption = await _consumptionRepository.GetConsumptionAsync(invoice.Id);
				if (consumption == null)
				{
					return NotFound(new { message = "Consumption not found for this invoice" });
				}

				var waterMeter = await _waterMeterRepository.GetWaterMeterWithCityAndCountryAsync(consumption.WaterMeter.Id);
				var waterMeterService = await _waterMeterRepository.GetWaterServiceByIdAsync(consumption.WaterMeter.Id);
				var tier = await _tierRepository.GetByIdAsync(consumption.TierId);

				var model = new InvoiceDetailsViewModel
				{
					Client = invoice.Client,
					WaterMeter = waterMeter,
					Invoice = invoice,
					Consumption = consumption,
					WaterMeterService = waterMeterService,
					Tier = tier,
				};

				return Ok(model);
			}
			catch (Exception ex)
			{

				return StatusCode(500, new { message = $"Ocorreu um erro ao processar a requisição. {ex.Message}" });
			}
		}

		[HttpGet("date/{id}")]
		public async Task<IActionResult> GetLatestDateAsync(int id)
        {
            try
			{
				DateTime nextMonthDate;
                var waterMeter = await _waterMeterRepository.GetWaterMeterWithConsumptionsAsync(id);


				if (waterMeter == null)
				{
					return NotFound(new { message = "Nenhum contador encontrado." });
				}
                if (waterMeter.Consumptions == null)
                {
                    nextMonthDate = new DateTime(2024, 1, 1);
                }
                else
                {
                    DateTime previousDate = waterMeter.Consumptions
                                   .OrderByDescending(c => c.ConsumptionDate)
                                   .FirstOrDefault()?.ConsumptionDate ?? DateTime.Now;

                    nextMonthDate = previousDate.AddMonths(1);
                }
                return Ok(nextMonthDate);

			}
			catch (Exception ex)
			{

				return StatusCode(500, new { message = $"Ocorreu um erro ao processar a requisição. {ex.Message}" });
			}
		}

	}
}
