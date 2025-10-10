using Demo.Api.Contracts.Requests;
using Demo.Api.Contracts.Responses;
using Demo.Api.Data;
using Demo.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public PersonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonResponse>>> GetStudents()
        {
            return await _context.Persons
                .AsNoTracking()
                .Select(std=> new PersonResponse
            {
                Id = std.Id,
                Name = std.Name,
                Address = std.Address,
                Qualifications = std.Qualifications.Select(q => new QualificationResponse
                {
                    Id = q.Id,
                    Title = q.Title,
                    Level = q.Level,
                }).ToList() ?? new List<QualificationResponse>()
            }).ToListAsync();
        }
        [HttpPost]
        public async Task<ActionResult<Person>> PostStudent(PersonRequest request)
        {
            var student = new Person
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Address = request.Address,
                Qualifications = request.Qualifications?.Select(q => new Qualification
                {
                    Id = q.Id,
                    Title = q.Title,
                    Level = q.Level,
                }).ToList() ?? new List<Qualification>()
            };

            _context.Persons.Add(student);
            await _context.SaveChangesAsync();
            var std = await _context.Persons.FindAsync(student.Id);
            var result = new PersonResponse
            {
                Id = std.Id,
                Name = std.Name,
                Address = std.Address,
                Qualifications = std.Qualifications?.Select(q => new QualificationResponse
                {
                    Id = q.Id,
                    Title = q.Title,
                    Level = q.Level,
                }).ToList() ?? new List<QualificationResponse>()
            };
            return Ok(result);
        }
    }
}
