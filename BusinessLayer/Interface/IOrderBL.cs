using Model.DTO.Order;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IOrderBL
    {
        public Task<ResponseModel<OrderDto>> PlaceOrder(PlaceOrderDto placeOrderDto);
        public Task<ResponseModel<OrderDto>> GetOrderById(int orderId);
        public Task<ResponseModel<List<OrderDto>>> GetAllOrders();
        public Task<ResponseModel<bool>> DeleteOrder(int orderId);
    }
}
