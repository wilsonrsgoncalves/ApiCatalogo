using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalogo.Filters
{
    public class ApiLoggingFilter(ILogger<ApiLoggingFilter> logger) : IActionFilter
    {
        private readonly ILogger<ApiLoggingFilter> _logger = logger;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //executa antes da Action
            _logger.LogInformation("### Executando -> OnActionExecuting");
            _logger.LogInformation("###################################################");
            _logger.LogInformation("Current Time: {Time}", DateTime.Now.ToLongTimeString());
            _logger.LogInformation("ModelState Validity: {IsValid}", context.ModelState.IsValid);
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //executa depois da Action
            _logger.LogInformation("### Executando -> OnActionExecuted");
            _logger.LogInformation("###################################################");
            _logger.LogInformation("Current Time: {Time}", DateTime.Now.ToLongTimeString());
            _logger.LogInformation("Response Status Code: {StatusCode}", context.HttpContext.Response.StatusCode);
            _logger.LogInformation("###################################################");
        }
    }
}