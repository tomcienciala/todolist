using FluentAssertions;
using Moq;
using ToDoList.Application.Exceptions;
using ToDoList.Application.Services;
using ToDoList.Data.Entities;
using ToDoList.Data.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Application.Tests.Services;

public class TasksServiceTests
{
    private readonly Mock<ITasksRepository> tasksRepositoryMock;
    private readonly TasksService tasksService;
    
    public TasksServiceTests()
    {
        tasksRepositoryMock = new Mock<ITasksRepository>();
        tasksService = new TasksService(tasksRepositoryMock.Object);
    }

    [Fact]
    public void Create_ReturnsSameIdAsInEntity_WhenTaskIsCreated()
    {
        // Arrange
        var taskDto = new CreateTaskDto()
        {
            Name = "Test Name",
            Description = "Test Desc",
            DueDate = DateTime.Now,
        };
        
        var expectedId = Guid.Empty;
        tasksRepositoryMock.Setup(repo => repo.Create(It.IsAny<TaskEntity>()))
            .Callback<TaskEntity>(task => expectedId = task.Id);
        
        tasksRepositoryMock.Setup(repo => repo.Save())
            .Verifiable();

        // Act
        var result = tasksService.Create(taskDto);

        // Assert
        result.Should().Be(expectedId);
        tasksRepositoryMock.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public void GetList_ReturnsTaskList_WhenMethodIsCalled()
    {
        // Arrange
        var expectedTasks = new List<TaskEntity>
        {
            new TaskEntity { Id = Guid.NewGuid(), Name = "Task 1", Description = "Test Task 1", DueDate = DateTime.Now },
            new TaskEntity { Id = Guid.NewGuid(), Name = "Task 2", Description = "Test Task 2", DueDate = DateTime.Now.AddDays(1) },
            new TaskEntity { Id = Guid.NewGuid(), Name = "Task 3", Description = "Test Task 3", DueDate = DateTime.Now.AddDays(2) },
            new TaskEntity { Id = Guid.NewGuid(), Name = "Task 4", Description = "Test Task 4", DueDate = DateTime.Now.AddDays(3) }
        };
        
        tasksRepositoryMock.Setup(repo => repo.GetAll())
            .Returns(expectedTasks);
        
        // Act
        var result = tasksService.GetList();
        
        // Assert
        result.Should().BeEquivalentTo(expectedTasks);
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateTaskStatus_WhenMethodIsCalled()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateTaskStatusDto = new UpdateTaskStatusDto()
        {
            IsCompleted = true,
        };
        
        var taskEntity = new TaskEntity()
        {
            Id = id,
            Name = "Task 1",
            Description = "Test Description",
            DueDate = DateTime.Now,
            IsCompleted = false,
        };
        tasksRepositoryMock.Setup(repo => repo.Get(It.IsAny<Guid>()))
            .Returns(taskEntity);
        
        var actualTask = new TaskEntity();
        tasksRepositoryMock.Setup(repo => repo.Update(It.IsAny<TaskEntity>()))
            .Callback<TaskEntity>(task => actualTask = task);
        
        tasksRepositoryMock.Setup(repo => repo.Save())
            .Verifiable();
        
        var expectedTask = new TaskEntity()
        {
            Id = taskEntity.Id,
            Name = taskEntity.Name,
            Description = taskEntity.Description,
            DueDate = taskEntity.DueDate,
            IsCompleted = true,
        };
        
        // Act
        tasksService.UpdateStatus(id, updateTaskStatusDto);
        
        // Assert
        actualTask.Should().BeEquivalentTo(expectedTask);
        tasksRepositoryMock.Verify(repo => repo.Save(), Times.Once);
    }

    [Fact]
    public void UpdateStatus_ThrowsTaskNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        tasksRepositoryMock.Setup(repo => repo.Get(It.IsAny<Guid>()))
            .Returns((TaskEntity)null);
        
        // Act
        var act = () => tasksService.UpdateStatus(id, new UpdateTaskStatusDto());
        
        // Assert
        act.Should().Throw<TaskNotFoundException>().WithMessage($"Task with ID {id} not found.");
    }

    [Fact]
    public void Delete_ShouldDeleteTask_WhenMethodIsCalled()
    {
        // Arrange
        var id = Guid.NewGuid();

        var taskEntity = new TaskEntity()
        {
            Id = id,
            Name = "Task 1",
            Description = "Test Description",
            DueDate = DateTime.Now,
            IsCompleted = false,
        };
        tasksRepositoryMock.Setup(repo => repo.Get(It.IsAny<Guid>()))
            .Returns(taskEntity);
        
        tasksRepositoryMock.Setup(repo => repo.Delete(id))
            .Verifiable();
        tasksRepositoryMock.Setup(repo => repo.Save())
            .Verifiable();
        
        // Act
        tasksService.Delete(id);
        
        // Assert
        tasksRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Once);
        tasksRepositoryMock.Verify(repo => repo.Save(), Times.Once);
    }
    
    [Fact]
    public void Delete_ThrowsTaskNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        tasksRepositoryMock.Setup(repo => repo.Get(It.IsAny<Guid>()))
            .Returns((TaskEntity)null);
        
        // Act
        var act = () => tasksService.Delete(id);
        
        // Assert
        act.Should().Throw<TaskNotFoundException>().WithMessage($"Task with ID {id} not found.");
    }
}