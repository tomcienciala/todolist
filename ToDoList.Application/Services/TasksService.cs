using ToDoList.Application.Exceptions;
using ToDoList.Application.Interfaces;
using ToDoList.Data.Entities;
using ToDoList.Data.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Application.Services;

public class TasksService : ITasksService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IRepository<TaskEntity> tasksRepository;

    public TasksService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
        tasksRepository = unitOfWork.GetRepository<TaskEntity>();
    }
    
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
            IsCompleted = true
        };
        
        tasksRepository.Add(task);
        await unitOfWork.SaveAsync(cancellationToken);
        return id;
    }

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