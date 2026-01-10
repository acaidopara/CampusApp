public interface IQueuedRequest
{
    string Url { get; }
    HttpMethod Method { get; }
    object Body { get; }
}
