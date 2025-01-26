using Microsoft.EntityFrameworkCore;
using ToDoList.Data.Interfaces;

namespace ToDoList.Data;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext context;
    private readonly DbSet<T> dbSet;
    
    public Repository(DbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync()
    { 
        return await dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await dbSet.FindAsync(id);
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public void Update(T entity)
    {
        dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        dbSet.Remove(entity);
    }
}