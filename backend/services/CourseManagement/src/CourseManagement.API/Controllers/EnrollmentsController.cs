using CourseManagement.Application.DTOs.Enrollment;
using CourseManagement.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly ICourseService _courseService;

        public EnrollmentsController(
            IClassService classService,
            ICourseService courseService)
        {
            _classService = classService;
            _courseService = courseService;
        }

        [HttpPost("classes")]
        public async Task<IActionResult> EnrollToClass(ClassEnrollmentDTO classEnrollmentDTO)
        {
            await _classService.EnrollStudentAsync(classEnrollmentDTO);
            return Ok();
        }

        [HttpPost("courses")]
        public async Task<IActionResult> EnrollToCourse(CourseEnrollmentDTO courseEnrollmentDTO)
        {
             await _courseService.EnrollStudentAsync(courseEnrollmentDTO);
            return Ok();
        }
    }
}
