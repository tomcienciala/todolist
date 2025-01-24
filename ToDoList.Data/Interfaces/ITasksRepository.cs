using ToDoList.Data.Entities;

namespace ToDoList.Data.Interfaces;

public interface ITasksRepository : IDisposable
{
    IEnumerable<TaskEntity> GetAll();
    TaskEntity Get(Guid id);
    void Create(TaskEntity task);
    void Update(TaskEntity task);
    void Delete(Guid id);
    void Save();
}