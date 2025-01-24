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
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public UserController(IUserBL userBL)
        {
           _userBL = userBL;
                
        }

          [HttpGet("Test")]
    public IActionResult Test()
    {
        return Ok("Test endpoint is working");
    }

        [HttpPost("RegisterUser")]
        public async Task<ResponseModel<User>> Registration(UserRegistration userRegistration)
        {
            return await _userBL.Registration(userRegistration);
        }


        [HttpPut("UpdateUser")]
        public async Task<ResponseModel<User>> Update(UpdateUser updateUser)
        {
            return await _userBL.Update(updateUser);
        }


        [HttpGet("GetAllUsers")]
        public async Task<ResponseModel<IEnumerable<User>>> GetAllUsers()
        {
            return await _userBL.GetAllUsers();
        }

        [HttpPost("ValidateUser")]
        public async Task<User> ValidateUser(Login login)
        {
            return await _userBL.ValidateUser(login);
        }

        [HttpPost("Login")]
        public async Task<ResponseModel<LoginResponse>> Login(Login user)
        {
            return await _userBL.Login(user);
        }

        [HttpPost("ForgetPassword")]
        public async Task<ResponseModel<string>> ForgotPassword(string email)
        {
            return await _userBL.ForgotPassword(email);
        }

        [HttpPost("ResetPassword")]
        public async Task<ResponseModel<string>> ResetPassword(string token, string newPassword)
        {
            return await _userBL.ResetPassword(token, newPassword);
        }

        [HttpGet("GetUser")]
        public async Task<ResponseModel<User>> GetUserDetails()
        {
            return await _userBL.GetUserDetails();
        }
    }
}
