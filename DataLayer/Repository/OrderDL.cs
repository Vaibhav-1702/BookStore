using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model.DTO.Order;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repository
{
    public class OrderDL : IOrderDL
    {
        private readonly BookStoreContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public OrderDL(BookStoreContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int? GetUserIdFromClaims()
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = userClaims?.Claims.FirstOrDefault(c => c.Type == "UserId");
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }

        public async Task<ResponseModel<OrderDto>> PlaceOrder(PlaceOrderDto placeOrderDto)
        {
            // Step 1: Validate User Authentication
            var userId = GetUserIdFromClaims(); // Fetch the UserId from claims
            if (userId == null)
            {
                return new ResponseModel<OrderDto>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            // Step 2: Validate Address
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == placeOrderDto.AddressId && a.UserId == userId.Value);
            if (address == null)
            {
                return new ResponseModel<OrderDto>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Address not found",
                    Success = false
                };
            }

            // Step 3: Validate Cart and Cart Items
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null || !cart.CartItems.Any())
            {
                return new ResponseModel<OrderDto>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Cart is empty or not found",
                    Success = false
                };
            }

            // Step 4: Initialize Order
            var order = new Order
            {
                UserId = userId.Value,
                TotalAmount = 0, // Initial value, will be calculated
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                AddressId = address.AddressId // Associate the order with the validated address
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // Save the order to get its ID

            decimal totalAmount = 0;

            // Step 5: Process Cart Items and Add to Order
            foreach (var cartItem in cart.CartItems)
            {
                var book = cartItem.Book;
                if (book.Stock < cartItem.Quantity)
                {
                    return new ResponseModel<OrderDto>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"Not enough stock for book {book.Title}. Only {book.Stock} units available.",
                        Success = false
                    };
                }

                // Update stock
                book.Stock -= cartItem.Quantity;

                // Create order item
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    BookId = cartItem.BookId,
                    Quantity = cartItem.Quantity,
                    PurchasePrice = book.Price
                };

                totalAmount += orderItem.PurchasePrice * cartItem.Quantity;

                _context.OrderItems.Add(orderItem);
            }

            // Step 6: Update Order Details
            order.TotalAmount = totalAmount;
            await _context.SaveChangesAsync(); // Save all changes for order and order items

            // Step 7: Clear Cart
            _context.CartItems.RemoveRange(cart.CartItems);
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            // Step 8: Prepare Response DTO
            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                UserId = order.UserId,         
                AddressId = order.AddressId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    BookId = oi.BookId,
                    Quantity = oi.Quantity,
                    PurchasePrice = oi.PurchasePrice
                }).ToList()
            };

            return new ResponseModel<OrderDto>
            {
                Data = orderDto,
                StatusCode = (int)HttpStatusCode.Created,
                Message = "Order placed successfully",
                Success = true
            };
        }



        public async Task<ResponseModel<OrderDto>> GetOrderById(int orderId)
        {
            // Step 1: Validate User Authentication
            var userId = GetUserIdFromClaims(); // Fetch the UserId from claims
            if (userId == null)
            {
                return new ResponseModel<OrderDto>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            // Step 2: Fetch and Validate Order
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId.Value);

            if (order == null)
            {
                return new ResponseModel<OrderDto>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Order not found or access denied",
                    Success = false
                };
            }

            // Step 3: Map to DTO
            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                AddressId = order.AddressId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    BookAuthor = oi.Book.Author,
                    Quantity = oi.Quantity,
                    PurchasePrice = oi.PurchasePrice
                }).ToList()
            };

            return new ResponseModel<OrderDto>
            {
                Data = orderDto,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Order retrieved successfully",
                Success = true
            };
        }



        public async Task<ResponseModel<List<OrderDto>>> GetAllOrders()
        {
            // Step 1: Validate User Authentication
            var userId = GetUserIdFromClaims(); // Fetch the UserId from claims
            if (userId == null)
            {
                return new ResponseModel<List<OrderDto>>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            // Step 2: Fetch Orders for the Authenticated User
            var orders = await _context.Orders
                .Where(o => o.UserId == userId.Value)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();

            // Step 3: Map to DTOs
            var orderDtos = orders.Select(order => new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                AddressId = order.AddressId,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    BookId = oi.BookId,
                    BookTitle = oi.Book.Title,
                    BookAuthor = oi.Book.Author,
                    Quantity = oi.Quantity,
                    PurchasePrice = oi.PurchasePrice
                }).ToList()
            }).ToList();

            return new ResponseModel<List<OrderDto>>
            {
                Data = orderDtos,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Orders retrieved successfully",
                Success = true
            };
        }



        public async Task<ResponseModel<bool>> DeleteOrder(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Order not found",
                    Success = false
                };
            }

            // Delete order items and restore book stock
            var orderItems = await _context.OrderItems.Where(oi => oi.OrderId == orderId).ToListAsync();
            foreach (var orderItem in orderItems)
            {
                var book = await _context.Books.FindAsync(orderItem.BookId);
                if (book != null)
                {
                    book.Stock += orderItem.Quantity;  // Restore stock
                }
                _context.OrderItems.Remove(orderItem);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>
            {
                Data = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Order deleted successfully",
                Success = true
            };
        }




    }
}
