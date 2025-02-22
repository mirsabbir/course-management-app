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
    public class ClassCourseConfiguration : IEntityTypeConfiguration<ClassCourse>
    {
        public void Configure(EntityTypeBuilder<ClassCourse> builder)
        {
            builder.HasKey(cc => new { cc.ClassId, cc.CourseId });

            builder.HasOne(cc => cc.Class)
                .WithMany(c => c.ClassCourses)
                .HasForeignKey(cc => cc.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cc => cc.Course)
                .WithMany(c => c.ClassCourses)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(cc => cc.AssignedBy)
                .IsRequired();
        }
    }

}
