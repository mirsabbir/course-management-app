﻿using Authorization.API;
using CourseManagement.Application.DTOs;
using CourseManagement.Application.DTOs.Courses;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain.Constants;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{courseId}")]
        public async Task<IActionResult> GetById(Guid courseId)
        {
            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseDTO dto)
        {
            var course = await _courseService.CreateCourseAsync(dto);
            return StatusCode(StatusCodes.Status201Created, course);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpPut("{courseId}")]
        public async Task<IActionResult> Update(Guid courseId, UpdateCourseDTO dto)
        {
            var updated = await _courseService.UpdateCourseAsync(dto);
            return Ok();
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> Delete(Guid courseId)
        {
            await _courseService.DeleteCourseAsync(courseId);
            return Ok();
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{courseId}/classes")]
        public async Task<IActionResult> GetClassesByCourseId(Guid courseId)
        {
            var classes = await _courseService.GetClassesAsync(courseId);
            if (classes == null) return NotFound();
            return Ok(classes);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{courseId}/students")]
        public async Task<IActionResult> GetStudentsByCourseId(Guid courseId)
        {
            var students = await _courseService.GetStudentsAsync(courseId);
            if (students == null) return NotFound();
            return Ok(students);
        }
    }
}
