using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TowerCo.Domain.Exceptions;

namespace TowerCo_Mgmt.Server.ExceptionHandling;

/// <summary>Maps domain exceptions to RFC 7807 ProblemDetails responses. Other exceptions fall through.</summary>
public sealed class DomainExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        httpContext.Response.StatusCode = ToStatusCode(domainException);
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = httpContext.Response.StatusCode, Detail = exception.Message }
        });
    }

    public static int ToStatusCode(DomainException exception) => exception switch
    {
        NotFoundException => StatusCodes.Status404NotFound,
        BusinessRuleException => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest
    };
}
