using ECommerce.Application.DTOs;
using ECommerce.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItemService _orderItemService;

        public OrderItemsController(IOrderItemService orderItemService)
        {
            _orderItemService = orderItemService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetAll()
        {
            return Ok(await _orderItemService.GetAllAsync());
        }

        [HttpGet("by-order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderItemDto>>> GetByOrderId(Guid orderId)
        {
            var items = await _orderItemService.GetByOrderIdAsync(orderId);
            return Ok(items);
        }

    }
}
