using BusinessLayer.Interface;
using DataLayer.Interface;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class AddressBL : IAddressBL
    {
        private readonly IAddressDL _addressDL;

        public AddressBL(IAddressDL addressDL)
        {
            _addressDL = addressDL; 
        }
        public async Task<ResponseModel<Address>> AddAddress(AddressDto newAddressDto)
        {
            return await _addressDL.AddAddress(newAddressDto);
        }

        public async Task<ResponseModel<bool>> DeleteAddress(int addressId)
        {
            return await _addressDL.DeleteAddress(addressId);
        }

        public async Task<ResponseModel<Address>> GetAddressById(int addressId)
        {
            return await _addressDL.GetAddressById(addressId);
        }

        public async Task<ResponseModel<List<Address>>> GetAllAddresses()
        {
           return await _addressDL.GetAllAddresses();
        }

        public async Task<ResponseModel<Address>> UpdateAddress(int addressId, UpdateAddressDto updateDto)
        {
            return await _addressDL.UpdateAddress(addressId, updateDto);
        }
    }
}
