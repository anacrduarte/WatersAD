using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<ICollection<Invoice>> GetAllInvoicesAndClientAsync();

        Task<Invoice> GetDetailsInvoiceAsync(int idClient);
    }
}
