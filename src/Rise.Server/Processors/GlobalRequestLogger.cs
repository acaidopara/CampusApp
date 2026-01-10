namespace Rise.Server.Processors;

/// <summary>
/// Logs incoming HTTP requests globally before they are processed, 
/// including the request path and parameters.
/// </summary>
public class GlobalRequestLogger : IGlobalPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext context, CancellationToken ct)
    {
        Log.Information("Requesting '{RequestPath}' with parameters {@RequestParameters}", context.HttpContext.Request.Path, context.Request);

        return Task.CompletedTask;
    }
}