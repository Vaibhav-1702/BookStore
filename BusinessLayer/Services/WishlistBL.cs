using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.DTO.Wishlist;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class WishlistBL : IWishlistBL
    {
        private readonly IWishlistDL _wishlistDL;

        public WishlistBL(IWishlistDL wishlistDL)
        {
            _wishlistDL = wishlistDL; 
        }

        public async Task<ResponseModel<Wishlist>> AddToWishlist(AddWishlistItemDto newItemDto)
        {
            return await _wishlistDL.AddToWishlist(newItemDto);
        }

        public async Task<ResponseModel<bool>> ClearWishlist()
        {
            return await _wishlistDL.ClearWishlist();
        }

        public async Task<ResponseModel<Wishlist>> GetWishlist()
        {
            return await _wishlistDL.GetWishlist();
        }

        public async Task<ResponseModel<bool>> IsBookInWishlist(IsinWishlist Id)
        {
            return await _wishlistDL.IsBookInWishlist(Id);
        }

        public async Task<ResponseModel<bool>> RemoveFromWishlist(int bookId)
        {
            return await _wishlistDL.RemoveFromWishlist(bookId);
        }
    }
}
