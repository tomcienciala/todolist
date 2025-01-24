using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Dto;

public class UpdateTaskStatusDto
{
    [Required]
    public bool IsCompleted { get; set; }
}