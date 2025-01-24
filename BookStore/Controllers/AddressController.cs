using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.DTO;
using Model.Model;
using Model.Utility;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressBL _addressBL;

        public AddressController(IAddressBL addressBL)
        {
            _addressBL = addressBL; 
        }


        [HttpPost("AddAddress")]
        public async Task<ResponseModel<Address>> AddAddress(AddressDto newAddressDto)
        {
            return await _addressBL.AddAddress(newAddressDto);
        }


        [HttpDelete("DeleteAddress")]
        public async Task<ResponseModel<bool>> DeleteAddress(int addressId)
        {
            return await _addressBL.DeleteAddress(addressId);
        }


        [HttpGet("GetAddressById")]
        public async Task<ResponseModel<Address>> GetAddressById(int addressId)
        {
            return await _addressBL.GetAddressById(addressId);
        }


        [HttpGet("GetAllAddress")]
        public async Task<ResponseModel<List<Address>>> GetAllAddresses()
        {
            return await _addressBL.GetAllAddresses();
        }


        [HttpPut("UpdateAddress")]
        public async Task<ResponseModel<Address>> UpdateAddress(int addressId, UpdateAddressDto updateDto)
        {
            return await _addressBL.UpdateAddress(addressId, updateDto);
        }
    }
}
