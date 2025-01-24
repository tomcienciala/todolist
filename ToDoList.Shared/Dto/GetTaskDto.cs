using System.ComponentModel.DataAnnotations;

namespace ToDoList.Shared.Dto;

public class GetTaskDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    [Required(ErrorMessage = "Status is required")]
    public bool IsCompleted { get; set; }
}