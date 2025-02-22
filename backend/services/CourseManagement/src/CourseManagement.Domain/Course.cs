using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Domain
{
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<ClassCourse> ClassCourses { get; set; } = new List<ClassCourse>();
        public virtual ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
    }
}
