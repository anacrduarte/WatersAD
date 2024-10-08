using Microsoft.AspNetCore.Mvc;
using WatersAD.Data.Entities;
using WatersAD.Data.Repository;

namespace WatersAD.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoicesController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicesController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        [HttpGet("[action]/{clientId}")]
        public async Task<IActionResult> GetInvoices(int clientId)
        {
            var invoice = await _invoiceRepository.GetInvoicesForClient(clientId);

            if (invoice == null)
            {
                return NotFound("No unpaid invoices found for the specified client.");
            }

         
            return Ok(invoice);
        }
    }
}
