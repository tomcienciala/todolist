using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ToDoList.Data.Entities;

public class TaskEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
}