using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    //[Authorize(Roles = "Employee")]
    public class ConsumptionsController : Controller
    {

        private readonly IConsumptionRepository _consumptionRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly ITierRepository _tierRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;

        public ConsumptionsController(IConsumptionRepository consumptionRepository, IWaterMeterRepository waterMeterRepository,
            ITierRepository tierRepository, IFlashMessage flashMessage, IUserHelper userHelper, IClientRepository clientRepository)
        {

            _consumptionRepository = consumptionRepository;
            _waterMeterRepository = waterMeterRepository;
            _tierRepository = tierRepository;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
        }

        // GET: Consumptions
        public IActionResult Index()
        {
            return View(_consumptionRepository.GetAllWaterMeterAndClient());
        }

        public async Task<IActionResult> ShowConsumptions()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);

            if(user == null)
            {
                return new NotFoundViewResult("ConsumptionNotFound");
            }
           var client = await _clientRepository.GetClientByUserEmailAsync(user.Email);

            
         
            try
            {
                var waterMeter = await _waterMeterRepository.GetWaterMetersWithConsumptionsByClientAsync(client.Id);
              

                var model = new ShowConsumptionsViewModel
                {
                    ClientId = client.Id,
                    WaterMeters = waterMeter,
                    Consumptions = waterMeter.SelectMany(wm => wm.Consumptions).OrderByDescending(c => c.ConsumptionDate).ToList(),
                   
                };




                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return this.RedirectToAction("Consumptions", "ShowConsumptions");
            }
        }


        // GET: Consumptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ConsumptionNotFound");
            }

            try
            {
                var consumption = await _consumptionRepository.GetByIdAsync(id.Value);
                if (consumption == null)
                {
                    return new NotFoundViewResult("ConsumptionNotFound");
                }

                return View(consumption);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        //TODO ver se quero que fique o que esta em baixo ou se vou alterar
        //GET: Consumptions/Create
        //public IActionResult Create()
        //{
        //    try
        //    {
        //        var model = new ConsumptionViewModel
        //        {
        //            WaterMeters = _waterMeterRepository.GetComboWaterMeter(),
        //        };
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        public IActionResult Create(int? clientId, int? waterMeterId)
        {
            try
            {
                var model = new ConsumptionViewModel
                {
                    WaterMeters = _waterMeterRepository.GetComboWaterMeter(),
                    ClientId = clientId.Value,
                    WaterMeterId = waterMeterId.Value
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
        // POST: Consumptions/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConsumptionViewModel model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    var waterMeter = await _waterMeterRepository.GetWaterMeterWithConsumptionsAsync(model.WaterMeterId);

                    if (waterMeter == null)
                    {
                        _flashMessage.Warning("Contador não encontrado.");
                        return View(model);
                    }

                    var consumption = await _consumptionRepository.GetByIdAsync(model.Id);
                    if (consumption != null)
                    {
                        _flashMessage.Warning("Este consumo já existe.");
                        return View(model);
                    }

                    var matchingTier = await _tierRepository.GetMatchingTierAsync(model.ConsumptionValue);

                    if (matchingTier == null)
                    {
                        _flashMessage.Warning("Escalão não encontrado.");
                        return View(model);
                    }

                    var previousConsumption = _consumptionRepository.GetPreviousConsumption(waterMeter);


                    if (previousConsumption != null && model.ConsumptionValue <= previousConsumption.ConsumptionValue)
                    {
                        _flashMessage.Warning("O valor de consumo atual deve ser maior do que o último valor registrado.");

                        return View(model);
                    }

                    await _consumptionRepository.CreateConsumptionAndInvoiceAsync(model, waterMeter, matchingTier, previousConsumption);
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }

            }
            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(model);
        }

        // GET: Consumptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("ConsumptionNotFound");
            }

            try
            {
                var consumption = await _consumptionRepository.GetByIdAsync(id.Value);
                if (consumption == null)
                {
                    return new NotFoundViewResult("ConsumptionNotFound");
                }
                return View(consumption);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Consumptions/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ConsumptionDate,RegistrationDate,ConsumptionValue")] Consumption consumption)
        {
            if (id != consumption.Id)
            {
                return new NotFoundViewResult("ConsumptionNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _consumptionRepository.UpdateAsync(consumption);
                    _flashMessage.Info("Consumo editado com sucesso.");

                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                    

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
                return new NotFoundViewResult("ConsumptionNotFound");
            }
            try
            {

                var consumption = await _consumptionRepository.GetByIdAsync(id.Value);
                if (consumption == null)
                {
                    return new NotFoundViewResult("ConsumptionNotFound");
                }

                return View(consumption);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Consumptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var consumption = await _consumptionRepository.GetByIdAsync(id);
                if (consumption == null)
                {
                    return new NotFoundViewResult("ConsumptionNotFound");
                }

                await _consumptionRepository.DeleteAsync(consumption);
                _flashMessage.Info("Consumo apagado com sucesso.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }


    }
}
