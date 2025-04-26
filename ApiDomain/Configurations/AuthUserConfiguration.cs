using ApiDomain.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Configurations
{
    class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
    {
        public void Configure(EntityTypeBuilder<AuthUser> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne<Customer>()
                .WithOne()
                .HasForeignKey<AuthUser>(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
