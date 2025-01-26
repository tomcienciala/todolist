namespace ToDoList.Application.Exceptions;

public class GuidAlreadyExistsException : Exception
{
    public GuidAlreadyExistsException(string message) : base(message) { }
}