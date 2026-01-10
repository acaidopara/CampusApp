using Rise.Shared.Infrastructure;


namespace Rise.Shared.Lessons;

public static class LessonDto
{
    public class Index
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required DateTime Start { get; set; }
        public required DateTime End { get; set; }
        public required List<ClassroomDto.Index> ClassroomDtos { get; set; }
        public required string LessonType { get; set; }
        public required List<string> TeacherNames { get; set; }
        public required List<string> ClassgroupNames { get; set; }
    }
}
