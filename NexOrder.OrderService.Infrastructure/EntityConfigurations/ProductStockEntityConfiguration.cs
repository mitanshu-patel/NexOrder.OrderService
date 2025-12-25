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
    public class ProductStockEntityConfiguration : IEntityTypeConfiguration<ProductStock>
    {
        public void Configure(EntityTypeBuilder<ProductStock> builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            builder.ToTable("ProductStocks");
            builder.HasKey(v => v.Id);
            builder.Property(v => v.AvailableQuantity).IsRequired();
            builder.HasOne(v => v.Product).WithOne(v => v.ProductStock).HasForeignKey<ProductStock>(t => t.ProductId);
        }
    }
}
