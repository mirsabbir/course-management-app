﻿using CourseManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface ICourseStudentRepository
    {
        Task<bool> ExistsAsync(Guid courseId, Guid studentId);
        Task AddAsync(CourseStudent courseStudent);
        Task RemoveAsync(Guid courseId, Guid studentId);
        Task<IEnumerable<CourseStudent>> GetStudentsByCourseIdAsync(Guid courseId);
        Task<IEnumerable<CourseStudent>> GetCoursesByStudentIdAsync(Guid studentId);
    }
}
