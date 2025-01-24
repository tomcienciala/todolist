using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoList.Api.Controllers;
using ToDoList.Application.Exceptions;
using ToDoList.Application.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Api.Tests.Controllers;

public class TasksControllerTests
{
    private readonly Mock<ITasksService> mockTaskService;
    private readonly TasksController controller;

    public TasksControllerTests()
    {
        mockTaskService = new Mock<ITasksService>();
        controller = new TasksController(mockTaskService.Object);
    }
    
    [Fact]
    public void Create_ReturnsOkResult_WithTaskId()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "New Task",
            Description = "Task Description",
            DueDate = DateTime.Now.AddDays(1)
        };
        
        var taskId = Guid.NewGuid();
        mockTaskService.Setup(service => service.Create(It.IsAny<CreateTaskDto>())).Returns(taskId);

        // Act
        var result = controller.Create(createTaskDto);

        // Assert
        result.Should().BeOfType<ActionResult<Guid>>()
            .Which.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(taskId);
    }
    
    [Fact]
    public void GetList_ReturnsEmptyList_WhenNoTasksExist()
    {
        // Arrange
        mockTaskService.Setup(service => service.GetList()).Returns(new List<GetTaskDto>());

        // Act
        var result = controller.GetList();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<GetTaskDto>>>()
            .Which.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<GetTaskDto>>()
            .Which.Should().BeEmpty();
    }
    
    [Fact]
    public void Update_ReturnsOkResult_WhenValidData()
    {
        // Arrange
        var updateTaskStatusDto = new UpdateTaskStatusDto { IsCompleted = true };
        var taskId = Guid.NewGuid();
        mockTaskService.Setup(service => service.UpdateStatus(It.IsAny<Guid>(), It.IsAny<UpdateTaskStatusDto>()))
            .Verifiable();

        // Act
        var result = controller.Update(taskId, updateTaskStatusDto);

        // Assert
        result.Should().BeOfType<OkResult>();

        mockTaskService.Verify();
    }
    
    [Fact]
    public void Update_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var updateTaskStatusDto = new UpdateTaskStatusDto { IsCompleted = true };
        var taskId = Guid.NewGuid();
        mockTaskService.Setup(service => service.UpdateStatus(It.IsAny<Guid>(), It.IsAny<UpdateTaskStatusDto>()))
            .Throws(new TaskNotFoundException($"Task {taskId} not found."));

        // Act
        var result = controller.Update(taskId, updateTaskStatusDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task {taskId} not found." });
    }
    
    [Fact]
    public void Delete_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        mockTaskService.Setup(service => service.Delete(taskId))
            .Throws(new TaskNotFoundException($"Task {taskId} not found."));

        // Act
        var result = controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task {taskId} not found." });
    }

    [Fact]
    public void Delete_ReturnsNoContent_WhenTaskDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        mockTaskService.Setup(service => service.Delete(It.IsAny<Guid>())).Verifiable();

        // Act
        var result = controller.Delete(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        
        mockTaskService.Verify(service => service.Delete(taskId), Times.Once);
    }
}