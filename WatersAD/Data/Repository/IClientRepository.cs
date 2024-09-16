using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        IEnumerable<Client> GetAllWithLocalitiesAndWaterMeter();

        Task<Client> GetClientAndLocalityAndCityAsync(int clientId);

        Task<Client> GetClientWithWaterMeter(int clientId);

        IEnumerable<Client> GetAllWithLocalitiesAndWaterMeterInactive();
    }
}
