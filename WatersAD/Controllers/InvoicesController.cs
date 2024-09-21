using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereyon.Web;
using WatersAD.Data;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;
using WatersAD.Helpers;

namespace WatersAD.Controllers
{
    public class InvoicesController : Controller
    {
       
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IFlashMessage _flashMessage;

        public InvoicesController(IInvoiceRepository invoiceRepository, IFlashMessage flashMessage)
        {
            
            _invoiceRepository = invoiceRepository;
            _flashMessage = flashMessage;
        }

        // GET: Invoices
        public IActionResult Index()
        {
            return View( _invoiceRepository.GetAll());
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

      
    }
}
