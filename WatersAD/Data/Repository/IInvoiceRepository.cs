using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        /// <summary>
        /// Asynchronously retrieves all invoices along with the associated client information.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="Invoice"/> objects, each including the associated client details.</returns>
        Task<ICollection<Invoice>> GetAllInvoicesAndClientAsync();


        /// <summary>
        /// Asynchronously retrieves the details of a specific invoice for the given client ID.
        /// </summary>
        /// <param name="idClient">The unique identifier of the client.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Invoice"/> object with detailed information for the specified client.</returns>
        Task<Invoice> GetDetailsInvoiceAsync(int idClient);

        Task<Invoice> GetInvoicesForClient(int clientId);
    }
}
