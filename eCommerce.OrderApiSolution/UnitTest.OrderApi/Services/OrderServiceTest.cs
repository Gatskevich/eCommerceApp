using System.Net;
using System.Net.Http.Json;
using FakeItEasy;
using FluentAssertions;
using OrderApi.Application.DTOs;
using OrderApi.Application.Services;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderService orderServiceInterface;
        public OrderServiceTest() => orderServiceInterface = A.Fake<IOrderService>();

        public class FakeHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response;

            public FakeHttpMessageHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        private static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o)
            };

            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);

            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };

            return _httpClient;
        }

        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            // Arrange
            int productId = 1;
            var productDTO = new ProductDTO(1, "Product 1", 13, 56.78m);
            var _httpClient = CreateFakeHttpClient(productDTO);
            var _orderService = new OrderService(null!, _httpClient, null!);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            // Arrange
            int productId = 1;
            var _httpClient = CreateFakeHttpClient(null!);
            var _orderService = new OrderService(null!, _httpClient, null!);

            // Act
            var result = await _orderService.GetProduct(productId);

            // Assert
            result.Should().BeNull();
        }
    }
}
