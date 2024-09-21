using WatersAD.Data.Entities;

namespace WatersAD.Data.Repository
{
    public interface ITierRepository: IGenericRepository<Tier>
    {

        /// <summary>
        ///  Retrieves the tier that matches a given consumption value asynchronously.
        /// </summary>
        /// <param name="consumptionValue">The consumption value to match with a tier.</param>
        /// <returns>A <see cref="Task{Tier}"/> representing the asynchronous operation, containing the matching tier, or null if no match is found.</returns>
        Task<Tier> GetMatchingTierAsync(double consumptionValue);
    }
}
