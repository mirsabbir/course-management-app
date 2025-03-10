﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Domain
{
    public class ClassStudent
    {
        public Guid ClassId { get; set; }
        public Class Class { get; set; }

        public Guid StudentId { get; set; }
        public Student Student { get; set; }

        public DateTime AssignedAt { get; set; }
        public Guid AssignedById { get; set; }
        public required string AssignedByName { get; set; }
    }
}
