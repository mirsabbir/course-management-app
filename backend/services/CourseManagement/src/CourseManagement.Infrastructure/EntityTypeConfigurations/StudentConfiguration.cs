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
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.DateOfBirth)
                .IsRequired();

            builder.Property(s => s.UserId)
                .IsRequired();

            builder.HasMany(s => s.CourseStudents)
                .WithOne(cs => cs.Student)
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.ClassStudents)
                .WithOne(cs => cs.Student)
                .HasForeignKey(cs => cs.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
