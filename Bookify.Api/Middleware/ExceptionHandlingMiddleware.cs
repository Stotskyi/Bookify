using Bookify.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Middleware;

public class ExceptionHandlingMiddleware    
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch(Exception exception)
        {
            _logger.LogError(exception,"Exception occured: {exception}", exception.Message);

            var excepptionDetails = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Title = excepptionDetails.Title,
                Status = excepptionDetails.Status,
                Type = excepptionDetails.Type,
                Detail = excepptionDetails.Detail
            };

            if (excepptionDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] = excepptionDetails.Errors;
            }
            
            context.Response.StatusCode = excepptionDetails.Status;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                "ValidationFailure",
                "Validation error",
                "One or more validation errors occurred",
                validationException.Errors),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                "ServerError",
                "Server error",
                "An unexpected error has occured",
                null)
        };
    }
}

internal record ExceptionDetails(int Status, string Type, string Title, string Detail, IEnumerable<object>? Errors);