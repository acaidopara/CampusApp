using Ardalis.Result;
using Rise.Domain.Departments;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.Department
{
    public class DepartmentTests
    {
        private Departments.Department CreateDepartment() =>
            new Departments.Department
            {
                Name = "IT",
                Description = "IT Department",
                DepartmentType = DepartmentType.Department
            };

        private Employee CreateEmployee(Departments.Department dept) =>
            new Employee
            {
                Firstname = "John",
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = dept,
                Email = new EmailAddress("john.doe@example.com"),
                Birthdate = new DateTime(1990, 1, 1),
                Employeenumber = "EMP001",
                Title = "Developer"
            };

        private Student CreateStudent(Departments.Department dept) =>
            new Student
            {
                Firstname = "Jane",
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = dept,
                Email = new EmailAddress("jane.doe@example.com"),
                Birthdate = new DateTime(2000, 1, 1),
                StudentNumber = "S12345"
            };

        [Fact]
        public void Can_Create_Department_With_Valid_Fields()
        {
            var department = CreateDepartment();

            department.Name.ShouldBe("IT");
            department.Description.ShouldBe("IT Department");
            department.DepartmentType.ShouldBe(DepartmentType.Department);
            department.Members.ShouldBeEmpty();
            department.Employees.ShouldBeEmpty();
            department.Students.ShouldBeEmpty();
        }

        [Fact]
        public void Can_Add_Employee_To_Department()
        {
            var dept = CreateDepartment();
            var employee = CreateEmployee(dept);

            var result = dept.AddEmployee(employee);

            result.Status.ShouldBe(ResultStatus.Ok);
        }

        [Fact]
        public void Adding_Same_Employee_Twice_Should_Return_Conflict()
        {
            var dept = CreateDepartment();
            var employee = CreateEmployee(dept);

            var membersField = typeof(Departments.Department).GetField("members", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var membersList = (List<User>)membersField!.GetValue(dept)!;
            membersList.Add(employee);

            var result = dept.AddEmployee(employee);

            result.Status.ShouldBe(ResultStatus.Conflict);
            result.Errors.First().ShouldBe("Employee already exists in this department");
        }

        [Fact]
        public void Can_Retrieve_Employees_And_Students()
        {
            var dept = CreateDepartment();
            var employee = CreateEmployee(dept);
            var student = CreateStudent(dept);

            var membersField = typeof(Departments.Department).GetField("members", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var membersList = (List<User>)membersField!.GetValue(dept)!;
            membersList.Add(employee);
            membersList.Add(student);

            var employees = dept.Employees;
            var students = dept.Students;

            employees.Count.ShouldBe(1);
            employees[0].ShouldBe(employee);

            students.Count.ShouldBe(1);
            students[0].ShouldBe(student);
        }
        
        [Fact]
        public void Cannot_Create_Department_With_Empty_Name()
        {
            var exception = Should.Throw<ArgumentException>(() =>
            {
                new Departments.Department
                {
                    Name = "",
                    Description = "IT Department",
                    DepartmentType = DepartmentType.Department
                };
            });

            exception.Message.ShouldBe("Required input value was empty. (Parameter 'value')");
        }
        
    }
}
