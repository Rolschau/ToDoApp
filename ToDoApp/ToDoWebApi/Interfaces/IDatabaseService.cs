using ToDoWebApi.Models;
using Microsoft.Extensions.Options;
using System.Linq;
using Hanssens.Net;
using System.Collections.Generic;

namespace ToDoWebApi.Services
{
    public interface IDatabaseService
    {
        public Task<ToDoDTO> CreateAsync(ToDoDTO toDoDto);

        public Task<List<ToDoDTO>> GetAsync();
        
        public Task<ToDoDTO> GetAsync(string id);

        public Task UpdateAsync(ToDoDTO toDoDTO);

        public Task DeleteAsync(string id);
    }
}
