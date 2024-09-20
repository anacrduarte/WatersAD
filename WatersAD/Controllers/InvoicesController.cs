using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WatersAD.Data;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;

namespace WatersAD.Controllers
{
    public class InvoicesController : Controller
    {
       
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicesController(IInvoiceRepository invoiceRepository)
        {
            
            _invoiceRepository = invoiceRepository;
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
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
           
            return View();
        }

        // POST: Invoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InvoiceDate,ClientId,Issued,Sent,TotalAmount")] Invoice invoice)
        {
            if (ModelState.IsValid)
            {
               await _invoiceRepository.CreateAsync(invoice);
                return RedirectToAction(nameof(Index));
            }
           
            return View(invoice);
        }

        // GET: Invoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
            if (invoice == null)
            {
                return NotFound();
            }
       
            return View(invoice);
        }

        // POST: Invoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice)
        {
            if (id != invoice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _invoiceRepository.UpdateAsync(invoice);
                }
                catch (DbUpdateConcurrencyException)
                {
                   
                        return NotFound();
                   
                }
                return RedirectToAction(nameof(Index));
            }

            return View(invoice);
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _invoiceRepository.GetByIdAsync(id.Value);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice != null)
            {
                await _invoiceRepository.DeleteAsync(invoice);
            }

            
            return RedirectToAction(nameof(Index));
        }

      
    }
}
