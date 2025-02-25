using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Domain
{
    public class Class
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string CreatedByName { get; set; }

        // Navigation properties
        public virtual ICollection<ClassCourse> ClassCourses { get; set; } = new List<ClassCourse>();
        public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
    }
}
