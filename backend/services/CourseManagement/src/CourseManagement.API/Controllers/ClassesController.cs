using Authorization.API;
using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassesController(IClassService classService)
        {
            _classService = classService;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _classService.GetAllClassesAsync();
            return Ok(courses);
        }

        [HttpGet("{classId}")]
        public async Task<IActionResult> GetById(Guid classId)
        {
            var course = await _classService.GetClassByIdAsync(classId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [AuthorizeRolesAndScopes(roles: [], scopes: ["course.manage"])]
        [HttpPost]
        public async Task<IActionResult> Create(CreateClassDTO dto)
        {
            var @class = await _classService.CreateClassAsync(dto);
            return CreatedAtAction(nameof(GetById), new { classId = @class.Id }, @class);
        }

        [HttpPut("{classId}")]
        public async Task<IActionResult> Update(Guid classId, UpdateClassDTO dto)
        {
            var updated = await _classService.UpdateClassAsync(dto);
            return Ok();
        }

        [HttpDelete("{classId}")]
        public async Task<IActionResult> Delete(Guid classId)
        {
            await _classService.DeleteClassAsync(classId);
            return Ok();
        }

        [HttpGet("{classId}/courses")]
        public async Task<IActionResult> GetCoursesByClassId(Guid classId)
        {
            var course = await _classService.GetCoursesAsync(classId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpGet("{classId}/students")]
        public async Task<IActionResult> GetStudentsByClassId(Guid classId)
        {
            var course = await _classService.GetStudentsAsync(classId);
            if (course == null) return NotFound();
            return Ok(course);
        }
    }
}
