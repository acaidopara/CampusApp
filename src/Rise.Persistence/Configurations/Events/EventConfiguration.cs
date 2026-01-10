using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using DomainEvents = Rise.Domain.Events;

namespace Rise.Persistence.Configurations.Events
{
    internal class NewsConfiguration : EntityConfiguration<DomainEvents.Event>
    {
        public override void Configure(EntityTypeBuilder<DomainEvents.Event> builder)
        {
            base.Configure(builder);

            builder.ToTable("Events");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            /*builder.Property(e => e.Date)
                .IsRequired()
                .HasColumnType("datetime");*/
            builder.OwnsOne(e => e.Date, ts =>
            {
                ts.Property(p => p.Date)
                    .HasColumnName("EventDate")
                    .HasColumnType("date");

                ts.Property(p => p.StartTime)
                    .HasColumnName("StartTime")
                    .HasColumnType("time");

                ts.Property(p => p.EndTime)
                    .HasColumnName("EndTime")
                    .HasColumnType("time")
                    .IsRequired(false);
            });

            builder.Property(e => e.ImageUrl)
                .IsRequired()
                .HasMaxLength(2048);

            builder.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Content)
                .IsRequired()
                .HasColumnType("longtext");

            builder.OwnsOne(e => e.Address, a =>
            {
                a.Property(p => p.Addressline1).HasColumnName("AddresLine1").IsRequired();
                a.Property(p => p.City).HasColumnName("City").IsRequired();
                a.Property(p => p.Addressline2).HasColumnName("AddresLine2").IsRequired();
                a.Property(p => p.PostalCode).HasColumnName("PostalCode").IsRequired();
            });

            builder.Property(e => e.Price)
                .HasPrecision(10, 2);

            builder.Property(e => e.RegisterLink)
                .HasMaxLength(2048)
                .IsRequired(false);
        }
    }
}