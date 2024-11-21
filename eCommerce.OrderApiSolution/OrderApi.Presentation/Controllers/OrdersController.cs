using System.Collections.Generic;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTO;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetProducts()
        {
            var orders = await orderInterface.GetAllAsync();

            if (!orders.Any())
            {
                return NotFound("No orders detected in the database");
            }

            var (_, list) = OrderConversion.FromEntity(null!, orders);

            if (!list!.Any())
            {
                return NotFound("No order found");
            }

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            var order = await orderInterface.FindByIdAsync(id);

            if (order is null)
            {
                return NotFound("Order requested not found");
            }

            var (_order, _) = OrderConversion.FromEntity(order, null!);

            return Ok(_order);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
            {
                return BadRequest("Invalid data provided");
            }

            var orders = await orderService.GetOrdersByClientId(clientId);

            if (!orders.Any())
            {
                return NotFound("No order found");
            }

            return Ok(orders);
        }

        [HttpGet("details{orderId:int}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDeatils(int orderId)
        {
            if (orderId <= 0)
            {
                return BadRequest("Invalid data provided");
            }

            var orderDetail = await orderService.GetOrderDetails(orderId);

            if (orderDetail.OrderId > 0)
            {
                return NotFound("No order found");
            }

            return Ok(orderDetail);
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(OrderDTO order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getEntity = OrderConversion.ToEntity(order);
            var response = await orderInterface.CreateAsync(getEntity);

            if (response.Flag is false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateOrder(OrderDTO product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var getEntity = OrderConversion.ToEntity(product);
            var response = await orderInterface.UpdateAsync(getEntity);

            if (response.Flag is false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteOrder(OrderDTO product)
        {
            var getEntity = OrderConversion.ToEntity(product);
            var response = await orderInterface.DeleteAsync(getEntity);

            if (response.Flag is false)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
