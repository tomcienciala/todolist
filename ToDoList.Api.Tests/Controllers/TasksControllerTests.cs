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
        tasksServiceMock.Setup(service => service.CreateAsync(It.IsAny<CreateTaskDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskId);
        
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await controller.CreateAsync(createTaskDto, cancellationToken);

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
        tasksServiceMock.Setup(service => service.CreateAsync(It.IsAny<CreateTaskDto>(), It.IsAny<CancellationToken>()))
            .Throws(new GuidAlreadyExistsException($"Task with Guid: {taskId} already exists."));
        
        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await controller.CreateAsync(createTaskDto, cancellationToken);
        
        // Assert
        result.Should().BeOfType<ActionResult<Guid>>()
            .Which.Result.Should().BeOfType<ConflictObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task with Guid: {taskId} already exists." });
    }
    
    [Fact]
    public async Task GetListAsync_ReturnsEmptyList_WhenNoTasksExist()
    {
        // Arrange
        tasksServiceMock.Setup(service => service.GetListAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<GetTaskDto>());

        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await controller.GetListAsync(cancellationToken);

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
        tasksServiceMock.Setup(service => service.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateTaskStatusDto>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await controller.UpdateAsync(taskId, updateTaskStatusDto, cancellationToken);

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
        tasksServiceMock.Setup(service => service.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateTaskStatusDto>(), It.IsAny<CancellationToken>()))
            .Throws(new TaskNotFoundException($"Task {taskId} not found."));

        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await controller.UpdateAsync(taskId, updateTaskStatusDto, cancellationToken);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task {taskId} not found." });
    }
    
    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Throws(new TaskNotFoundException($"Task {taskId} not found."));

        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await controller.DeleteAsync(taskId, cancellationToken);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>()
            .Which.Value.Should().BeEquivalentTo(new { message = $"Task {taskId} not found." });
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenTaskDeleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        tasksServiceMock.Setup(service => service.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Verifiable();

        var cancellationToken = CancellationToken.None;
        
        // Act
        var result = await controller.DeleteAsync(taskId, cancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        
        tasksServiceMock.Verify(service => service.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}