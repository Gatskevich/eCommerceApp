using eCommerce.SharedLibrary.DTOs.Requests;
using eCommerce.SharedLibrary.DTOs.Responses;
using MassTransit;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Application.Consumers;

public class GetProductRequestConsumer(IProduct productInterface) : IConsumer<GetProductRequest>
{
    public async Task Consume(ConsumeContext<GetProductRequest> context)
    {
        var productId = context.Message.ProductId;
        var product = await productInterface.FindByIdAsync(productId);

        if (product is null)
        {
            await context.RespondAsync<ProductDTO>(null!);

            return;
        }

        var (productDTO, _) = ProductConversion.FromEntity(product, null!);

        await context.RespondAsync(new ProductDTO(
            productDTO.Id,
            productDTO.Name,
            productDTO.Quantity,
            productDTO.Price));
    }
}
