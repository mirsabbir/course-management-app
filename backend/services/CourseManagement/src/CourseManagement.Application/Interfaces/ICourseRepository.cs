using CourseManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<IEnumerable<Course>> GetByClassIdAsync(Guid classId);
        Task<Course> GetByIdAsync(Guid courseId);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(Guid courseId);
        Task<bool> ExistsByNameAsync(string name);
    }
}
