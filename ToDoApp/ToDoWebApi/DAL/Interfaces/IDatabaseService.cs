using Microsoft.Extensions.Options;
using System.Linq;
using Hanssens.Net;
using System.Collections.Generic;
using ToDoWebApi.BLL.Models;

namespace ToDoWebApi.DAL.Interfaces
{
    public interface IDatabaseService
    {
        public Task<ToDoDTO> CreateAsync(ToDoDTO toDoDTO);

        public Task<List<ToDoDTO>> GetAsync(bool excludeDone = false, bool sortDescending = false);

        public Task<ToDoDTO> GetAsync(string id);

        public Task<ToDoDTO> UpdateAsync(ToDoDTO toDoDTO);

        public Task DeleteAsync(string id);
    }
}
