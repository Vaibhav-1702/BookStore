using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface
{
     public interface IAddressDL
    {
        public Task<ResponseModel<Address>> AddAddress(AddressDto newAddressDto);

        public Task<ResponseModel<Address>> UpdateAddress(int addressId, UpdateAddressDto updateDto);

        public Task<ResponseModel<Address>> GetAddressById(int addressId);

        public Task<ResponseModel<List<Address>>> GetAllAddresses();

        public Task<ResponseModel<bool>> DeleteAddress(int addressId);
    }
}
