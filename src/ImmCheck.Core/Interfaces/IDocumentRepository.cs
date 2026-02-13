namespace ImmCheck.Core.Interfaces;

public interface IDocumentRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(long id);
    Task<T> CreateAsync(T entity);
    Task<T?> UpdateAsync(long id, T entity);
    Task<bool> DeleteAsync(long id);
}
