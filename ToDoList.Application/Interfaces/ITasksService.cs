using ToDoList.Shared.Dto;

namespace ToDoList.Application.Interfaces;

public interface ITasksService
{
    public Task<Guid> CreateAsync(CreateTaskDto taskDto);
    public Task<IEnumerable<GetTaskDto>> GetListAsync();
    public Task UpdateStatusAsync(Guid id, UpdateTaskStatusDto taskDto);
    public Task DeleteAsync(Guid id);
}