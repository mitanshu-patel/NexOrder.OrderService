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
    public class RemoteProductEntityConfiguration : IEntityTypeConfiguration<RemoteProduct>
    {
        public void Configure(EntityTypeBuilder<RemoteProduct> builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            builder.ToTable("RemoteProducts");

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).ValueGeneratedNever();
            builder.Property(v => v.Name).IsRequired();
            builder.Property(v => v.Description).IsRequired();
            builder.Property(v => v.Price).IsRequired();
        }
    }
}
