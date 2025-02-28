using CourseManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllAsync();
        Task<IEnumerable<Class>> GetPagedAsync(int pageNumber, int pageSize);
        Task<int> CountAsync();
        Task<IEnumerable<Class>> GetByCourseIdAsync(Guid courseId);
        Task<Class> GetByIdAsync(Guid classId);
        Task AddAsync(Class @class);
        Task UpdateAsync(Class @class);
        Task DeleteAsync(Guid classId);
        Task<bool> ExistsByNameAsync(string name);
        Task<IEnumerable<Class>> SearchAsync(string query);
    }
}
