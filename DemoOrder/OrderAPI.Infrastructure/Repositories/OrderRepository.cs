using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Application.Interfaces;
using OrderAPI.Domain.Entities;
using OrderAPI.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OrderAPI.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0
                    ? new Response(true, $"Order created successfully with ID: {order.Id}")
                    : new Response(false, "Order creation failed. Please try again.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptionx(ex);
                return new Response(false, $"Error occurred while creating order: {ex.Message}");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null) return new Response(false, $"Order with ID: {entity.Id} not found.");
                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return new Response(true, $"Order with ID: {entity.Id} deleted successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptionx(ex);
                return new Response(false, $"Error occurred while deleting order: {ex.Message}");
            }
        }

        public async Task<Order?> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders.FindAsync(id);
                if (order is not null)
                {
                    return order;
                }
                throw new Exception($"Order with ID: {id} not found.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptionx(ex);
                throw new Exception($"Error occurred while retrieving order with ID: {id}. Details: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var listOrder = await context.Orders.AsNoTracking().ToListAsync();
                return listOrder ?? throw new Exception("No orders found in the database.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptionx(ex);
                throw new Exception($"Error occurred while retrieving all orders: {ex.Message}", ex);
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync();
                return order ?? throw new Exception("Order matching the condition not found.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptionx(ex);
                throw new Exception($"Error occurred while retrieving order by condition: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.Orders.Where(predicate).ToListAsync();
                return orders ?? throw new Exception("No orders found matching the condition.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptionx(ex);
                throw new Exception($"Error occurred while retrieving orders by condition: {ex.Message}", ex);
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);
                if (order is null)
                {
                    return new Response(false, $"Order with ID: {entity.Id} not found.");
                }
                context.Entry(order).State = EntityState.Detached;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"Order with ID: {entity.Id} updated successfully.");
            }
            catch (Exception ex)
            {
                LogException.LogExceptionx(ex);
                return new Response(false, $"Error occurred while updating order with ID: {entity.Id}. Details: {ex.Message}");
            }
        }
    }
}
