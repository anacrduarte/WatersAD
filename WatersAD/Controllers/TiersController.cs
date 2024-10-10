using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;

namespace WatersAD.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TiersController : Controller
    {

        private readonly ITierRepository _tierRepository;
        private readonly IFlashMessage _flashMessage;

        public TiersController(ITierRepository tierRepository, IFlashMessage flashMessage)
        {

            _tierRepository = tierRepository;
            _flashMessage = flashMessage;
        }

        // GET: Tiers
        public IActionResult Index()
        {
            return View(_tierRepository.GetAll().OrderBy(t => t.TierNumber));
        }
      
        // GET: Tiers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tiers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tier tier)
        {
            if (ModelState.IsValid)
            {
                var existingTiers =  _tierRepository.GetAll(); 

                
                foreach (var existingTier in existingTiers)
                {
                    if (tier.UpperLimit <= existingTier.UpperLimit)
                    {
                        
                        _flashMessage.Warning($"O valor máximo do novo escalão está entre os valores de um escalão existente!Edite o limite dos escalões de depois crie um novo.");
                        return View(tier); 
                    }
                }
                try
                {
                    await _tierRepository.CreateAsync(tier);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    _flashMessage.Warning($"Erro na requisição do pedido. {ex.Message} " );


                    return View(tier);
                }

            }
            return View(tier);
        }

        // GET: Tiers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("TierNotFound");
            }

            var tier = await _tierRepository.GetByIdAsync(id.Value);

            if (tier == null)
            {
                return new NotFoundViewResult("TierNotFound");
            }
            return View(tier);
        }

        // POST: Tiers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Tier tier)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    await _tierRepository.UpdateAsync(tier);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {

                    _flashMessage.Warning($"Cada escalão só pode ter um unico número de escalão associado!!");
                    return View(tier);

                }

            }
            return View(tier);
        }



        public async Task<IActionResult> Delete(int id)
        {
            
            try
            {
                var tier = await _tierRepository.GetByIdAsync(id);
                if (tier == null)
                {
                    return new NotFoundViewResult("TierNotFound");

                }

                await _tierRepository.DeleteAsync(tier);
            }
            catch (Exception ex)
            {
                _flashMessage.Warning($"Erro a apagar escalão! { ex.Message}");
                throw;
            }
            return RedirectToAction(nameof(Index));
        }


    }
}
