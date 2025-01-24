using ToDoList.Application.Exceptions;
using ToDoList.Application.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Application.Services;

public class TasksService : ITasksService
{
    public Guid Create(CreateTaskDto taskDto)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<GetTaskDto> GetList()
    {
        throw new NotImplementedException();
    }

    public void UpdateStatus(Guid id, UpdateTaskStatusDto taskDto)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new TaskNotFoundException($"Task with ID {id} not found.");
    }
}