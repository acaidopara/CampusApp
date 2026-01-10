using System;

namespace Rise.Domain.Infrastructure
{
    public class Openingtime : Entity
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateOnly Date { get; set; }

        // public constructor is puur voor testen
        public Openingtime()
        {
            
        }
        
        public Openingtime(DateOnly date, TimeOnly startTime, TimeOnly endTime)
        {
            if (date < DateOnly.FromDateTime(DateTime.Today))
                throw new ArgumentOutOfRangeException(nameof(date), "Date cannot be in the past.");
            
            if (startTime >= endTime)
                throw new ArgumentException("StartTime must be earlier than EndTime.", nameof(startTime));

            Date = date;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
