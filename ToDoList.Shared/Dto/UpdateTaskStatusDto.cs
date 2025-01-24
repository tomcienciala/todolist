using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Dto;

public class UpdateTaskStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public bool IsCompleted { get; set; }
}