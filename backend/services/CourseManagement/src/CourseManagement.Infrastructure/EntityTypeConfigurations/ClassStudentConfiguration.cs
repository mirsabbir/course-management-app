using CourseManagement.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Infrastructure.EntityTypeConfigurations
{
    public class ClassStudentConfiguration : IEntityTypeConfiguration<ClassStudent>
    {
        public void Configure(EntityTypeBuilder<ClassStudent> builder)
        {
            builder.HasKey(cs => new { cs.ClassId, cs.StudentId });

            builder.HasOne(cs => cs.Class)
                .WithMany(c => c.ClassStudents)
                .HasForeignKey(cs => cs.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cs => cs.Student)
                .WithMany(s => s.ClassStudents)
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(cs => cs.AssignedById)
                .IsRequired();
        }
    }

}
