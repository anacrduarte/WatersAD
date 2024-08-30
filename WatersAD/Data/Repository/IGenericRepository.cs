namespace WatersAD.Data.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Get all results for entity T
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Get entity for id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Create an entity type T(generic)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CreateAsync(T entity);

        /// <summary>
        /// Update entidade do tipo T
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Delete entity from database
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns></returns>
        Task DeleteAsync(T entity);

        /// <summary>
        ///Know if the entity still exists
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>True or False</returns>
        Task<bool> ExistAsync(int id);
    }
}
