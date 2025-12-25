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
    public class RemoteUserEntityConfiguration : IEntityTypeConfiguration<RemoteUser>
    {
        public void Configure(EntityTypeBuilder<RemoteUser> builder)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            builder.ToTable("RemoteUsers");

            builder.HasKey(v => v.Id);
            builder.Property(v => v.Id).ValueGeneratedNever();
            builder.Property(v => v.Name).IsRequired();
            builder.Property(v => v.Email).IsRequired();
        }
    }
}
