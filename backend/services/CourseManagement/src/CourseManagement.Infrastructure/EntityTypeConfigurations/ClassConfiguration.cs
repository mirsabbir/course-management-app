using CourseManagement.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Infrastructure.EntityTypeConfigurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
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
                .WithOne(cc => cc.Class)
                .HasForeignKey(cc => cc.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.ClassStudents)
                .WithOne(cs => cs.Class)
                .HasForeignKey(cs => cs.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
