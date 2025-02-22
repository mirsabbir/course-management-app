using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.DTOs.Students
{
    public class UpdateStudentDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
