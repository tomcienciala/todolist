namespace ToDoList.Data.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
}