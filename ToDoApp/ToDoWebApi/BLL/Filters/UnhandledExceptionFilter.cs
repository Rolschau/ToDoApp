using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDoWebApi.DAL.Services;

namespace ToDoWebApi.BLL.Filters
{
    public class UnhandledExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;

        public UnhandledExceptionFilter(ILogger<UnhandledExceptionFilter> logger) => _logger = logger;
        public void OnException(ExceptionContext context)
        {
            _logger?.LogCritical("Unhandled exception: {Message} at {Now}", context.Exception, DateTimeOffset.Now);
            context.Result = new ContentResult
            {
                Content = context.Exception.ToString()
            };
        }
    }
}
