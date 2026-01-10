using Rise.Domain.Departments;
using Rise.Domain.Users;

namespace Rise.Domain.Education;

public class StudyField : Entity
{
    public required string Name { get; set; }
    public List<Student>? Students { get; set; }
    public List<Course>? Courses { get; set; }
    public Department? Departement { get; set; }
}