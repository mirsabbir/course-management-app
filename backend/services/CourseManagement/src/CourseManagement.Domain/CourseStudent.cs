using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Domain
{
    public class CourseStudent
    {
        public Guid CourseId { get; set; }
        public virtual Course Course { get; set; }

        public Guid StudentId { get; set; }
        public virtual Student Student { get; set; }

        public Guid AssignedBy { get; set; }
    }
}
