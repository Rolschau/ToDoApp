using ToDoWebApi.BLL.Models;
using Hanssens.Net;
using ToDoWebApi.DAL.Interfaces;
using ToDoWebApi.DAL.Models;
using ToDoWebApi.Controllers;
using ToDoWebApi.BLL.Sorters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace ToDoWebApi.DAL.Services
{
    // Version 1: 

    // Version 2 (suggestions for changes): 
    // "key" could be e.g. "todos" or maybe a UserID, that way "Query" is useable. https://github.com/hanssens/localstorage/issues/33
    //_localStorage.Query<ToDo>("todos", (todo) => todo.Id = id); //where _id is a specific todo item id.
    //_localStorage.Query<ToDo>("todos", (todo) => todo.Done = isDone); //where isDone

    public class DatabaseService : IDatabaseService
    {
        private readonly ILogger<ToDoController> _logger;
        private readonly LocalStorage _localStorage;
        private readonly ILocalStorageConfiguration _localStorageConfiguration;
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

        public DatabaseService(IOptions<LocalStorageConfiguration> localStorageConfigurationOptions, ILogger<ToDoController> logger)
        {
            _localStorageConfiguration = localStorageConfigurationOptions.Value;
            _logger = logger;

            // initialize LocalStorage with a password of your choice
            _localStorage = new LocalStorage(_localStorageConfiguration, "todopassword");
        }

        private void PersistRetry()
        {
            int maxAttempts = 10;
            int waitMilliseconds = 1000;
            for (int i = 1; i <= maxAttempts; i++)
            {
                try
                {
                    if (_localStorage.Count == 0) return; //https://github.com/hanssens/localstorage/issues/17
                    _localStorage.Persist();
                }
                catch
                {
                    Thread.Sleep(waitMilliseconds);
                    if (i == maxAttempts)
                    {
                        _logger.LogCritical("LocalStorage Persist failed to save the database after {milliseconds}ms", maxAttempts * waitMilliseconds);
                        throw;
                    }
                }
            }
        }

        private void LoadRetry()
        {
            int maxAttempts = 10;
            int waitMilliseconds = 1000;
            for (int i = 1; i <= maxAttempts; i++)
            {
                try
                {
                    if (_localStorage.Count == 0) return; //https://github.com/hanssens/localstorage/issues/17
                    _localStorage.Load();
                }
                catch
                {
                    Thread.Sleep(waitMilliseconds);
                    if (i == maxAttempts)
                    {
                        _logger.LogCritical("LocalStorage Load failed to save the database after {milliseconds}ms", maxAttempts * waitMilliseconds);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Create a todo in the database.
        /// </summary>
        /// <param name="todo"></param>
        /// <returns></returns>
        public ToDoDTO Create(ToDoDTO toDoDTO)
        {
            while (toDoDTO.Id is null || _localStorage.Exists(toDoDTO.Id))
                toDoDTO.Id = GetNewId();
            _localStorage.Store(key: toDoDTO.Id, instance: Convert(toDoDTO));
            PersistRetry();
            _localStorage.Load();
            return (ToDoDTO)toDoDTO.Clone();
        }

        /// <summary>
        /// Return all todos from database.
        /// </summary>
        /// <returns></returns>
        public List<ToDoDTO> Get(bool excludeDone = false)
        {
            List<ToDoDTO> todos = new();
            //_localStorage.Load();
            List<string> keys = _localStorage.Keys().ToList();
            int removedKeysCount = keys.RemoveAll(key => key is null);
            if (removedKeysCount > 0)
            {
                _logger.LogError("LocalStorage has {removedKeysCount} empty keys, they will be skipped.", removedKeysCount);
                _logger.LogError("Reload the local database to sanitize the sanitized, after quick actions.");
                _localStorage.Load();
            }
            foreach (string key in keys)
            {
                //Bug: Når man trykker sortér mange gange, så fejler den i update...get "key is null"... ..debug det!!
                var item = _localStorage.Get(key);
                var todo = Convert(_localStorage.Get<ToDo>(key));
                if (excludeDone && todo.Done)
                    continue;
                todos.Add(todo);
            }
            todos.Sort(new ToDoComparer());

            return todos;
        }

        /// <summary>
        /// Return a todo with a specific id from database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ToDoDTO Get(string id)
        {
            _localStorage.Load();
            return Convert(_localStorage.Get<ToDo>(key: id));
        }

        /// <summary>
        /// Updates (or creates if not found) a ToDo item in the database.
        /// </summary>
        /// <param name="toDoDTO"></param>
        /// <returns></returns>
        public ToDoDTO Update(ToDoDTO toDoDTO)
        {
            LoadRetry();
            var exists = _localStorage.Exists(toDoDTO.Id);
            if (exists)
                while (_localStorage.Exists(toDoDTO.Id))
                    _localStorage.Remove(toDoDTO.Id);
            try
            {
                _localStorage.Store(toDoDTO.Id, Convert(toDoDTO));
            }
            catch
            {
                _logger.LogDebug("Key {key} {exists} and failed when removing+adding via Store", toDoDTO.Id, (exists ? "exists" : "does not exist"));
                //System.ArgumentException: 'An item with the same key has already been added. Key: 638014311819156398'
            }
            PersistRetry();
            //_localStorage.Load();
            LoadRetry();
            return toDoDTO;
        }

        /// <summary>
        /// Deletes a todo item with a specific id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void Delete(string id)
        {
            _localStorage.Remove(key: id);
            PersistRetry();
            _localStorage.Load();
        }

        /// <summary>
        /// Deletes the database with all the todo items.
        /// </summary>
        /// <returns></returns>
        public void DeleteDatabaseAsync() => _localStorage.Destroy();
    }
}
