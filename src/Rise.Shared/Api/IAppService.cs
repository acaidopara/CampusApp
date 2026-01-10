namespace Rise.Shared.Api;

public interface IAppService
{
    Task<Result<T?>> GetAsync<T>(string url, CancellationToken cancellationToken = default, bool saveToCache = true);

    Task<Result<T?>> PostAsync<T, B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false);
    Task<Result> PostAsync<B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false);
    Task<Result> PostAsync(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false);


    Task<Result<T?>> DelAsync<T>(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false);
    Task<Result> DelAsync(string url, CancellationToken cancellationToken = default, bool ensureIsSend = false);

    Task<Result<T?>> PutAsync<T, B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false);
    Task<Result> PutAsync<B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false);

    Task<Result<T?>> PatchAsync<T, B>(string url, B request, CancellationToken cancellationToken = default, bool ensureIsSend = false);

    Task ShutDown();
}