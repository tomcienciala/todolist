using ToDoList.Application.Exceptions;
using ToDoList.Application.Interfaces;
using ToDoList.Data.Entities;
using ToDoList.Data.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Application.Services;

/// <summary>
/// Provides services for creating, retrieving, updating, and deleting tasks.
/// </summary>
public class TasksService : ITasksService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepository<TaskEntity> tasksRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="TasksService"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work used for transaction management.</param>
    public TasksService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
        tasksRepository = unitOfWork.GetRepository<TaskEntity>();
    }
    
    /// <summary>
    /// Creates a new task asynchronously.
    /// </summary>
    /// <param name="taskDto">The data transfer object containing the task details.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the task creation process.</param>
    /// <returns>The ID of the newly created task.</returns>
    /// <exception cref="GuidAlreadyExistsException">Thrown when a task with the generated GUID already exists.</exception>
    public async Task<Guid> CreateAsync(CreateTaskDto taskDto, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        if (await tasksRepository.GetByIdAsync(id, cancellationToken) is not null)
        {
            throw new GuidAlreadyExistsException($"Task with Guid: {id} already exists.");
        }

        var task = new TaskEntity()
        {
            Id = id,
            Name = taskDto.Name,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            IsCompleted = false
        };
        
        tasksRepository.Add(task);
        await unitOfWork.SaveAsync(cancellationToken);
        return id;
    }
    
    /// <summary>
    /// Retrieves a list of all tasks asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the task retrieval process.</param>
    /// <returns>A list of tasks represented by <see cref="GetTaskDto"/>.</returns>
    public async Task<IEnumerable<GetTaskDto>> GetListAsync(CancellationToken cancellationToken)
    {
        var taskList = await tasksRepository.GetAllAsync(cancellationToken);
        
        return taskList.Select(task => new GetTaskDto()
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted
        });
    }

    /// <summary>
    /// Updates the completion status of a task asynchronously.
    /// </summary>
    /// <param name="id">The ID of the task to update.</param>
    /// <param name="taskDto">The data transfer object containing the new completion status.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the update process.</param>
    /// <exception cref="TaskNotFoundException">Thrown when the task with the specified ID is not found.</exception>
    public async Task UpdateStatusAsync(Guid id, UpdateTaskStatusDto taskDto, CancellationToken cancellationToken)
    {
        var task = await tasksRepository.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new TaskNotFoundException($"Task with ID {id} not found.");
        }
        
        task.IsCompleted = taskDto.IsCompleted;
        tasksRepository.Update(task);
        await unitOfWork.SaveAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a task asynchronously.
    /// </summary>
    /// <param name="id">The ID of the task to delete.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the deletion process.</param>
    /// <exception cref="TaskNotFoundException">Thrown when the task with the specified ID is not found.</exception>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var task = await tasksRepository.GetByIdAsync(id, cancellationToken);
        if (task == null)
        {
            throw new TaskNotFoundException($"Task with ID {id} not found.");
        }
        
        tasksRepository.Delete(task);
        await unitOfWork.SaveAsync(cancellationToken);
    }
}