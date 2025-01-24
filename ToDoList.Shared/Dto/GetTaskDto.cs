using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Dto;

public class GetTaskDto
{
    [Required]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool IsCompleted { get; set; }
}