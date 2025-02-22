using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.DTOs.Enrollment
{
    public class ClassEnrollmentDTO
    {
        public Guid StudentId { get; set; }
        public Guid ClassId { get; set; }
    }
}
