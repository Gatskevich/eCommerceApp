using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTO;

namespace OrderApi.IntegrationTest
{
    public class OrderIntegrationTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;

        public OrderIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public Task InitializeAsync() => _factory.InitializeAsync();

        public Task DisposeAsync() => _factory.DisposeAsync();

        [Fact]
        public async Task CreateOrder_ShouldReturnConflict_WhenOrderWithSameIdAlreadyExists()
        {
            // Arrange
            var existingOrder = new OrderDTO(1, 123, 456, 10, DateTime.UtcNow);

            // Post the initial order to create it
            var initialResponse = await _factory.HttpClient.PostAsJsonAsync("/api/orders", existingOrder);
            initialResponse.StatusCode.Should().Be(HttpStatusCode.OK, "the first order creation should succeed");

            // Try to create the same order again
            var duplicateOrderRequest = new OrderDTO(1, 123, 456, 10, DateTime.UtcNow);

            // Act
            var duplicateResponse = await _factory.HttpClient.PostAsJsonAsync("/api/orders", duplicateOrderRequest);
            var validationResult = await duplicateResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            // Assert
            duplicateResponse.StatusCode.Should().Be(HttpStatusCode.Conflict, "a duplicate order should result in a conflict response");
            validationResult?.Errors.Should().HaveCount(1, "there should be one validation error for the conflict");

            var error = validationResult?.Errors.FirstOrDefault();
            error?.Key.Should().Be("Order with ID '1' already exists", "the conflict error should identify the duplicate order ID");
            error?.Value.First().Should().Be("A conflict error has occurred.", "the error message should explain the conflict");
        }
    }
}
