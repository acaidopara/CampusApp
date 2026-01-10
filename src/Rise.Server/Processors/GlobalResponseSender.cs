using IResult = Ardalis.Result.IResult;

namespace Rise.Server.Processors;

/// <summary>
/// Sends HTTP responses globally after processing by mapping <see cref="IResult"/> 
/// status codes to corresponding HTTP status codes, and writing them to the response.
/// </summary>
sealed class GlobalResponseSender : IGlobalPostProcessor
{
    public async Task PostProcessAsync(IPostProcessorContext ctx, CancellationToken ct)
    {
        if (!ctx.HttpContext.ResponseStarted())
        {
            if (ctx.Response is IResult result)
            {
                switch (result.Status)
                {
                    case ResultStatus.Ok:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status200OK, cancellation: ct);
                        break;
                    case ResultStatus.Created:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status201Created, cancellation: ct);
                        break;
                    case ResultStatus.Forbidden:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status403Forbidden, cancellation: ct);
                        break;
                    case ResultStatus.Unauthorized:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status401Unauthorized, cancellation: ct);
                        break;
                    case ResultStatus.Invalid:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status400BadRequest, cancellation: ct);
                        break;
                    case ResultStatus.NotFound:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status404NotFound, cancellation: ct);
                        break;
                    case ResultStatus.NoContent:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status204NoContent, cancellation: ct);
                        break;
                    case ResultStatus.Conflict:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status409Conflict, cancellation: ct);
                        break;
                    case ResultStatus.CriticalError:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status500InternalServerError, cancellation: ct);
                        break;
                    case ResultStatus.Error:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status422UnprocessableEntity, cancellation: ct);
                        break;
                    case ResultStatus.Unavailable:
                        await ctx.HttpContext.Response.SendAsync(result, StatusCodes.Status503ServiceUnavailable, cancellation: ct);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Result status is not supported.");
                }
            }
            else
            {
                await ctx.HttpContext.Response.SendAsync(ctx.Response, cancellation: ct);
            }
        }
    }
}