using DataLayer.Context;
using DataLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Model.DTO;
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
    public class AddressDL : IAddressDL
    {
        private readonly BookStoreContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddressDL(BookStoreContext context, IHttpContextAccessor httpContextAccessor)
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

        public async Task<ResponseModel<Address>> AddAddress(AddressDto newAddressDto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Address>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var address = new Address
            {
                UserId = userId.Value,
                AddressLine = newAddressDto.AddressLine,
                City = newAddressDto.City,
                State = newAddressDto.State,
                PinCode = newAddressDto.PinCode,
                Country = newAddressDto.Country
            };

            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();

            return new ResponseModel<Address>
            {
                Data = address,
                StatusCode = (int)HttpStatusCode.Created,
                Message = "Address added successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<Address>> UpdateAddress(int addressId, UpdateAddressDto updateDto)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Address>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);
            if (address == null)
            {
                return new ResponseModel<Address>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Address not found or unauthorized",
                    Success = false
                };
            }

            address.AddressLine = updateDto.AddressLine;
            address.City = updateDto.City;
            address.State = updateDto.State;
            address.PinCode = updateDto.PinCode;
            address.Country = updateDto.Country;

            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();

            return new ResponseModel<Address>
            {
                Data = address,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Address updated successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<Address>> GetAddressById(int addressId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<Address>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);
            if (address == null)
            {
                return new ResponseModel<Address>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Address not found or unauthorized",
                    Success = false
                };
            }

            return new ResponseModel<Address>
            {
                Data = address,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Address retrieved successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<List<Address>>> GetAllAddresses()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return new ResponseModel<List<Address>>
                {
                    Data = null,
                    StatusCode = (int)HttpStatusCode.Unauthorized,
                    Message = "User is not authenticated",
                    Success = false
                };
            }

            var addresses = await _context.Addresses.Where(a => a.UserId == userId).ToListAsync();

            return new ResponseModel<List<Address>>
            {
                Data = addresses,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Addresses retrieved successfully",
                Success = true
            };
        }

        public async Task<ResponseModel<bool>> DeleteAddress(int addressId)
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

            var address = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);
            if (address == null)
            {
                return new ResponseModel<bool>
                {
                    Data = false,
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = "Address not found or unauthorized",
                    Success = false
                };
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return new ResponseModel<bool>
            {
                Data = true,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Address deleted successfully",
                Success = true
            };
        }
    }
}
