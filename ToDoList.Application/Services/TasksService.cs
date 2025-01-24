using ToDoList.Application.Exceptions;
using ToDoList.Application.Interfaces;
using ToDoList.Data.Entities;
using ToDoList.Data.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Application.Services;

public class TasksService : ITasksService
{
    private readonly ITasksRepository tasksRepository;

    public TasksService(ITasksRepository tasksRepository)
    {
        this.tasksRepository = tasksRepository;
    }
    public Guid Create(CreateTaskDto taskDto)
    {
        var id = Guid.NewGuid();

        var task = new TaskEntity()
        {
            Id = id,
            Name = taskDto.Name,
            Description = taskDto.Description,
            DueDate = taskDto.DueDate,
            IsCompleted = true
        };
        
        tasksRepository.Create(task);
        tasksRepository.Save();
        return id;
    }

    public IEnumerable<GetTaskDto> GetList()
    {
        return tasksRepository.GetAll()
            .Select(task => new GetTaskDto()
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted
            });
    }

    public void UpdateStatus(Guid id, UpdateTaskStatusDto taskDto)
    {
        var task = tasksRepository.Get(id);
        if (task == null)
        {
            throw new TaskNotFoundException($"Task with ID {id} not found.");
        }
        
        task.IsCompleted = taskDto.IsCompleted;
        tasksRepository.Update(task);
        tasksRepository.Save();
    }

    public void Delete(Guid id)
    {
        var task = tasksRepository.Get(id);
        if (task == null)
        {
            throw new TaskNotFoundException($"Task with ID {id} not found.");
        }
        
        tasksRepository.Delete(id);
        tasksRepository.Save();
    }
}