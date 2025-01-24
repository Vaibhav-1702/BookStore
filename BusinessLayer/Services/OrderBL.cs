using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.DTO.Order;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class OrderBL : IOrderBL
    {
        private readonly IOrderDL _orderDL;

        public OrderBL(IOrderDL orderDL)
        {
            _orderDL = orderDL;
        }

        public async Task<ResponseModel<bool>> DeleteOrder(int orderId)
        {
            return await _orderDL.DeleteOrder(orderId);
        }

        public async Task<ResponseModel<List<OrderDto>>> GetAllOrders()
        {
            return await _orderDL.GetAllOrders();
        }

        public async Task<ResponseModel<OrderDto>> GetOrderById(int orderId)
        {
            return await _orderDL.GetOrderById(orderId);
        }

        public async Task<ResponseModel<OrderDto>> PlaceOrder(PlaceOrderDto placeOrderDto)
        {
            return await _orderDL.PlaceOrder(placeOrderDto);
        }
    }
}
