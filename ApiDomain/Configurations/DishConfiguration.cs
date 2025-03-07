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
    public class DishConfiguration : IEntityTypeConfiguration<Dish>
    {
        public void Configure(EntityTypeBuilder<Dish> builder)
        {
            builder.HasKey(x => x.DishId);
            builder.HasMany(x => x.CartDetails)
                .WithOne(x => x.Dish)
                .HasForeignKey(x => x.DishId);
            builder.HasOne(x => x.Category)
                .WithMany(x => x.Dishes)
                .HasForeignKey(x => x.CategoryId);
        }
    }
}
