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
        public async Task<ActionResult<IEnumerable<Person>>> GetStudents()
        {
            return await _context.Persons.ToListAsync();
        }
        [HttpPost]
        public async Task<ActionResult<Person>> PostStudent(Person student)
        {
            _context.Persons.Add(student);
            await _context.SaveChangesAsync();
           var studentresponse = await _context.Persons.FindAsync(student.Id);
            return Ok(studentresponse);
        }
    }
}
