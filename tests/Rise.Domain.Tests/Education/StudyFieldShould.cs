using Rise.Domain.Education;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.Education
{
    public class StudyFieldShould
    {
        [Fact]
        public void Can_Create_StudyField_With_Students_And_Courses()
        {
            var dept = new Departments.Department { Name = "CS", Description = "CS Dept" };
            var students = new List<Student>();
            var courses = new List<Course>();

            var studyField = new StudyField
            {
                Name = "Computer Science",
                Departement = dept,
                Students = students,
                Courses = courses
            };

            studyField.Name.ShouldBe("Computer Science");
            studyField.Departement.ShouldBe(dept);
            studyField.Students.ShouldBe(students);
            studyField.Courses.ShouldBe(courses);
        }
    }
}
