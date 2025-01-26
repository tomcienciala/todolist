using Microsoft.EntityFrameworkCore;
using ToDoList.Data.Interfaces;

namespace ToDoList.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext context;
    private readonly Dictionary<Type, object> repositories = new();

    public UnitOfWork(DbContext context)
    {
        this.context = context;
    }
    
    public IRepository<T> GetRepository<T>() where T : class
    {
        if (!repositories.ContainsKey(typeof(T)))
        {
            var repository = new Repository<T>(context);
            repositories.Add(typeof(T), repository);
        }
        
        return (IRepository<T>)repositories[typeof(T)];
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        context.Dispose();
    }
}