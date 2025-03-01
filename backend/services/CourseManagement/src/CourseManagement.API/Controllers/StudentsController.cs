using Authorization.API;
using CourseManagement.API.Models;
using CourseManagement.Application.DTOs;
using CourseManagement.Application.DTOs.Students;
using CourseManagement.Application.Interfaces;
using CourseManagement.Application.Services;
using CourseManagement.Domain.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (students, totalCount) = await _studentService.GetAllAsync(pageNumber, pageSize);

            var response = new PagedResponse<StudentDTO>
            {
                Data = students,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return Ok(response); // Return the paginated student data
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetById(Guid studentId)
        {
            var student = await _studentService.GetByIdAsync(studentId);
            if (student == null) return NotFound();
            return Ok(student);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentDTO dto)
        {
            var student = await _studentService.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created, student);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpPut("{studentId}")]
        public async Task<IActionResult> Update(Guid studentId, UpdateStudentDTO dto)
        {
            var updated = await _studentService.UpdateAsync(dto);
            return Ok();
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpDelete("{studentId}")]
        public async Task<IActionResult> Delete(Guid studentId)
        {
            await _studentService.DeleteAsync(studentId);
            return Ok();
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("search")]
        public async Task<IActionResult> SearchStudents([FromQuery] string query)
        {
            var students = await _studentService.SearchStudentsAsync(query);
            return Ok(students);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Student, RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{studentId}/courses")]
        public async Task<IActionResult> GetCourses(Guid studentId)
        {
            var courses = await _studentService.GetCoursesAsync(studentId);
            return Ok(courses);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Student, RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{studentId}/classes")]
        public async Task<IActionResult> GetClasses(Guid studentId)
        {
            var classes = await _studentService.GetClassesAsync(studentId);
            return Ok(classes);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Student], scopes: ["course.manage"])]
        [HttpGet("me")]
        public async Task<IActionResult> GetStudentInfo()
        {
            var student = await _studentService.GetStudentInfoAsync();
            if (student == null) return NotFound();
            return Ok(student);
        }
    }
}
