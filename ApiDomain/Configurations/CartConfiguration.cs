using ApiDomain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiDomain.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        { 
            builder.HasKey(x => x.CartId);
            builder.HasOne(x => x.Customer) //связь 1->1 c клиентом
                .WithOne(x => x.Cart)
                .HasForeignKey<Cart>(x => x.CustomerId);
            builder.HasMany(x => x.CartDetails)//связи 1->Many с леталями корзины
                .WithOne(x => x.Cart)
                .HasForeignKey(x => x.CartId);
        }
    }
}
