using ToDoWebApi.Models;
using Microsoft.Extensions.Options;
using System.Linq;
using Hanssens.Net;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ToDoWebApi.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly LocalStorage _localStorage;

        public record struct ToDo(string? Id, string? Value, bool? Done, int? Order);

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
        private ToDoDTO Convert(ToDo toDo) => new() { Id = toDo.Id, Value = toDo.Value, Done = toDo.Done ?? false, Order = toDo.Order };

        /// <summary>
        /// Gets a new id.
        /// <para>Note: Remember to check if the id exists at the destination, keep trying until success.</para>
        /// </summary>
        /// <returns>A string of current UTC ticks</returns>
        private string GetNewId() => new DateTimeOffset().Ticks.ToString();


        //public DatabaseService(IOptions<xxxDBSettings> xxxDBSettings)
        public DatabaseService(ILocalStorageConfiguration localStorageConfiguration)
        {
            /*
            // setup a configuration with encryption enabled (defaults to 'false')
            // note that adding EncryptionSalt is optional, but recommended
            if (localStorageConfiguration == default)
                localStorageConfiguration = new LocalStorageConfiguration()
                {
                    EnableEncryption = true,
                    EncryptionSalt = "todosalt",
                    Filename = "todo"
                    //ToDo: consider making a separate Filename per user id eg. SSO like IdentityServer4 (OAuth/OpenID protocols, ASP.NET Core).
                };
            */

            // initialize LocalStorage with a password of your choice
            _localStorage = new LocalStorage(localStorageConfiguration, "todopassword");
        }

        /// <summary>
        /// Create a todo in the database.
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public Task<ToDoDTO> CreateAsync(ToDoDTO toDoDto) {
            while (toDoDto.Id is null || _localStorage.Exists(toDoDto.Id))
                toDoDto.Id = GetNewId();
            _localStorage.Store(key: toDoDto.Id, instance: Convert(toDoDto));

            return Task.FromResult(toDoDto);
        }

        /// <summary>
        /// Return all todos from database.
        /// </summary>
        /// <returns></returns>
        public Task<List<ToDoDTO>> GetAsync()
        {
            List<ToDoDTO> todos = new();
            foreach (string key in _localStorage.Keys())
                todos.Add(Convert(_localStorage.Get<ToDo>(key)));
            
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
        public Task UpdateAsync(ToDoDTO toDoDTO) => Task.Factory.StartNew( () => _localStorage.Store(toDoDTO.Id, Convert(toDoDTO)));

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
