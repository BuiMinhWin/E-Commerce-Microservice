using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Data;
using ProductAPI.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ProductAPI.Controller
{
    public class ProductRepoTest
    {
        private readonly ProductDbContext context;
        private readonly ProductRepository repository;

        public ProductRepoTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDb").Options;

            context = new ProductDbContext(options);
            repository = new ProductRepository(context);


        }

        //Create 
        [Fact]
        public async Task CreateAsync_ProductExist_ReturnError()
        {
            // arrange
            var existingProduct = new Product { Name = "Product1" };
            context.Products.Add(existingProduct);
            await context.SaveChangesAsync();

            //act
            var result = await repository.CreateAsync(existingProduct);

            //assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product1 already exist");
        }

        [Fact]
        public async Task CreateAsync_ReturnSuccess()
        {
            // arrange
            var newProduct = new Product() { Id = 1, Name = "Product 1", Quantity = 1, Price = 100 };
          

            //act
            var result = await repository.CreateAsync(newProduct);

            //assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 added successfully");
        }

        //DELETE
        [Fact]
        public async Task DeleteAsync_ProductNotFound_ReturnNotFoundResponse()
        {
            var product = new Product() { Id = 1, Name = "Product 1", Quantity = 1, Price = 100 };


            //act
            var result = await repository.DeleteAsync(product);

            //assert
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product 1 not found");
        }

        [Fact]
        public async Task DeleteAsync_Success()
        {
            var product = new Product() { Id = 1, Name = "Product 1", Quantity = 1, Price = 100 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            //act
            var result = await repository.DeleteAsync(product);

            //assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 is deleted successfully");
        }

        [Fact]
        public async Task FindByIdAsync_Success()
        {
            var product = new Product() { Id = 1, Name = "Product 1", Quantity = 1, Price = 100 };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            //act
            var result = await repository.FindByIdAsync(product.Id);

            //assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            //result.Name.Should().Be("Product 1");            
        }

        [Fact]
        public async Task FindByIdAsync_Notfound()
        {
            var product = new Product() { Id = 1, Name = "Product 1", Quantity = 1, Price = 100 };
           
            //act
            var result = await repository.FindByIdAsync(product.Id);

            //assert
            result.Should().BeNull();
           

        }

        [Fact]
        public async Task GetAllAsync_ReturnAllProducts()
        {
            var products = new List<Product>()
            {
                 new Product { Id = 1,Name = "Product 1", Quantity = 1 ,Price = 100},
                 new Product { Id = 2,Name = "Product 2", Quantity = 2 ,Price = 200},
                 new Product { Id = 3,Name = "Product 3", Quantity = 3 ,Price = 300}
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            var result = await repository.GetAllAsync();

            result.Should().NotBeNull();
            result.Count().Should().Be(3);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
            result.Should().Contain(p => p.Name == "Product 3");
        }

        [Fact]
        public async Task GetAllAsync_ReturnNull()
        { 
       
            var result = await repository.GetAllAsync();

            result.Should().BeEmpty();

        }

        //UPDATE PRODUCT
        [Fact]
        public async Task UpdateAsync_ProductNotfound()
        {
            var product = new Product() { Id = 1, Name = "Product 1", Quantity = 1, Price = 100 };

            var result = await repository.UpdateAsync(product);

            //assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("Product 1 not found");

        }

        [Fact]
        public async Task UpdateAsync_Success()
        {
            var product = new Product() { Id = 1, Name = "Product 1", Quantity = 1, Price = 100 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            //act
            var result = await repository.UpdateAsync(product);

            //assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Product 1 update successfully");
        }
    }
}
