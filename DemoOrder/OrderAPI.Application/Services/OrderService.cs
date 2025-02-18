using OrderAPI.Application.DTOs;
using OrderAPI.Application.DTOs.Conversions;
using OrderAPI.Application.Interfaces;
using Polly;
using Polly.Registry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderAPI.Application.Services
{
    public class OrderService(IOrder orderInterface,HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        public async Task<ProductDTO> GetProduct(int productId)
        {
            var getProduct = await httpClient.GetAsync($"/api/Product/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
                return product;
        }
        public async Task<AppUserDTO> GetUser(int userId)
        {

            var getUser= await httpClient.GetAsync($"/api/Authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null;
            var user = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return user;
        }
        public async Task<OrderDetailDTO> GetOrderDetail(int orderId)
        {
            //prepare order
            var order = await orderInterface.FindByIdAsync(orderId);
            if (order is null || order!.Id <= 0)
                return null;
            

            //get retry pipeline
            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            //prepare product
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            //prepare client
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            //populate order details
            return new OrderDetailDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.PhoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if (!orders.Any()) return null;
            
            //convert from entity to DTO
            var(_, listOrder)= OrderConversion.FromEntity(null, orders);
            return listOrder;
        }
    }
}
