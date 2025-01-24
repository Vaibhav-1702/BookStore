using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.DTO.Cart;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class CartBL : ICartBL
    {
        private readonly ICartDL _cartDL;

        public CartBL(ICartDL cartDL)
        {
            _cartDL = cartDL; 
        }

        public async Task<ResponseModel<Cart>> AddItemToCart(AddCartItemDto newItemDto)
        {
            return await _cartDL.AddItemToCart(newItemDto);
        }

      

        public async Task<ResponseModel<Cart>> ClearCart()
        {
            return await _cartDL.ClearCart();
        }

        public async Task<ResponseModel<CartDto>> GetCartByUserId()
        {
            return await _cartDL.GetCartByUserId();
        }

        public async Task<ResponseModel<Cart>> RemoveItemFromCart(int cartItemId)
        {
            return await _cartDL.RemoveItemFromCart(cartItemId);
        }

        public async Task<ResponseModel<Cart>> UpdateCartItemQuantity(int cartItemId, UpdateCartItemDto updateDto)
        {
            return await _cartDL.UpdateCartItemQuantity(cartItemId, updateDto);
        }

        public async Task<ResponseModel<bool>> IsBookInCart(IsInCart Id)
        {
            return await _cartDL.IsBookInCart(Id);
        }
    }
}
