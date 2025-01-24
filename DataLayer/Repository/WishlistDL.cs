using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model.DTO.Wishlist;
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
    public class WishlistDL : IWishlistDL
    {
        private readonly BookStoreContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WishlistDL(BookStoreContext context, IHttpContextAccessor httpContextAccessor)
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

        public async Task<ResponseModel<Wishlist>> AddToWishlist(AddWishlistItemDto newItemDto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Wishlist>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                wishlist = new Wishlist { UserId = userId.Value };
                _context.Wishlists.Add(wishlist);
                await _context.SaveChangesAsync();
            }

            var existingItem = wishlist.WishlistItems.FirstOrDefault(wi => wi.BookId == newItemDto.BookId);
            if (existingItem != null)
            {
                return new ResponseModel<Wishlist>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = "Book is already in the wishlist",
                    Success = false
                };
            }

            wishlist.WishlistItems.Add(new WishlistItem
            {
                BookId = newItemDto.BookId,
                
            });

            await _context.SaveChangesAsync();

            return new ResponseModel<Wishlist>
            {
                Data = wishlist,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book added to wishlist successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<bool>> ClearWishlist()
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

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Wishlist not found",
                    Success = false
                };
            }

            _context.WishlistItems.RemoveRange(wishlist.WishlistItems);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>
            {
                Data = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Wishlist cleared successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<Wishlist>> GetWishlist()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Wishlist>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .ThenInclude(wi => wi.Book)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return new ResponseModel<Wishlist>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Wishlist not found",
                    Success = false
                };
            }

            return new ResponseModel<Wishlist>
            {
                Data = wishlist,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Wishlist retrieved successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<bool>> RemoveFromWishlist(int bookId)
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

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Wishlist not found",
                    Success = false
                };
            }

            var item = wishlist.WishlistItems.FirstOrDefault(wi => wi.BookId == bookId);
            if (item == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Book not found in wishlist",
                    Success = false
                };
            }

            _context.WishlistItems.Remove(item);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>
            {
                Data = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Book removed from wishlist",
                Success = true
            };
        }

        public async Task<ResponseModel<bool>> IsBookInWishlist(IsinWishlist Id)
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

            var wishlist = await _context.Wishlists
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Wishlist not found",
                    Success = false
                };
            }

            var exists = wishlist.WishlistItems.Any(wi => wi.BookId == Id.bookId);

            return new ResponseModel<bool>
            {
                Data = exists,
                StatusCode = (int)HttpStatusCode.OK,
                Message = exists ? "Book is in wishlist" : "Book is not in wishlist",
                Success = exists
            };
        }


    }
}
