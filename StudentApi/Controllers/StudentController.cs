using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using StudentApi.Data;
using StudentApi.Models;

namespace StudentApi.Controllers
{
    [Route("api/Student")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDbContext _context;
        public StudentController(StudentDbContext context) => _context = context;

       /* [HttpGet]
        public async Task<IEnumerable<Student>> Get()
            => await _context.Students.ToListAsync();
*/
       

        [HttpGet("{studentid}")]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByStudentId(string studentid)
        {
            var student = _context.Students.Where(s => s.StudemtId == studentid).FirstOrDefault();
            return student == null ? NotFound() : Ok(student);
        }

        [HttpGet("{idstudent}/{year}/{course}")]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByStudentIdname(string idstudent, string year,string course)
        {
            var student = _context.Students.Where(x => x.StudemtId == idstudent && x.year == year && x.Namecourse == course).FirstOrDefault();
            return student == null ? NotFound() : Ok(student);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Student student)
        {
            var temp = _context.Students.Where(x => x.StudemtId == student.StudemtId && 
            x.Namecourse == student.Namecourse && x.year == student.year).FirstOrDefault();
            if (temp != null || int.TryParse(student.StudemtId, out _) == false)
            {
                return Problem();
            }
            _context.Students.Add(student);
            _context.SaveChanges();
            return Ok(CreatedAtAction(nameof(GetById), new { id = student.Id }, student).Value);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Student student)
        {
            if (id != student.Id) return BadRequest();
            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var studentToDelete = await _context.Students.FindAsync(id);
            if (studentToDelete == null) return NotFound();
            _context.Students.Remove(studentToDelete);
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("course/{name}")]
        public async Task<IActionResult> DeleteCourse(string name)
        {
            var studentToDelete = _context.Students.Where(s => s.Namecourse == name).ToList();
            if (studentToDelete == null) return NotFound();
            foreach(var student in studentToDelete)
            {
                _context.Students.Remove(student);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("year/{name}")]
        public async Task<IActionResult> DeleteYear(string name)
        {
            var studentToDelete = _context.Students.Where(s => s.year == name).ToList();
            if (studentToDelete == null) return NotFound();
            foreach (var student in studentToDelete)
            {
                _context.Students.Remove(student);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }




        ///////////////privete///////
        

        /* [HttpGet("{id}")]
        [ProducesResponseType(typeof(Student), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]*/
        private async Task<IActionResult> GetById(int id)
        {
            var student = await _context.Students.FindAsync(id);
            return student == null ? NotFound() : Ok(student);
        }
    }
}
