using Rise.Domain.Users;

namespace Rise.Domain.Tests.Users
{
    public class EmployeeShould
    {
        private Departments.Department CreateDepartment() =>
            new Departments.Department { Name = "IT", Description = "IT Dept" };

        private Employee CreateEmployee()
        {
            var dept = CreateDepartment();
            return new Employee
            {
                Firstname = "John",
                Lastname = "Doe",
                AccountId = Guid.NewGuid().ToString(),
                Department = dept,
                Email = new EmailAddress("john@example.com"),
                Birthdate = new DateTime(1980,1,1),
                Employeenumber = "EMP001",
                Title = "Developer"
            };
        }

        [Fact]
        public void Can_Create_Employee_With_All_Required_Properties()
        {
            var emp = CreateEmployee();

            emp.Firstname.ShouldBe("John");
            emp.Lastname.ShouldBe("Doe");
            emp.Department.Name.ShouldBe("IT");
            emp.Email.Value.ShouldBe("john@example.com");
            emp.Employeenumber.ShouldBe("EMP001");
            emp.Title.ShouldBe("Developer");
        }

        [Fact]
        public void Setting_Employeenumber_Or_Title_To_NullOrWhitespace_Should_Throw()
        {
            var emp = CreateEmployee();

            Should.Throw<ArgumentException>(() => emp.Employeenumber = null!);
            Should.Throw<ArgumentException>(() => emp.Employeenumber = "");
            Should.Throw<ArgumentException>(() => emp.Title = null!);
            Should.Throw<ArgumentException>(() => emp.Title = "   ");
        }
    }
}
