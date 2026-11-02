using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TowerCo.Domain.Exceptions;
using TowerCo_Mgmt.Server.ExceptionHandling;

namespace TowerCo.Tests.Server;

public class DomainExceptionHandlerTests
{
    private sealed class TestRuleException() : BusinessRuleException("rule broken");
    private sealed class TestOtherDomainException() : DomainException("other");

    public static TheoryData<DomainException, int> StatusCases => new()
    {
        { new NotFoundException("Site", 42), StatusCodes.Status404NotFound },
        { new TestRuleException(), StatusCodes.Status409Conflict },
        { new TestOtherDomainException(), StatusCodes.Status400BadRequest }
    };

    [Theory]
    [MemberData(nameof(StatusCases))]
    public async Task TryHandleAsync_DomainException_WritesMappedStatus(DomainException exception, int expectedStatus)
    {
        var (handler, context) = CreateHandler();

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(expectedStatus, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_NonDomainException_ReturnsFalse()
    {
        var (handler, context) = CreateHandler();

        var handled = await handler.TryHandleAsync(context, new InvalidOperationException(), CancellationToken.None);

        Assert.False(handled);
    }

    [Fact]
    public void NotFoundException_Message_IncludesEntityAndKey()
    {
        Assert.Equal("Site '42' was not found.", new NotFoundException("Site", 42).Message);
    }

    private static (DomainExceptionHandler Handler, HttpContext Context) CreateHandler()
    {
        var provider = new ServiceCollection().AddLogging().AddProblemDetails().BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = provider, Response = { Body = new MemoryStream() } };
        var handler = new DomainExceptionHandler(provider.GetRequiredService<IProblemDetailsService>());
        return (handler, context);
    }
}
