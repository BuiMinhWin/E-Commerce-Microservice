using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderAPI.Application.DTOs;
using OrderAPI.Application.DTOs.Conversions;
using OrderAPI.Application.Interfaces;
using OrderAPI.Application.Services;

namespace OrderAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            var orders = await orderInterface.GetAllAsync();
            if (!orders.Any()) return NotFound("Not found");
            var (_, list) = OrderConversion.FromEntity(null, orders);
            return (!list.Any()) ? NotFound() : Ok(list);
        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);
            if (order is null) return NotFound(null);
            var (_order, _) = OrderConversion.FromEntity(order, null);
            return Ok(_order);
        }
        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0) return NotFound("not found client");
            var orders = await orderService.GetOrdersByClientId(clientId);
            return !orders.Any() ? NotFound() : Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDetailDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0) return NotFound("not found order");
            var orderDetail = await orderService.GetOrderDetail(orderId);
            return orderDetail.OrderId > 0 ? Ok(orderDetail) : NotFound("no order found");
        }

       [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest("incomplete");
            var getEntity = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.CreateAsync(getEntity);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO orderDTO)
        {
            //convert from dto to entity
            var order = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.UpdateAsync(order);
            return response.Flag ? Ok(response) : BadRequest(response);
        }

       

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO orderDTO)
        {
            var order = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.DeleteAsync(order);
            return response.Flag? Ok(response) : BadRequest(response);  
        }

    }
}
