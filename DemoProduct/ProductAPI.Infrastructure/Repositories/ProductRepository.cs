using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductAPI.Application.Interfaces;
using ProductAPI.Domain.Entities;
using ProductAPI.Infrastructure.Data;

namespace ProductAPI.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context): IProduct
    {
        public async Task<Response> CreateAsync(Product entity)
        {
            try
            {
                //check if the product existed
                var getProduct = await GetByAsync(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} already exist");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();
                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} added successfully");
                return new Response(false, $"Error occured while adding {entity.Name}");

            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptionx(ex);

                //display scary-free message to the client
                return new Response(false , "Error occured when adding new product");
            }
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            var product = await FindByIdAsync(entity.Id);
            if (product is null && !string.IsNullOrEmpty(entity.Name))
                return new Response(false, $"{entity.Name} not found");
            context.Entry(product).State = EntityState.Detached;
            context.Products.Update(entity);
            await context.SaveChangesAsync();
            return new Response(true, $"{entity.Name} update successfully");
        }

        public async Task<Response> DeleteAsync(Product entity)
        {
            try
            {
                //check if the product existed
                var product = await FindByIdAsync(entity.Id);
                if (product is null) return new Response(false, $"{entity.Name} not found");
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is deleted successfully");

            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptionx(ex);

                //display scary-free message to the client
                return new Response(false, "Error occurred when delete new product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                var products = await context.Products.AsNoTracking().ToListAsync();
                return products is not null ? products : null;

            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptionx(ex);

                //display scary-free message to the client
                throw new Exception("Error occurred when find product");
            }
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null;

            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptionx(ex);

                //display scary-free message to the client
                throw new Exception("Error occurred when find product");
            }
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync()!;
                return product is not null ? product : null;
            }
            catch (Exception ex)
            {
                //Log the original exception
                LogException.LogExceptionx(ex);

                //display scary-free message to the client
                throw new Exception("Error occurred when find product");
            }
        }
    }
}
