using Microsoft.EntityFrameworkCore;
using ImmCheck.Core.Interfaces;
using ImmCheck.Infrastructure.Data;

namespace ImmCheck.Infrastructure.Repositories;

public class DocumentRepository<T> : IDocumentRepository<T> where T : class
{
    private readonly AppDbContext _db;
    private readonly DbSet<T> _dbSet;

    public DocumentRepository(AppDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> CreateAsync(T entity)
    {
        _dbSet.Add(entity);
        await _db.SaveChangesAsync();
        return entity;
    }

    public async Task<T?> UpdateAsync(long id, T entity)
    {
        var existing = await _dbSet.FindAsync(id);
        if (existing == null) return null;

        _db.Entry(existing).CurrentValues.SetValues(entity);
        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var entity = await _dbSet.FindAsync(id);
        if (entity == null) return false;

        _dbSet.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}
