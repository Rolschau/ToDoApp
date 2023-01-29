using ToDoWebApi.BLL.Models;
using Microsoft.Extensions.Options;
using System.Linq;
using Hanssens.Net;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ToDoWebApi.DAL.Interfaces;
using ToDoWebApi.DAL.Models;
using ToDoWebApi.Controllers;
using System.Reflection;
using System.Diagnostics;
using ToDoWebApi.BLL.Sorters;

namespace ToDoWebApi.DAL.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<ToDoController> _logger;
        private readonly LocalStorage _localStorage;

        //public record struct ToDo(string? Id, string? Value, bool? Done, int? Order);

        /// <summary>
        /// Converts a ToDo DTO to a ToDo item. This separates the item format externally and internally toward storage.
        /// </summary>
        /// <param name="toDoDTO"></param>
        /// <returns>A ToDo item matching the format in the database</returns>
        private ToDo Convert(ToDoDTO toDoDTO) => new() { Id = toDoDTO.Id, Value = toDoDTO.Value, Done = toDoDTO.Done, Order = toDoDTO.Order };

        /// <summary>
        /// Converts a ToDo to a ToDo DTO item. This separates the item format externally and internally toward storage.
        /// </summary>
        /// <param name="toDo"></param>
        /// <returns>A ToDoDTO item matching the format used externally</returns>
        private ToDoDTO Convert(ToDo toDo) => new() { Id = toDo.Id, Value = toDo.Value, Done = toDo.Done, Order = toDo.Order };

        /// <summary>
        /// Gets a new id.
        /// <para>Note: Remember to check if the id exists at the destination, keep trying until success.</para>
        /// </summary>
        /// <returns>A string of current UTC ticks</returns>
        private string GetNewId() => DateTimeOffset.Now.Ticks.ToString();


        //public DatabaseService(IOptions<xxxDBSettings> xxxDBSettings)
        public DatabaseService(ILocalStorageConfiguration localStorageConfiguration, ILogger<ToDoController> logger)
        {
            _logger = logger;
            
            // initialize LocalStorage with a password of your choice
            _localStorage = new LocalStorage(localStorageConfiguration, "todopassword");
        }

        /// <summary>
        /// Create a todo in the database.
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public Task<ToDoDTO> CreateAsync(ToDoDTO toDoDTO)
        {
            while (toDoDTO.Id is null || _localStorage.Exists(toDoDTO.Id))
                toDoDTO.Id = GetNewId();
            _localStorage.Store(key: toDoDTO.Id, instance: Convert(toDoDTO));

            return Task.FromResult((ToDoDTO)toDoDTO.Clone());
        }

        /// <summary>
        /// Return all todos from database.
        /// </summary>
        /// <returns></returns>
        public Task<List<ToDoDTO>> GetAsync(bool excludeDone = false, bool sortDescending = false)
        {
            List<ToDoDTO> todos = new();
            foreach (string key in _localStorage.Keys())
            {
                var todo = Convert(_localStorage.Get<ToDo>(key));
                if (excludeDone && todo.Done)
                    continue;
                todos.Add(todo);
            }

            {
                Stopwatch stopWatch = new();
                stopWatch.Start();
                todos.Sort(new ToDoComparer());
                stopWatch.Stop();
                Console.WriteLine("Sorting with ToDoComparer took " + stopWatch.ElapsedTicks);
            }

            {
                Stopwatch stopWatch = new();
                stopWatch.Start();
                if (sortDescending)
                    todos = todos.OrderByDescending(x => x.Order).ToList();
                else
                    todos = todos.OrderBy(x => x.Order).ToList();
                stopWatch.Stop();
                Console.WriteLine("OrderBy took " + stopWatch.ElapsedTicks);
            }

            return Task.FromResult(todos);
        }

        /// <summary>
        /// Return a todo with a specific id from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ToDoDTO> GetAsync(string id) => Task.FromResult(Convert(_localStorage.Get<ToDo>(key: id)));

        /// <summary>
        /// Updates (or creates if not found) a ToDo item in the database.
        /// </summary>
        /// <param name="toDoDTO"></param>
        /// <returns></returns>
        public Task<ToDoDTO> UpdateAsync(ToDoDTO toDoDTO)
        {
            _localStorage.Store(toDoDTO.Id, Convert(toDoDTO));
            return Task.FromResult(toDoDTO);
        }

        /// <summary>
        /// Deletes a todo item with a specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task DeleteAsync(string id) => Task.Factory.StartNew(() => _localStorage.Remove(key: id));

        /// <summary>
        /// Deletes the database with all the todo items.
        /// </summary>
        /// <returns></returns>
        public Task DeleteDatabaseAsync() => Task.Factory.StartNew(() => _localStorage.Destroy());
    }
}
