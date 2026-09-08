using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace OTSISampleTemplate.Web.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception occurred: {Message}", context.Exception.Message);

        var result = new ObjectResult(new
        {
            Message = "An unexpected error occurred. Please contact support.",
            Detail = context.Exception.Message
        })
        {
            StatusCode = 500
        };

        context.Result = result;
        context.ExceptionHandled = true;
    }
}
