﻿using Authorization.API;
using CourseManagement.Application.DTOs.Classes;
using CourseManagement.Application.Interfaces;
using CourseManagement.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController(IClassService classService) : ControllerBase
    {
        private readonly IClassService _classService = classService;

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var classes = await _classService.GetAllClassesAsync();
            return Ok(classes);
        }

        [HttpGet("{classId}")]
        public async Task<IActionResult> GetById(Guid classId)
        {
            var @class = await _classService.GetClassByIdAsync(classId);
            if (@class == null) return NotFound();
            return Ok(@class);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpPost]
        public async Task<IActionResult> Create(CreateClassDTO dto)
        {
            var @class = await _classService.CreateClassAsync(dto);
            return StatusCode(StatusCodes.Status201Created, @class);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpPut("{classId}")]
        public async Task<IActionResult> Update(Guid classId, UpdateClassDTO dto)
        {
            _ = await _classService.UpdateClassAsync(dto);
            return Ok();
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpDelete("{classId}")]
        public async Task<IActionResult> Delete(Guid classId)
        {
            await _classService.DeleteClassAsync(classId);
            return Ok();
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{classId}/courses")]
        public async Task<IActionResult> GetCoursesByClassId(Guid classId)
        {
            var courses = await _classService.GetCoursesAsync(classId);
            if (courses == null) return NotFound();
            return Ok(courses);
        }

        [AuthorizeRolesAndScopes(roles: [RoleConstants.Staff], scopes: ["course.manage"])]
        [HttpGet("{classId}/students")]
        public async Task<IActionResult> GetStudentsByClassId(Guid classId)
        {
            var students = await _classService.GetStudentsAsync(classId);
            if (students == null) return NotFound();
            return Ok(students);
        }
    }
}
