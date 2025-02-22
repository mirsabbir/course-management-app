using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.DTOs.Enrollment
{
    public class CourseEnrollmentDTO
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
    }
}
