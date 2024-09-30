using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;
using WatersAD.Models;

namespace WatersAD.Controllers
{
    public class InvoicesController : Controller
    {
       
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFlashMessage _flashMessage;
        private readonly IUserHelper _userHelper;
        private readonly IClientRepository _clientRepository;
        private readonly IWaterMeterRepository _waterMeterRepository;
        private readonly IConsumptionRepository _consumptionRepository;

        public InvoicesController(IInvoiceRepository invoiceRepository, IFlashMessage flashMessage, IUserHelper userHelper, 
            IClientRepository clientRepository, IWaterMeterRepository waterMeterRepository, IConsumptionRepository consumptionRepository)
        {
            
            _invoiceRepository = invoiceRepository;
            _flashMessage = flashMessage;
            _userHelper = userHelper;
            _clientRepository = clientRepository;
            _waterMeterRepository = waterMeterRepository;
            _consumptionRepository = consumptionRepository;
        }

        // GET: Invoices
        public IActionResult Index()
        {
            return View( _invoiceRepository.GetAll());
        }

        public async Task<IActionResult> GetInvoiceClient()
        {
            try
            {
                var user = await _userHelper.GetUserByEmailAsync(User.Identity!.Name!);
                if(user == null)
                {
                    return NotFound();
                }
                var client = await _clientRepository.GetClientByUserEmailAsync(user.Email);

                if(client == null)
                {
                    return NotFound();
                }
               var consumption = await _consumptionRepository.GetAllInvoicesForClientAsync(client.Id);
                

                var model = new InvoicesClientViewModel
                {
                    ClientId = client.Id,
                    WaterMeters = consumption.Select(c => c.WaterMeter).Distinct().ToList(),
                    Consumptions = consumption,
                    Invoices = consumption.Select(c => c.Invoice).Distinct().ToList(),
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return this.RedirectToAction("Invoice", "GetInvoiceClient");
            }
            
        }
        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }

            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
                if (invoice == null)
                {
                    return new NotFoundViewResult("InvoiceNotFound");
                }

                return View(invoice);
            }
            catch (Exception ex)
            {

                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: Invoices/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InvoiceDate,ClientId,Issued,Sent,TotalAmount")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _invoiceRepository.CreateAsync(invoice);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                    return RedirectToAction(nameof(Index));
                }
            }
           
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }

            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
                if (invoice == null)
                {
                    return new NotFoundViewResult("InvoiceNotFound");
                }

                return View(invoice);
            }
            catch (Exception ex)
            {

                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Invoices/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _invoiceRepository.UpdateAsync(invoice);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                    return RedirectToAction(nameof(Index));

                }
              
            }
            _flashMessage.Warning("Por favor, corrija os erros no formulário.");
            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("InvoiceNotFound");
            }

            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
                if (invoice == null)
                {
                    return new NotFoundViewResult("InvoiceNotFound");
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                if (invoice == null)
                {
                    return new NotFoundViewResult("InvoiceNotFound");

                }

                await _invoiceRepository.DeleteAsync(invoice);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> InvoiceHistory(int clientId, int waterMeterId)
        {
            try
            {
               
                var client = await _clientRepository.GetByIdAsync(clientId);

                if (client == null)
                {
                    return NotFound();
                }

                var consumption = await _consumptionRepository.GetAllInvoicesForClientAsync(client.Id);

                var invoices = consumption.Where(c=> c.WaterMeterId == waterMeterId).Select(c=> c.Invoice).ToList();

                var waterMeter = await _waterMeterRepository.GetByIdAsync(waterMeterId);

                var model = new InvoicesClientViewModel
                {
                    Invoices = invoices,
                    WaterMeter = waterMeter,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _flashMessage.Danger($"Ocorreu um erro ao processar a requisição. {ex.Message}");
                return this.RedirectToAction("Invoice", "GetInvoiceClient");
            }

        }
    }
}
