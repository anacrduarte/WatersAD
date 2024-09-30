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

     
      
    }
}
