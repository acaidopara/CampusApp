namespace Rise.Server.Processors;

/// <summary>
/// Logs outgoing HTTP responses globally after they are processed, 
/// including the request path and response result.
/// </summary>
public class GlobalResponseLogger : IGlobalPostProcessor
{
    public Task PostProcessAsync(IPostProcessorContext context, CancellationToken ct)
    {
        Log.Information("Requested '{RequestPath}' with result {@RequestResult}", context.HttpContext.Request.Path, context.Response);

        return Task.CompletedTask;
    }
}