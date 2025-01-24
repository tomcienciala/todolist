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
    public ActionResult<Guid> Create([FromBody] CreateTaskDto createTaskDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var taskId = tasksService.Create(createTaskDto);
        return Ok(taskId);
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<GetTaskDto>> GetList()
    {
        var taskList = tasksService.GetList();
        return Ok(taskList);
    }

    [HttpPatch("{id}")]
    public ActionResult Update(Guid id, [FromBody] UpdateTaskStatusDto updateTaskStatusDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            tasksService.UpdateStatus(id, updateTaskStatusDto);
            return Ok();
        }
        catch (TaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(Guid id)
    {
        try
        {
            tasksService.Delete(id);
            return NoContent();
        }
        catch (TaskNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}