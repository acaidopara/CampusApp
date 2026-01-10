using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.CampusLife;

namespace Rise.Persistence.Configurations.CampusLife
{
    internal class StudentClubConfiguration : EntityConfiguration<StudentClub>
    {
        public override void Configure(EntityTypeBuilder<StudentClub> builder)
        {
            base.Configure(builder);

            builder.ToTable("Studentclubs");

            builder.HasKey(e => e.Id);
            
            builder.OwnsOne(e => e.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(256);
            });

        }
    }
}