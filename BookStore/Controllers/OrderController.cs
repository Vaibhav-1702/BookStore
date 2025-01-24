using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO.Order;
using Model.Utility;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderBL _orderBL;

        public OrderController(IOrderBL orderBL)
        {
            _orderBL = orderBL; 
        }

        [HttpDelete("DeleteOrder")]
        public async Task<ResponseModel<bool>> DeleteOrder(int orderId)
        {
            return await _orderBL.DeleteOrder(orderId);
        }


        [HttpGet("GetAllOrder")]
        public async Task<ResponseModel<List<OrderDto>>> GetAllOrders()
        {
            return await _orderBL.GetAllOrders();
        }


        [HttpGet("GetOrderById")]
        public async Task<ResponseModel<OrderDto>> GetOrderById(int orderId)
        {
            return await _orderBL.GetOrderById(orderId);
        }


        [HttpPost("PlaceOrder")]
        public async Task<ResponseModel<OrderDto>> PlaceOrder(PlaceOrderDto placeOrderDto)
        {
            return await _orderBL.PlaceOrder(placeOrderDto);
        }
    }
}
