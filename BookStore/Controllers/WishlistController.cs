using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO.Wishlist;
using Model.Model;
using Model.Utility;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistBL _wishlistBL;

        public WishlistController(IWishlistBL wishlistBL)
        {
            _wishlistBL = wishlistBL;  
        }

        [HttpPost("AddToWishlist")]
        public async Task<ResponseModel<Wishlist>> AddToWishlist(AddWishlistItemDto newItemDto)
        {
            return await _wishlistBL.AddToWishlist(newItemDto);
        }

        [HttpDelete("ClearWishList")]
        public async Task<ResponseModel<bool>> ClearWishlist()
        {
            return await _wishlistBL.ClearWishlist();
        }

        [HttpGet("GetWishlist")]
        public async Task<ResponseModel<Wishlist>> GetWishlist()
        {
            return await _wishlistBL.GetWishlist();
        }

        [HttpPost("IsBookInWishlist")]
        public async Task<ResponseModel<bool>> IsBookInWishlist(IsinWishlist Id)
        {
            return await _wishlistBL.IsBookInWishlist(Id);
        }

        [HttpDelete("RemoveFromWishlist")]
        public async Task<ResponseModel<bool>> RemoveFromWishlist(int bookId)
        {
            return await _wishlistBL.RemoveFromWishlist(bookId);
        }
    }
}
