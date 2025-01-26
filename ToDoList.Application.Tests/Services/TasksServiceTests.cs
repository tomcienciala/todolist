using FluentAssertions;
using Moq;
using ToDoList.Application.Exceptions;
using ToDoList.Application.Services;
using ToDoList.Data.Entities;
using ToDoList.Data.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Application.Tests.Services
{
    public class TasksServiceTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IRepository<TaskEntity>> tasksRepositoryMock;
        private readonly TasksService tasksService;

        public TasksServiceTests()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            tasksRepositoryMock = new Mock<IRepository<TaskEntity>>();
            unitOfWorkMock.Setup(uow => uow.GetRepository<TaskEntity>()).Returns(tasksRepositoryMock.Object);
            tasksService = new TasksService(unitOfWorkMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ReturnsSameIdAsInEntity_WhenTaskIsCreated()
        {
            // Arrange
            var taskDto = new CreateTaskDto()
            {
                Name = "Test Name",
                Description = "Test Desc",
                DueDate = DateTime.Now,
            };

            var expectedId = Guid.NewGuid();
            tasksRepositoryMock.Setup(repo => repo.Add(It.IsAny<TaskEntity>()))
                .Callback<TaskEntity>(task => expectedId = task.Id);

            unitOfWorkMock.Setup(uow => uow.SaveAsync(It.IsAny<CancellationToken>()))
                .Verifiable();
            
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await tasksService.CreateAsync(taskDto, cancellationToken);

            // Assert
            result.Should().Be(expectedId);
            unitOfWorkMock.Verify(uow => uow.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task CreateAsync_ThrowsGuidAlreadyExistsException_WhenGuidAlreadyExists()
        {
            // Arrange
            var taskDto = new CreateTaskDto()
            {
                Name = "Test Name",
                Description = "Test Desc",
                DueDate = DateTime.Now,
            };
            
            var generatedId = Guid.NewGuid();
            tasksRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Callback<Guid, CancellationToken>((id, token) => generatedId = id)
                .ReturnsAsync(new TaskEntity());
            
            var cancellationToken = CancellationToken.None;

            // Act
            var act = async () => await tasksService.CreateAsync(taskDto, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<GuidAlreadyExistsException>()
                .WithMessage($"Task with Guid: {generatedId} already exists.");
        }

        [Fact]
        public async Task GetListAsync_ReturnsTaskList_WhenMethodIsCalled()
        {
            // Arrange
            var expectedTasks = new List<TaskEntity>
            {
                new TaskEntity { Id = Guid.NewGuid(), Name = "Task 1", Description = "Test Task 1", DueDate = DateTime.Now },
                new TaskEntity { Id = Guid.NewGuid(), Name = "Task 2", Description = "Test Task 2", DueDate = DateTime.Now.AddDays(1) },
                new TaskEntity { Id = Guid.NewGuid(), Name = "Task 3", Description = "Test Task 3", DueDate = DateTime.Now.AddDays(2) },
                new TaskEntity { Id = Guid.NewGuid(), Name = "Task 4", Description = "Test Task 4", DueDate = DateTime.Now.AddDays(3) }
            };

            tasksRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedTasks);
            
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await tasksService.GetListAsync(cancellationToken);

            // Assert
            result.Should().BeEquivalentTo(expectedTasks.Select(task => new GetTaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                DueDate = task.DueDate,
                IsCompleted = task.IsCompleted
            }));
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateTaskStatus_WhenMethodIsCalled()
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

            tasksRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(taskEntity);

            tasksRepositoryMock.Setup(repo => repo.Update(It.IsAny<TaskEntity>()))
                .Callback<TaskEntity>(task => task.IsCompleted = true);

            unitOfWorkMock.Setup(uow => uow.SaveAsync(It.IsAny<CancellationToken>()))
                .Verifiable();
            
            var cancellationToken = CancellationToken.None;

            // Act
            await tasksService.UpdateStatusAsync(id, updateTaskStatusDto, cancellationToken);

            // Assert
            taskEntity.IsCompleted.Should().BeTrue();
            unitOfWorkMock.Verify(uow => uow.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusAsync_ThrowsTaskNotFoundException_WhenTaskDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            tasksRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskEntity)null);

            var cancellationToken = CancellationToken.None;
            
            // Act
            var act = async () => await tasksService.UpdateStatusAsync(id, new UpdateTaskStatusDto(), cancellationToken);

            // Assert
            await act.Should().ThrowAsync<TaskNotFoundException>()
                .WithMessage($"Task with ID {id} not found.");
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTask_WhenMethodIsCalled()
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

            tasksRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(taskEntity);

            tasksRepositoryMock.Setup(repo => repo.Delete(It.IsAny<TaskEntity>()))
                .Verifiable();

            unitOfWorkMock.Setup(uow => uow.SaveAsync(It.IsAny<CancellationToken>()))
                .Verifiable();
            
            var cancellationToken = CancellationToken.None;

            // Act
            await tasksService.DeleteAsync(id, cancellationToken);

            // Assert
            tasksRepositoryMock.Verify(repo => repo.Delete(It.IsAny<TaskEntity>()), Times.Once);
            unitOfWorkMock.Verify(uow => uow.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsTaskNotFoundException_WhenTaskDoesNotExist()
        {
            // Arrange
            var id = Guid.NewGuid();
            tasksRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TaskEntity)null);

            var cancellationToken = CancellationToken.None;
            
            // Act
            var act = async () => await tasksService.DeleteAsync(id, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<TaskNotFoundException>()
                .WithMessage($"Task with ID {id} not found.");
        }
    }
}
