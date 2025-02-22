using CourseManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Infrastructure.EntityTypeConfigurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.CreatedById)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.HasMany(c => c.ClassCourses)
                .WithOne(cc => cc.Course)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.CourseStudents)
                .WithOne(cs => cs.Course)
                .HasForeignKey(cs => cs.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
