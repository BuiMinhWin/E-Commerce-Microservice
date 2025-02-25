using OrderAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderAPI.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO orderDTO) => new Order()
        {
            Id = orderDTO.Id,
            ClientId = orderDTO.ClientId,
            ProductId = orderDTO.ProductId,
            OrderedDate = orderDTO.OrderedDate,
            PurchaseQuantity = orderDTO.PurchaseQuantity
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order?order, IEnumerable<Order>? orders)
        {
            if (order is not null || orders is null) 
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ProductId,
                    order.ClientId,
                    order.PurchaseQuantity,
                    order.OrderedDate);
                    return (singleOrder, null); 
                    
            }
            if (orders is not null || order is null)
            {
                var listOrders = orders!.Select(o => 
                new OrderDTO(
                    o.Id,
                    o.ProductId,
                    o.ClientId,
                    o.PurchaseQuantity,
                    o.OrderedDate));
                return (null, listOrders);
            }
            return (null, null);
        }
    }
}
