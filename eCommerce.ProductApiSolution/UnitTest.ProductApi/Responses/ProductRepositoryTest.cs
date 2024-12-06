using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace UnitTest.ProductApi.Responses
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDb").Options;

            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);

        }

        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExist_ReturErrorResponse()
        {
            // Arrage
            var existingProduct = new Product { Name = "ExistingProduct" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.CreateAsync(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
            result.Message.Should().Be("ExistingProduct already added");
        }

        [Fact]
        public async Task CreateAsync_WhenProductDoesNotExist_AddProductAndReturnsSuccessResponse()
        {
            // Arrage
            var product = new Product { Name = "New Product" };

            // Act
            var result = await productRepository.CreateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("New Product added to database successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsFound_ReturnsSuccessResponse()
        {
            // Arrange
            var product = new Product() { Id = 1, Name = "Existing Product", Price = 78.67m, Quantity = 5 };
            productDbContext.Products.Add(product);

            // Act
            var result = await productRepository.DeleteAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
            result.Message.Should().Be("Existing Product is deleted successfully");
        }

    }
}
