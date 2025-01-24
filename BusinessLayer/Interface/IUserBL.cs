using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        public Task<ResponseModel<User>> Registration(UserRegistration userRegistration);

        public Task<ResponseModel<User>> Update( UpdateUser updateUser);

        public Task<ResponseModel<IEnumerable<User>>> GetAllUsers();

        public Task<User> ValidateUser(Login login);

        public Task<ResponseModel<LoginResponse>> Login(Login user);

        public Task<ResponseModel<string>> ForgotPassword(string email);

        public Task<ResponseModel<string>> ResetPassword(string token, string newPassword);

        public Task<ResponseModel<User>> GetUserDetails();
    }
}
