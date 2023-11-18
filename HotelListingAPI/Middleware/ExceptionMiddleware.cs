using System.Net;
using HotelListingAPI.Exceptions;
using Newtonsoft.Json;

namespace HotelListingAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    // _next: Represents the next operations its about to occur
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    // HttpContext --> information about your request, potential response etc
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Something went wrong processing: {context.Request.Path}");
            await HandleExeceptionAsync(context, exception);
        }
    }

    private  Task HandleExeceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

        var errorDetails = new ErrorDetails()
        {
            ErrorType = "Failure",
            ErrorMessage = exception.Message
        };

        switch (exception)
        {
            case NotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorDetails.ErrorType = "Not found";
                break;
        }

        string response = JsonConvert.SerializeObject(errorDetails);
        // convert enum value to int
        context.Response.StatusCode = (int)statusCode;
        var contextResponse = context.Response.WriteAsync(response);

        return contextResponse;
    }
}

public class ErrorDetails
{
    public string ErrorType { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
}