using Microsoft.AspNetCore.Mvc;
using ToDoWebApi.Models;
using ToDoWebApi.Services;

namespace ToDoWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ILogger<ToDoController> _logger; //ToDo: implement logging to somewhere... and then consider logging :D
        private readonly IDatabaseService _databaseService;

        public ToDoController(ILogger<ToDoController> logger, IDatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        /// <summary>
        /// Creates the todo item in the database and returns it with the created id.
        /// <para>Note: You can use the id from the response to add it to the front-end list.</para>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public ToDoDTO CreateItem([FromBody] ToDoDTO item) => _databaseService.CreateAsync(item).Result;

        [HttpGet]
        public IEnumerable<ToDoDTO> GetAll() => _databaseService.GetAsync().Result;

        [HttpPut]
        public void UpdateItem([FromBody] ToDoDTO item) => _databaseService.UpdateAsync(item);

        [HttpDelete("{id}")]
        public void DeleteItem(string id) => _databaseService.DeleteAsync(id);
    }
}