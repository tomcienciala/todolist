using Microsoft.AspNetCore.Mvc;
using ToDoList.Application.Exceptions;
using ToDoList.Application.Interfaces;
using ToDoList.Shared.Dto;

namespace ToDoList.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITasksService tasksService;

    public TasksController(ITasksService tasksService)
    {
        this.tasksService = tasksService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateAsync([FromBody] CreateTaskDto createTaskDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var taskId = await tasksService.CreateAsync(createTaskDto, cancellationToken);
            return Ok(taskId);
        }
        catch (GuidAlreadyExistsException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetTaskDto>>> GetListAsync(CancellationToken cancellationToken)
    {
        var taskList = await tasksService.GetListAsync(cancellationToken);
        return Ok(taskList);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateTaskStatusDto updateTaskStatusDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await tasksService.UpdateStatusAsync(id, updateTaskStatusDto, cancellationToken);
            return Ok();
        }
        catch (TaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await tasksService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (TaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}