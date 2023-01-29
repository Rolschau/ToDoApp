using System.Runtime.Serialization;

namespace ToDoWebApi.BLL.Interfaces
{
    public interface IToDo
    {
        public string? Id { get; set; }

        public string? Value { get; set; }

        public bool Done { get; set; }

        public int Order { get; set; }
    }
}