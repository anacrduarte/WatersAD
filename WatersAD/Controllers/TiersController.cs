﻿using Microsoft.AspNetCore.Mvc;
using Vereyon.Web;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;

namespace WatersAD.Controllers
{
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
                try
                {
                    await _tierRepository.CreateAsync(tier);
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

        // GET: Tiers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tier = await _tierRepository.GetByIdAsync(id.Value);

            if (tier == null)
            {
                return NotFound();
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
            var tier = await _tierRepository.GetByIdAsync(id);
            if (tier != null)
            {
                await _tierRepository.DeleteAsync(tier);
            }

            return RedirectToAction(nameof(Index));
        }


    }
}