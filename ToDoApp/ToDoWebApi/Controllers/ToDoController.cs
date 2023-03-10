using Microsoft.AspNetCore.Mvc;
using ToDoWebApi.DAL.Interfaces;
using ToDoWebApi.BLL.Models;

namespace ToDoWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ILogger<ToDoController> _logger;
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
        public ToDoDTO CreateItem([FromBody] ToDoDTO item)
        {
            _logger.LogDebug("CreateItem was called at {Now}", DateTimeOffset.Now);
            return _databaseService.Create(item);
        }

        [HttpGet]
        public List<ToDoDTO> GetAll(bool excludeDone = false) {
            _logger.LogDebug("GetAll was called at {Now}", DateTimeOffset.Now);
            return _databaseService.Get(excludeDone);
        }

        [HttpPut]
        public void UpdateItem([FromBody] ToDoDTO item) => _databaseService.Update(item);

        [HttpDelete("{id}")]
        public void DeleteItem(string id) => _databaseService.Delete(id);
    }
}