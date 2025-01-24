using Model.DTO.Wishlist;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IWishlistBL
    {
        public Task<ResponseModel<Wishlist>> AddToWishlist(AddWishlistItemDto newItemDto);

        public Task<ResponseModel<bool>> ClearWishlist();

        public Task<ResponseModel<Wishlist>> GetWishlist();

        public Task<ResponseModel<bool>> RemoveFromWishlist(int bookId);

        public Task<ResponseModel<bool>> IsBookInWishlist(IsinWishlist Id);
    }
}
