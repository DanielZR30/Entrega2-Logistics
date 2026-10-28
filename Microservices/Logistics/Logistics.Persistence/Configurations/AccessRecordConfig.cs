using Logistics.Domain.Entities.AccessRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Persistence.Configurations
{
    internal class AccessRecordConfig : IEntityTypeConfiguration<AccessRecord>
    {
        public void Configure(EntityTypeBuilder<AccessRecord> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.MovementType)
                   .IsRequired();

            builder.Property(r => r.Result)
                   .IsRequired();

            builder.Property(r => r.OccurredAt)
                   .IsRequired();

            builder.HasIndex(r => r.OccurredAt);

            builder.HasOne(r => r.Credential)
                   .WithMany(c => c.AccessRecords)
                   .HasForeignKey(r => r.CredentialId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.AccessPoint)
                   .WithMany(p => p.AccessRecords)
                   .HasForeignKey(r => r.AccessPointId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
