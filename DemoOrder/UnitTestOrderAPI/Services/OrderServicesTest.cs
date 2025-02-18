using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text;
using FakeItEasy;
using FluentAssertions;
using OrderAPI.Application.DTOs;
using OrderAPI.Application.Interfaces;
using OrderAPI.Application.Services;
using OrderAPI.Domain.Entities;

namespace UnitTestOrderAPI.Services
{
    public class OrderServicesTest
    {
        private readonly IOrderService service;
        private readonly IOrder interfaceOrder;

        public OrderServicesTest() 
        {
            interfaceOrder = A.Fake<IOrder>();
            service = A.Fake<IOrderService>();
        }

        public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_response);

        }

        private static HttpClient CreateFakeHttpClient(object e)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(e)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            return _httpClient;
        }

        [Fact]
        public async Task GetProduct_ValidId_ReturnProduct()
        {
            int productId = 1;
            var productDTO = new ProductDTO(1, "Product 1", 13, 123);
            var _httpClient = CreateFakeHttpClient(productDTO);

            //System under test -sut
            //we only the httpclient to make calls
            //specify only httpclient and null to the rest
            var _orderService = new OrderService(null, _httpClient, null);

            //act
            var result = await _orderService.GetProduct(productId);

            //assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);

        }

        [Fact]
        public async Task GetProduct_InvalidId_ReturnNull()
        {
            int productId = 1;

            var _httpClient = CreateFakeHttpClient(null);

            var _orderService = new OrderService(null, _httpClient, null);

            //act
            var result = await _orderService.GetProduct(productId);

            //assert
            result.Should().BeNull();


        }

        [Fact]
        public async Task GetOrdersByClientId_OrderExist()
        {
            int clientId = 1;
            var order = new List<Order>
            {
                new (){ Id = 1, ProductId = 1, ClientId = clientId,PurchaseQuantity = 1, OrderedDate = DateTime.UtcNow},
                new (){ Id = 1, ProductId = 2, ClientId = clientId,PurchaseQuantity = 1, OrderedDate = DateTime.UtcNow},
            };

            //mock the GetOrder
            A.CallTo(() => interfaceOrder.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored)).Returns(order);
            var _orderService = new OrderService(interfaceOrder ,null, null);
           //act
           var result = await _orderService.GetOrdersByClientId(clientId);

            //assert
            result.Should().NotBeNull();
            result.Should().HaveCount(order.Count);
            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}
