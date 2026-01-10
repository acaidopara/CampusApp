namespace Rise.Shared.Lessons;

public static partial class LessonResponse
{
    public class Index
    {
        public IEnumerable<LessonDto.Index> Lessons { get; set; } = [];
        public int TotalCount { get; set; }
    }

    public class NextLesson
    {
        public required LessonDto.Index Lesson { get; set; }
    }
}