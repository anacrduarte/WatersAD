using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface ITierRepository: IGenericRepository<Tier>
    {
        Task<List<Tier>> GetAllAsync();

        Task<Tier?> GetMatchingTierAsync(double consumptionValue);
    }
}
