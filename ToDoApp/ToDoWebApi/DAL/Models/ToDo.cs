namespace ToDoWebApi.DAL.Models
{
    public record struct ToDo(string? Id, string? Value, bool Done, int Order);
}