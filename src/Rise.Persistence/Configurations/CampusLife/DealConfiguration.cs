using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.CampusLife;

namespace Rise.Persistence.Configurations.CampusLife
{
    internal class DealConfiguration : EntityConfiguration<StudentDeal>
    {
        public override void Configure(EntityTypeBuilder<StudentDeal> builder)
        {
            base.Configure(builder);

            builder.ToTable("StudentDeals");

            builder.HasKey(d => d.Id);

        }
    }
}