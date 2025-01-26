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
    private readonly Mock<ITasksService> tasksServiceMock;
    private readonly TasksController controller;

    public TasksControllerTests()
    {
        tasksServiceMock = new Mock<ITasksService>();
        controller = new TasksController(tasksServiceMock.Object);
    }
    
    [Fact]
    public async Task CreateAsync_ReturnsOkResult_WithTaskId()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "New Task",
            Description = "Task Description",
            DueDate = DateTime.Now.AddDays(1)
        };
        
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.CreateAsync(It.IsAny<CreateTaskDto>())).ReturnsAsync(taskId);

        // Act
        var result = await controller.CreateAsync(createTaskDto);

        // Assert
        result.Should().BeOfType<ActionResult<Guid>>()
            .Which.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(taskId);
    }

    [Fact]
    public async Task CreateAsync_ThrowsException_WhenNewGuidConflicts()
    {
        // Arrange
        var createTaskDto = new CreateTaskDto
        {
            Name = "New Task",
            Description = "Task Description",
            DueDate = DateTime.Now.AddDays(1)
        };
        
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.CreateAsync(It.IsAny<CreateTaskDto>()))
            .Throws(new GuidAlreadyExistsException($"Task with Guid: {taskId} already exists."));
        
        // Act
        var result = await controller.CreateAsync(createTaskDto);
        
        // Assert
        result.Should().BeOfType<ActionResult<Guid>>()
            .Which.Result.Should().BeOfType<ConflictObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task with Guid: {taskId} already exists." });
    }
    
    [Fact]
    public async Task GetListAsync_ReturnsEmptyList_WhenNoTasksExist()
    {
        // Arrange
        tasksServiceMock.Setup(service => service.GetListAsync()).ReturnsAsync(new List<GetTaskDto>());

        // Act
        var result = await controller.GetListAsync();

        // Assert
        result.Should().BeOfType<ActionResult<IEnumerable<GetTaskDto>>>()
            .Which.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeAssignableTo<IEnumerable<GetTaskDto>>()
            .Which.Should().BeEmpty();
    }
    
    [Fact]
    public async Task UpdateAsync_ReturnsOkResult_WhenValidData()
    {
        // Arrange
        var updateTaskStatusDto = new UpdateTaskStatusDto { IsCompleted = true };
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateTaskStatusDto>()))
            .Verifiable();

        // Act
        var result = await controller.UpdateAsync(taskId, updateTaskStatusDto);

        // Assert
        result.Should().BeOfType<OkResult>();

        tasksServiceMock.Verify();
    }
    
    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var updateTaskStatusDto = new UpdateTaskStatusDto { IsCompleted = true };
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateTaskStatusDto>()))
            .Throws(new TaskNotFoundException($"Task {taskId} not found."));

        // Act
        var result = await controller.UpdateAsync(taskId, updateTaskStatusDto);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task {taskId} not found." });
    }
    
    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.DeleteAsync(taskId))
            .Throws(new TaskNotFoundException($"Task {taskId} not found."));

        // Act
        var result = await controller.DeleteAsync(taskId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task {taskId} not found." });
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenTaskDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.DeleteAsync(It.IsAny<Guid>())).Verifiable();

        // Act
        var result = await controller.DeleteAsync(taskId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        
        tasksServiceMock.Verify(service => service.DeleteAsync(taskId), Times.Once);
    }
}