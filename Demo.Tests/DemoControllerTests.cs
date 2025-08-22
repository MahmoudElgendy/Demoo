using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Demo.Api.Controllers;
using Demo.Api.Data;
using Demo.Api.Models;
using Shouldly;

namespace Demo.Tests
{
    public class DemoControllerTests
    {
        private ApplicationDbContext GetInMemoryDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetStudents_ReturnsAllItems()
        {
            // Arrange
            var db = GetInMemoryDbContext("GetStudentsDb");
            db.Persons.AddRange(
                new Person { Id = Guid.NewGuid(), Name = "One" },
                new Person { Id = Guid.NewGuid(), Name = "Two" }
            );
            await db.SaveChangesAsync();

            var controller = new PersonsController(db);

            // Act
            var result = await controller.GetStudents();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<Person>>>(result);
            var list = Assert.IsType<List<Person>>(actionResult.Value);
            list.Count().ShouldBe(2);
        }

        [Fact]
        public async Task PostStudent_AddsNewStudent()
        {
            // Arrange
            var db = GetInMemoryDbContext("PostStudentDb");
            var controller = new PersonsController(db);
            var newStudent = new Person { Name = "John" };

            // Act
            var result = await controller.PostStudent(newStudent);

            // Assert
            var okResult = result.Result as OkObjectResult;
            var person = okResult.Value as Person;
            person!.Name.ShouldBe("John");

        }
    }
}
