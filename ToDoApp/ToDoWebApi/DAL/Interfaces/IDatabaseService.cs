using ToDoWebApi.BLL.Models;

namespace ToDoWebApi.DAL.Interfaces
{
    public interface IDatabaseService
    {
        public ToDoDTO Create(ToDoDTO toDoDTO);

        public List<ToDoDTO> Get(bool excludeDone = false);

        public ToDoDTO Get(string id);

        public ToDoDTO Update(ToDoDTO toDoDTO);

        public void Delete(string id);
    }
}
