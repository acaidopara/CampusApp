using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.CampusLife;

namespace Rise.Persistence.Configurations.CampusLife
{
    internal class JobConfiguration : EntityConfiguration<Job>
    {
        public override void Configure(EntityTypeBuilder<Job> builder)
        {
            base.Configure(builder);

            builder.ToTable("Jobs");

            builder.HasKey(j => j.Id);
            
            builder.OwnsOne(j => j.Address, ab =>
            {
                ab.Property(a => a.Addressline1).HasColumnName("Street").IsRequired().HasMaxLength(200);
                ab.Property(a => a.Addressline2).HasColumnName("Number").HasMaxLength(50);
                ab.Property(a => a.PostalCode).HasColumnName("ZipCode").IsRequired().HasMaxLength(10);
                ab.Property(a => a.City).HasColumnName("City").IsRequired().HasMaxLength(100);
            });
            
            builder.OwnsOne(j => j.EmailAddress, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(256);
            });

        }
    }
}