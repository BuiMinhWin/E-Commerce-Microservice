using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Infrastructure.Data
{
    public class ProductDbContext(DbContextOptions<ProductDbContext>options):DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

    }
}
