using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        IEnumerable<Client> GetAllWithLocalitiesAndWaterMeter();

        //IQueryable<Client> GetAllActive();

        //IQueryable<Client> GetAllInactive();

        Task<Client> GetClientAndLocalityAndCityAsync(int clientId);

        Task<Client> GetClientWithWaterMeter(int clientId);
    }
}
