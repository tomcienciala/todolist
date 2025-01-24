using Microsoft.AspNetCore.Mvc;
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
        return Ok(tasksService.Create(createTaskDto));
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<GetTaskDto>> GetList()
    {
        return Ok(tasksService.GetList());
    }

    [HttpPatch("{id}")]
    public ActionResult Update(Guid id, [FromBody] UpdateTaskStatusDto updateTaskStatusDto)
    {
        tasksService.UpdateStatus(id, updateTaskStatusDto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(Guid id)
    {
        tasksService.Delete(id);
        return Ok();
    }
}