using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderApi.Application.DTO;
using OrderApi.Application.DTOs;

namespace OrderApi.Application.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId);
        Task<OrderDetailsDTO> GetOrderDetails(int orderId);
    }
}
