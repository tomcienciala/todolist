namespace ToDoList.Data.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}