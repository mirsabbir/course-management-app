using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Domain
{
    public class Student
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Guid UserId { get; set; }

        // Navigation properties
        public virtual ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
        public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();
    }
}
