using Logistics.Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Persistence.Configurations
{
    internal class EmployeeConfig : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.IdentificationNumber)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.HasIndex(e => e.IdentificationNumber)
                   .IsUnique();

            builder.Property(e => e.Names)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.LastNames)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.OwnsOne(e => e.Email, email =>
            {
                email.Property(m => m.Value)
                     .HasColumnName("Email")
                     .HasMaxLength(256)
                     .IsRequired();
            });

            builder.Property(e => e.Position)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(e => e.Area)
                   .IsRequired();

            builder.Property(e => e.Status)
                   .IsRequired();

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.HasMany(e => e.Credentials)
                   .WithOne(c => c.Employee)
                   .HasForeignKey(c => c.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
