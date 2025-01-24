using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Dto;

public class CreateTaskDto
{
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
}