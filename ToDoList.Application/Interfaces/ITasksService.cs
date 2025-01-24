using ToDoList.Shared.Dto;

namespace ToDoList.Application.Interfaces;

public interface ITasksService
{
    public Guid Create(CreateTaskDto taskDto);
    public IEnumerable<GetTaskDto> GetList();
    public void UpdateStatus(Guid id, UpdateTaskStatusDto taskDto);
    public void Delete(Guid id);
}