using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class ConsumptionsController : Controller
    {

        private readonly IConsumptionRepository _consumptionRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly ITierRepository _tierRepository;
        private readonly IFlashMessage _flashMessage;

        public ConsumptionsController(IConsumptionRepository consumptionRepository, IWaterMeterRepository waterMeterRepository,
            ITierRepository tierRepository, IFlashMessage flashMessage)
        {

            _consumptionRepository = consumptionRepository;
            _waterMeterRepository = waterMeterRepository;
            _tierRepository = tierRepository;
            _flashMessage = flashMessage;
        }

        // GET: Consumptions
        public IActionResult Index()
        {
            return View(_consumptionRepository.GetAllWaterMeterAndClient());
        }


        // GET: Consumptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption = await _consumptionRepository.GetByIdAsync(id.Value);
            if (consumption == null)
            {
                return NotFound();
            }

            return View(consumption);
        }

        // GET: Consumptions/Create
        public IActionResult Create()
        {
            var model = new ConsumptionViewModel
            {
                WaterMeters = _waterMeterRepository.GetComboWaterMeter(),
            };
            return View(model);
        }

        // POST: Consumptions/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsumptionViewModel model)
        {
            if (ModelState.IsValid)
            {

                var waterMeter = await _waterMeterRepository.GetWaterMeterWithConsumptionsAsync(model.WaterMeterId);

                if (waterMeter == null)
                {
                    return NotFound();
                }

                var consumption = await _consumptionRepository.GetByIdAsync(model.Id);
                if (consumption != null)
                {
                    return NotFound();
                }



                var matchingTier = await _tierRepository.GetMatchingTierAsync(model.ConsumptionValue);

                if (matchingTier == null)
                {
                    return NotFound();
                }

                var previousConsumption = _consumptionRepository.GetPreviousConsumption(waterMeter);

                
                if (previousConsumption != null && model.ConsumptionValue <= previousConsumption.ConsumptionValue)
                {
                    _flashMessage.Warning("O valor de consumo atual deve ser maior do que o último valor registrado.");
                    
                    return View(model);
                }

                try
                {
                    await _consumptionRepository.CreateConsumptionAndInvoiceAsync(model, waterMeter, matchingTier, previousConsumption);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    throw;
                }

                


            }
            return View(model);
        }

        // GET: Consumptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption = await _consumptionRepository.GetByIdAsync(id.Value);
            if (consumption == null)
            {
                return NotFound();
            }
            return View(consumption);
        }

        // POST: Consumptions/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConsumptionDate,RegistrationDate,ConsumptionValue")] Consumption consumption)
        {
            if (id != consumption.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _consumptionRepository.UpdateAsync(consumption);
                }
                catch (DbUpdateConcurrencyException)
                {

                    return NotFound();

                }
                return RedirectToAction(nameof(Index));
            }
            return View(consumption);
        }

        // GET: Consumptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumption = await _consumptionRepository.GetByIdAsync(id.Value);
            if (consumption == null)
            {
                return NotFound();
            }

            return View(consumption);
        }

        // POST: Consumptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumption = await _consumptionRepository.GetByIdAsync(id);
            if (consumption != null)
            {
                await _consumptionRepository.DeleteAsync(consumption);
            }


            return RedirectToAction(nameof(Index));
        }


    }
}
