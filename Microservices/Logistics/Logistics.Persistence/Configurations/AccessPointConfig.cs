using Logistics.Domain.Entities.AccessPoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logistics.Persistence.Configurations
{
    internal class AccessPointConfig : IEntityTypeConfiguration<AccessPoint>
    {
        public void Configure(EntityTypeBuilder<AccessPoint> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                   .HasMaxLength(64)
                   .IsRequired();

            builder.Property(p => p.Description)
                   .HasMaxLength(512);

            builder.Property(p => p.Type)
                   .IsRequired();

            builder.Property(p => p.IsEnabled)
                   .IsRequired();

            builder.Property(p => p.CreatedAt)
                   .IsRequired();

            builder.HasMany(p => p.AccessRecords)
                   .WithOne(r => r.AccessPoint)
                   .HasForeignKey(r => r.AccessPointId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
