using Microsoft.EntityFrameworkCore;
using ToDoList.Data.Interfaces;

namespace ToDoList.Data;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext context;
    private readonly DbSet<T> dbSet;
    
    public Repository(AppDbContext context)
    {
        this.context = context;
        this.dbSet = context.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    { 
        return await dbSet.ToListAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbSet.FindAsync(id, cancellationToken);
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