using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainNews = Rise.Domain.News;

namespace Rise.Persistence.Configurations.News
{
    internal class NewsConfiguration : EntityConfiguration<DomainNews.News>
    {
        public override void Configure(EntityTypeBuilder<DomainNews.News> builder)
        {
            base.Configure(builder);

            builder.ToTable("News");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.ImageUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(n => n.Subject)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(n => n.Content)
                .IsRequired()
                .HasColumnType("longtext");

            builder.Property(n => n.AuthorName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.AuthorFunction)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(n => n.AuthorAvatarUrl)
                .HasMaxLength(2048)
                .IsRequired(false);

            builder.Property(n => n.Date)
                .IsRequired()
                .HasColumnType("datetime");
        }
    }
}