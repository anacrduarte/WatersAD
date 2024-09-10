using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface IClientRepository : IGenericRepository<Client>
    {
        IEnumerable<Client> GetAllWithLocalities();
    }
}
