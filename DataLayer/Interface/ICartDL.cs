using Model.DTO.Cart;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface
{
    public interface ICartDL
    {
        public Task<ResponseModel<Cart>> AddItemToCart(AddCartItemDto newItemDto);

        public  Task<ResponseModel<Cart>> RemoveItemFromCart(int cartItemId);

        public  Task<ResponseModel<Cart>> ClearCart();

        public  Task<ResponseModel<CartDto>> GetCartByUserId();

        public Task<ResponseModel<Cart>> UpdateCartItemQuantity(int cartItemId, UpdateCartItemDto updateDto);

        public Task<ResponseModel<bool>> IsBookInCart(IsInCart Id);
    }
}
