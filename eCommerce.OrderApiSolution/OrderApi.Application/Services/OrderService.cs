﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using OrderApi.Application.DTO;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;

namespace OrderApi.Application.Services
{
    public class OrderService( 
        IOrder orderInterface, 
        HttpClient httpClient, 
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        //GET PRODUCT
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call Product ApI using HttpClient
            // Redirect this call to the ApI Gateway since product Api is not response to outsiders.
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if(!getProduct.IsSuccessStatusCode)
            {
                return null!;
            }

            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;

        }

        //GET USER
        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call Product ApI using HttpClient
            // Redirect this call to the ApI Gateway since product Api is not response to outsiders.
            var getUser = await httpClient.GetAsync($"/api/products/{userId}");
            if (!getUser.IsSuccessStatusCode)
            {
                return null!;
            }

            var product = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return product!;

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

            // Get Retry pipline
            var retryPipline = resiliencePipeline.GetPipeline("my-retry-pipline");

            // Prepare Product
            var productDTO = await retryPipline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            // Prepare Client
            var appUserDTO = await retryPipline.ExecuteAsync(async token => await GetUser(order.ClientId));

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