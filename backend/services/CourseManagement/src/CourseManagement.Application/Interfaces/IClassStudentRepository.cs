using CourseManagement.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.Interfaces
{
    public interface IClassStudentRepository
    {
        Task<bool> ExistsAsync(Guid classId, Guid studentId);
        Task AddAsync(ClassStudent classStudent);
        Task RemoveAsync(Guid classId, Guid studentId);
        Task<IEnumerable<ClassStudent>> GetStudentsByClassIdAsync(Guid classId);
        Task<IEnumerable<ClassStudent>> GetClasssesByStudentIdAsync(Guid studentId);
        Task<IEnumerable<Student>> GetStudentsDirectlyOrIndirectlyAssignedToClassAsync(Guid classId, Guid studentId);
    }
}
