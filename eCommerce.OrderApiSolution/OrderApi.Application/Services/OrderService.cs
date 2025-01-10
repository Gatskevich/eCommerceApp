using eCommerce.SharedLibrary.DTOs.Requests;
using eCommerce.SharedLibrary.DTOs.Responses;
using MassTransit;
using OrderApi.Application.DTO;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;

namespace OrderApi.Application.Services
{
    public class OrderService( 
        IOrder orderInterface,
        IRequestClient<GetProductRequest> productClient,
        IRequestClient<GetUserRequest> userClient) : IOrderService
    {
        //GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            try
            {
                var productRequest = new GetProductRequest(productId);
                var response = await productClient.GetResponse<ProductDTO>(productRequest);
                var product = response.Message;

                return product;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        //GET USER
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var userRequest = new GetUserRequest(userId);
            var response = await userClient.GetResponse<GetUserDTO>(userRequest);
            var user = response.Message;

            return user;
        }

        // GET ORDER DETAILS BY ID
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            // Prepare Order
            var order = await orderInterface.FindByIdAsync(orderId);

            if(order is null || order!.Id <= 0)
            {
                return null!;
            }

            // Prepare Product
            var productDTO = await GetProduct(order.ProductId);

            // Prepare Client
            var appUserDTO = await GetUser(order.ClientId);

            // Populate order Details
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuantity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuantity,
                order.OrderedDate
                );
        }

        // GET ORDERS BY CLIENT ID
        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            // Get all Client's orders
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);
            if(!orders.Any())
            {
                return null!;
            }

            // Convert from entity to DTO
            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
