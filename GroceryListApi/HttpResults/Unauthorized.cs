using System.Reflection;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http.Metadata;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Http.HttpResults;

/// <summary>
/// An <see cref="IResult"/> that on execution will write an object to the response
/// with Unauthorized (401) status code.
/// </summary>
// HACK: The UnauthorizedHttpResult class is sealed, so can't extend it here.
// I have not investigated if this alternative has any side effects, so please use this with caution.
// public sealed class Unauthorized : UnauthorizedHttpResult, IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
//
// Alternative:
[PublicAPI]
public sealed class Unauthorized : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Unauthorized"/> class with the values.
    /// </summary>
    internal Unauthorized()
    {
    }

    /// <summary>
    /// Gets the HTTP status code: <see cref="StatusCodes.Status401Unauthorized"/>
    /// </summary>
    public int StatusCode => StatusCodes.Status401Unauthorized;

    int? IStatusCodeHttpResult.StatusCode => StatusCode;

    /// <inheritdoc/>
    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        // Creating the logger with a string to preserve the category after the refactoring.
        var loggerFactory = httpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("Microsoft.AspNetCore.Http.Result.UnauthorizedHttpResult");

        // HACK: The HttpResultsHelper class is internal, so can't use it here.
        // HttpResultsHelper.Log.WritingResultAsStatusCode(logger, StatusCode);
        //
        // Alternative:
        // ReSharper disable once LogMessageIsSentenceProblem
        logger.LogInformation("Setting HTTP status code {StatusCode}.", StatusCode);

        httpContext.Response.StatusCode = StatusCode;

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        // HACK: The ProducesResponseTypeMetadata class is internal, so can't use it here.
        // builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status401Unauthorized));
        //
        // Alternative:
        builder.Metadata.Add(new UnauthorizedResponseTypeMetadata());
    }

    private class UnauthorizedResponseTypeMetadata : IProducesResponseTypeMetadata
    {
        public Type? Type => typeof(void);
        public int StatusCode => 401;
        public IEnumerable<string> ContentTypes => Enumerable.Empty<string>();
    }
}

// HACK: Extending IResultExtensions so that we can use this alternative Unauthorized result.
// Ideally, TypedResults.Unauthorized() would provide response type metadata, but alas.
public static class UnauthorizedResultExtensions
{
    private static readonly Unauthorized Unauthorized = new();

    public static Unauthorized UnauthorizedTypedResult(this IResultExtensions current) => Unauthorized;
}