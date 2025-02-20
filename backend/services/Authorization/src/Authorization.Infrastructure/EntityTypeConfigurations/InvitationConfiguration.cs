using Authorization.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authorization.Infrastructure.EntityTypeConfigurations
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> builder)
        {
            // Set the table name
            builder.ToTable("Invitations");

            // Configure the primary key
            builder.HasKey(i => i.Id);

            // Ensure Id is required and has a fixed length
            builder.Property(i => i.Id)
                   .IsRequired()
                   .HasMaxLength(100);

            // Configure Email: Required and Unique
            builder.Property(i => i.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasIndex(i => i.Email)
                   .IsUnique();

            // Configure Token: Required and Indexed
            builder.Property(i => i.Token)
                   .IsRequired()
                   .HasMaxLength(256);

            builder.HasIndex(i => i.Token)
                   .IsUnique();

            // Expiration Date: Required
            builder.Property(i => i.ExpirationDate)
                   .IsRequired();
        }
    }
}
