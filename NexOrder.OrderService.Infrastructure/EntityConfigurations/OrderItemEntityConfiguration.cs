using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexOrder.OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.OrderService.Infrastructure.EntityConfigurations
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
           ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            builder.ToTable("OrderItems");

            builder.HasKey(v => v.Id);
            builder.HasOne(v => v.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(v => v.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(v => v.UnitPrice).IsRequired();

            builder.HasOne(v => v.Product)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
