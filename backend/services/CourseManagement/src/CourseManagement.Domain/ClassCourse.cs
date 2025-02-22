using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Domain
{
    public class ClassCourse
    {
        public Guid ClassId { get; set; }
        public virtual Class Class { get; set; }

        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public Guid AssignedBy { get; set; }
    }
}
