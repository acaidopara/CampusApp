namespace Rise.Domain.Events;

public class EventTimeSlot : ValueObject
{
    public DateOnly Date { get; } = default!;
    public TimeOnly StartTime { get; } = default!;
    public TimeOnly? EndTime { get; }

    private EventTimeSlot()
    {
        
    }

    public EventTimeSlot(DateOnly date, TimeOnly startTime, TimeOnly? endTime = null)
    {
        if (date <= DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ArgumentOutOfRangeException(nameof(date), "Date must be in the future");
        }

        Guard.Against.Default(startTime, nameof(startTime));

        if (endTime != null && endTime.Value < startTime)
            throw new ArgumentException("EndTime must be greater than or equal to StartTime", nameof(endTime));
    
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
    }



    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Date;
        yield return StartTime;
        if (EndTime != null) yield return EndTime;
    }
}