using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMSApi.Models;

namespace TMSApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentContext _dbContext;
        public StudentController(StudentContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            if (_dbContext.Students == null)
                return NotFound();

            return await _dbContext.Students.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            if (_dbContext.Students == null)
                return NotFound();

            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
                return NotFound();

            return student;
        }

        [HttpPost]

        public async Task<ActionResult<Student>> PostStudent(Student student)
        {
            _dbContext.Students.Add(student);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
        }

        [HttpPut]
        public async Task<IActionResult> PutStudent(int id, Student student)
        {
            if (id != student.Id)
                return BadRequest();

            _dbContext.Entry(student).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentAvailable(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();

        }
        private bool StudentAvailable(int id)
        {
            return (_dbContext.Students?.Any(x => x.Id == id)).GetValueOrDefault();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            if (_dbContext.Students == null)
                return NotFound();
            var student = await _dbContext.Students.FindAsync(id);
            if (student == null)
                return NotFound();

            _dbContext.Students.Remove(student);
            await _dbContext.SaveChangesAsync();
            return Ok();

        }
    }
}
