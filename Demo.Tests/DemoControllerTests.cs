using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Demo.Api.Controllers;
using Demo.Api.Data;
using Demo.Api.Models;

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
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task PostStudent_AddsNewStudent()
        {
            // Arrange
            var db = GetInMemoryDbContext("PostStudentDb");
            var controller = new PersonsController(db);
            var newStudent = new Person { Name = "New Student" };

            // Act
            var result = await controller.PostStudent(newStudent);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Person>(createdAt.Value);
            Assert.Equal("New Student", returnValue.Name);
            Assert.Single(db.Persons);
        }
    }
}
