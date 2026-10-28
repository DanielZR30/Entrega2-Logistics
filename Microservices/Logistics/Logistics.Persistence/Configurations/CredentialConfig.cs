using Logistics.Domain.Entities.Credentials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Persistence.Configurations
{
    internal class CredentialConfig : IEntityTypeConfiguration<Credential>
    {
        public void Configure(EntityTypeBuilder<Credential> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Code)
                   .HasMaxLength(32)
                   .IsRequired();

            builder.HasIndex(c => c.Code)
                   .IsUnique();

            builder.Property(c => c.Status)
                   .IsRequired();

            builder.Property(c => c.AssignedAt)
                   .IsRequired();

            builder.HasOne(c => c.Employee)
                   .WithMany(e => e.Credentials)
                   .HasForeignKey(c => c.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
