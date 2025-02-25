using Authorization.API;
using CourseManagement.Application.DTOs;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var students = await _studentService.GetAllAsync();
            return Ok(students);
        }

        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetById(Guid studentId)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentDTO dto)
        {
            var student = await _studentService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created, student);
        }

        [HttpPut("{studentId}")]
        public async Task<IActionResult> Update(Guid studentId, UpdateStudentDTO dto)
        {
            var updated = await _studentService.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{studentId}")]
        public async Task<IActionResult> Delete(Guid studentId)
        {
            await _studentService.DeleteAsync(studentId);
            return Ok();
        }
    }
}
