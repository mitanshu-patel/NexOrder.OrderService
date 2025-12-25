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
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            builder.HasKey(o => o.Id);
            builder.ToTable("Orders");
            builder.Property(v => v.TotalAmount).IsRequired();

            builder.HasOne(v => v.User).WithMany(t => t.Orders).HasForeignKey(o => o.UserId);
        }
    }
}
