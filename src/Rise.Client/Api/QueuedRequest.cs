public class QueuedRequest<B> : IQueuedRequest
{
    public required string Url { get; set; }
    public required HttpMethod Method { get; set; }
    public required B BodyJson { get; set; }
    object IQueuedRequest.Body => BodyJson!;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
