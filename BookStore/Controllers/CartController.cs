using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO.Cart;
using Model.Model;
using Model.Utility;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartBL _cartBL;

        public CartController(ICartBL cartBL)
        {
            _cartBL = cartBL; 
        }

        [HttpPost("AddItemToCart")]
        public async Task<ResponseModel<Cart>> AddItemToCart(AddCartItemDto newItemDto)
        {
            return await _cartBL.AddItemToCart(newItemDto);
        }


        [HttpDelete("ClearCart")]
        public async Task<ResponseModel<Cart>> ClearCart()
        {
            return await _cartBL.ClearCart();
        }


        [HttpGet("GetCart")]
        public async Task<ResponseModel<CartDto>> GetCartByUserId()
        {
            return await _cartBL.GetCartByUserId();
        }

        [HttpDelete("RemoveItemFromCart")]
        public async Task<ResponseModel<Cart>> RemoveItemFromCart(int cartItemId)
        {
            return await _cartBL.RemoveItemFromCart(cartItemId);
        }

        [HttpPut("UpdateItemQuantity")]
        public async Task<ResponseModel<Cart>> UpdateCartItemQuantity(int cartItemId, UpdateCartItemDto updateDto)
        {
            return await _cartBL.UpdateCartItemQuantity(cartItemId, updateDto);
        }

        [HttpPost("IsBookInCart")]
        public async Task<ResponseModel<bool>> IsBookInCart(IsInCart Id)
        {
            return await _cartBL.IsBookInCart(Id);
        }
    }
}
