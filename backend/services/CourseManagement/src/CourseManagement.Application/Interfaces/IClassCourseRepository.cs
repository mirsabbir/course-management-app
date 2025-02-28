using CourseManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface IClassCourseRepository
    {
        Task<bool> ExistsAsync(Guid courseId, Guid classId);
        Task AddAsync(ClassCourse classCourse);
        Task RemoveAsync(Guid courseId, Guid classId);
    }
}
