using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Charts;
using System.Collections;
using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        private readonly DataContext _context;

        public InvoiceRepository(DataContext context) :base(context) 
        {
            _context = context;
        }

        public async Task<ICollection<Invoice>> GetAllInvoicesAndClientAsync()
        {
            return await _context.Invoices.Include(c => c.Client).ToListAsync();
                
        }
        public async Task<Invoice> GetDetailsInvoiceAsync(int invoiceId)
        {
            return await _context.Invoices
                .Include(c=> c.Client)
                .FirstOrDefaultAsync(c => c.Id == invoiceId);

        }
    }
}
