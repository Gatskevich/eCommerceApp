﻿using System.Linq.Expressions;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrderDbContext context) : IOrder
    {
        public async Task<Response> CreateAsync(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();

                if(order.Id > 0)
                {
                    return new Response(true, "Order placed successfully");
                }
                else
                {
                    return new Response(false, "Error occured while placing order");
                }
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                // display message to the client
                return new Response(false, "Error occured while placing order");
            }
        }

        public async Task<Response> DeleteAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);

                if (order is null)
                {
                    return new Response(false, "Order not found");
                }

                context.Orders.Remove(entity);
                await context.SaveChangesAsync();

                return new Response(true, "Order successfully deleted");
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                // display message to the client
                return new Response(false, "Error occured while deleting order");
            }
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            try
            {
                var order = await context.Orders!.FindAsync(id);

                if (order is null)
                {
                    return null!;
                }

                return order;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                // display message to the client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                var orders = await context.Orders.AsNoTracking().ToListAsync();

                if (orders is null)
                {
                    return null!;
                }

                return orders;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                // display message to the client
                throw new Exception("Error occured while retrieving orders");
            }
        }

        public async Task<Order> GetByAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync();

                if (order is null)
                {
                    return null!;
                }

                return order;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                // display message to the client
                throw new Exception("Error occured while retrieving order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.Orders.Where(predicate).ToListAsync();

                if (orders is null)
                {
                    return null!;
                }

                return orders;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                // display message to the client
                throw new Exception("Error occured while retrieving orders");
            }
        }

        public async Task<Response> UpdateAsync(Order entity)
        {
            try
            {
                var order = await FindByIdAsync(entity.Id);

                if (order is null)
                {
                    return new Response(false, $"Order not found");
                }

                context.Entry(order).State = EntityState.Detached;
                context.Orders.Update(entity);
                await context.SaveChangesAsync();

                return new Response(true, "Order updated");
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                // display message to the client
                return new Response(false, "Error occured while placing order");
            }
        }
    }
}