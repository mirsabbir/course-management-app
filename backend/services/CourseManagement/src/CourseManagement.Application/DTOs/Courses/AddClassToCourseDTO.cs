using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Application.DTOs.Courses
{
    public class AddClassToCourseDTO
    {
        public Guid CourseId { get; set; }
        public Guid ClassId { get; set; }
    }
}
