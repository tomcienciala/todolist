using ToDoList.Shared.Dto;

namespace ToDoList.Application.Interfaces;

public interface ITasksService
{
    public Task<Guid> CreateAsync(CreateTaskDto taskDto, CancellationToken cancellationToken);
    public Task<IEnumerable<GetTaskDto>> GetListAsync(CancellationToken cancellationToken);
    public Task UpdateStatusAsync(Guid id, UpdateTaskStatusDto taskDto, CancellationToken cancellationToken);
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}