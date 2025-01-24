using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model.DTO.Cart;
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
    public class CartDL : ICartDL
    {
        private readonly BookStoreContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartDL(BookStoreContext context, IHttpContextAccessor httpContextAccessor)
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

        public async Task<ResponseModel<Cart>> AddItemToCart(AddCartItemDto newItemDto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            // Fetch the cart for the user, including cart items and related book details
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book) // Ensure Book is included
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId.Value };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Check if the book exists and fetch its stock details
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == newItemDto.BookId);
            if (book == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Book not found",
                    Success = false
                };
            }

            if (book.Stock < newItemDto.Quantity)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = $"Insufficient stock. Only {book.Stock} units available.",
                    Success = false
                };
            }

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.BookId == newItemDto.BookId);
            if (existingItem != null)
            {
                if (existingItem.Quantity + newItemDto.Quantity > book.Stock)
                {
                    return new ResponseModel<Cart>
                    {
                        Data = null,
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = $"Adding this quantity exceeds available stock. Only {book.Stock - existingItem.Quantity} units can be added.",
                        Success = false
                    };
                }

                existingItem.Quantity += newItemDto.Quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    BookId = newItemDto.BookId,
                    Quantity = newItemDto.Quantity
                });
            }

            // Log cart items before recalculating
            Console.WriteLine("Cart Items Before Calculation:");
            foreach (var item in cart.CartItems)
            {
                Console.WriteLine($"BookId: {item.BookId}, Price: {item.Book?.Price}, Quantity: {item.Quantity}");
            }

            // Recalculate the total price
            cart.TotalPrice = CalculateCartTotal(cart.CartItems);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return new ResponseModel<Cart>
            {
                Data = cart,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Item added to cart successfully",
                Success = true
            };
        }




        public async Task<ResponseModel<Cart>> RemoveItemFromCart(int cartItemId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Cart item not found or unauthorized",
                    Success = false
                };
            }

            _context.CartItems.Remove(cartItem);
            cartItem.Cart.TotalPrice = CalculateCartTotal(cartItem.Cart.CartItems);
            await _context.SaveChangesAsync();

            return new ResponseModel<Cart>
            {
                Data = cartItem.Cart,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Item removed from cart successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<Cart>> ClearCart()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Cart not found",
                    Success = false
                };
            }

            _context.CartItems.RemoveRange(cart.CartItems);
            cart.TotalPrice = 0;
            await _context.SaveChangesAsync();

            return new ResponseModel<Cart>
            {
                Data = cart,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Cart cleared successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<CartDto>> GetCartByUserId()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<CartDto>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new ResponseModel<CartDto>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Cart not found",
                    Success = false
                };
            }

            var cartDto = new CartDto
            {
                CartId = cart.CartId,
                TotalPrice = cart.CartItems.Sum(ci => ci.Book.Price * ci.Quantity), // Calculate total price here
                Items = cart.CartItems.Select(ci => new CartItemDto
                {
                    CartItemId = ci.CartItemId,
                    BookId = ci.BookId,
                    BookName = ci.Book.Title, // Assuming a `Title` property in `Book`
                    Quantity = ci.Quantity,
                    Price = ci.Book.Price * ci.Quantity
                }).ToList()
            };

            return new ResponseModel<CartDto>
            {
                Data = cartDto,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Cart retrieved successfully",
                Success = true
            };
        }


        public async Task<ResponseModel<Cart>> UpdateCartItemQuantity(int cartItemId, UpdateCartItemDto updateDto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId && ci.Cart.UserId == userId);

            if (cartItem == null)
            {
                return new ResponseModel<Cart>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Cart item not found or unauthorized",
                    Success = false
                };
            }

            cartItem.Quantity = updateDto.Quantity;
            cartItem.Cart.TotalPrice = CalculateCartTotal(cartItem.Cart.CartItems);
            await _context.SaveChangesAsync();

            return new ResponseModel<Cart>
            {
                Data = cartItem.Cart,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Cart item quantity updated successfully",
                Success = true
            };
        }

        public decimal CalculateCartTotal(IEnumerable<CartItem> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
                return 0;

            return cartItems
                .Where(ci => ci.Book != null)
                .Sum(ci => ci.Book.Price * ci.Quantity);
        }

        public async Task<ResponseModel<bool>> IsBookInCart(IsInCart Id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Cart not found",
                    Success = false
                };
            }

            var exists = cart.CartItems.Any(ci => ci.BookId == Id.bookId);

            return new ResponseModel<bool>
            {
                Data = exists,
                StatusCode = (int)HttpStatusCode.OK,
                Message = exists ? "Book is in the cart" : "Book is not in the cart",
                Success = exists
            };
        }

    }


}

