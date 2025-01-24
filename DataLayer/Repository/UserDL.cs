using DataLayer.Context;
using DataLayer.Exceptions;
using DataLayer.Interface;
using DataLayer.Utility;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using MailKit.Net.Smtp;
using Model.DTO;
using Model.Model;
using Model.Utility;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DataLayer.Repository
{
    public class UserDL : IUserDL
    {
        private readonly BookStoreContext _context;
        private readonly TokenUtility _tokenUtility;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserDL(BookStoreContext context, TokenUtility tokenUtility,IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
           _context = context;
            _tokenUtility = tokenUtility;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
                
        }

        private int? GetUserIdFromClaims()
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;
            var userIdClaim = userClaims?.Claims.FirstOrDefault(c => c.Type == "UserId");
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
        public async Task<ResponseModel<User>> Registration(UserRegistration userRegistration)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(user => user.Email.Equals(userRegistration.email));
                if (user != null)
                {
                    throw new UserException("User Already Registered");
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegistration.password);
                user = new User
                {
                    Name = userRegistration.name,
                    Email = userRegistration.email,
                    Password = hashedPassword
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return new ResponseModel<User>
                {
                    Data = user,
                    Message = "User Registered Successfully",
                    StatusCode = (int)HttpStatusCode.Created,
                    Success = true
                };
            }
            catch (UserException)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "User Already Registered",
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Success = false
                };
            }
            catch (Exception)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "An error occurred during registration",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<ResponseModel<User>> Update(UpdateUser updateUser)
        {
            try
            {
                // Retrieve UserId from claims
                var userId = GetUserIdFromClaims();
                if (userId == null)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "User is not authenticated",
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Success = false
                    };
                }

                // Fetch the user by UserId from claims
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);
                if (user == null)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "User not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                // Update user details
                user.Name = updateUser.name;
                user.Email = updateUser.email;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new ResponseModel<User>
                {
                    Data = user,
                    Message = "User updated successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "An error occurred during update",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }


        public async Task<ResponseModel<IEnumerable<User>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();

                return new ResponseModel<IEnumerable<User>>
                {
                    Data = users,
                    Message = "Users retrieved successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<IEnumerable<User>>
                {
                    Data = null,
                    Message = "An error occurred while retrieving users",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<User> ValidateUser(Login login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(login.Email));

            if (user == null)
            {
                return null; 
            }

            if (BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
            {
                return user; 
            }
            else
            {
                return null; 
            }
        }


        public async Task<ResponseModel<LoginResponse>> Login(Login user)
        {
            try
            {
                if (user == null || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                    throw new ArgumentNullException("Email and password are required.");
                }

                // Fetch user from the database
                var userEntity = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (userEntity == null)
                {
                    return new ResponseModel<LoginResponse>
                    {
                        Data = null,
                        Success = false,
                        Message = "Invalid email or password.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                // Verify the password using BCrypt
                if (!BCrypt.Net.BCrypt.Verify(user.Password, userEntity.Password))
                {
                    return new ResponseModel<LoginResponse>
                    {
                        Data = null,
                        Success = false,
                        Message = "Invalid email or password.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                // Generate JWT token
                var token = _tokenUtility.GenerateToken(userEntity.UserId, userEntity.Email);

                return new ResponseModel<LoginResponse>
                {
                    Data = new LoginResponse
                    {
                        Token = token,
                        UserName = userEntity.Name // Assuming UserName is a field in your user entity
                    },
                    Success = true,
                    Message = "Login successful.",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (ArgumentNullException ex)
            {
                return new ResponseModel<LoginResponse>
                {
                    Data = null,
                    Success = false,
                    Message = ex.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<LoginResponse>
                {
                    Data = null,
                    Success = false,
                    Message = "An error occurred during login: " + ex.Message,
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }


        public async Task<ResponseModel<string>> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email));
            if (user == null)
            {
                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "User not found",
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Success = false
                };
            }

            // Generate password reset token using TokenUtility
            var token = _tokenUtility.GenerateToken(user.UserId, user.Email);

            // Send token via email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Vaibhav", _configuration["SMTP:FromEmail"]));
            message.To.Add(new MailboxAddress(user.Name, user.Email));
            message.Subject = "Your Password Reset Token";
            message.Body = new TextPart("plain")
            {
                Text = $"Here is your password reset token:\n\n{token}\n\nUse this token to reset your password. The token is valid for 1 hour."
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_configuration["SMTP:Host"], int.Parse(_configuration["SMTP:Port"]), SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_configuration["SMTP:Username"], _configuration["SMTP:Password"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return new ResponseModel<string>
            {
                Data = token,
                Message = "Password reset token sent successfully",
                StatusCode = (int)HttpStatusCode.OK,
                Success = true
            };
        }

        public async Task<ResponseModel<string>> ResetPassword(string token, string newPassword)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

                // Validate the token
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.Zero  // Reduce default clock skew for more accuracy
                }, out SecurityToken validatedToken);

                // Extract email from the token's claims
                var emailClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (emailClaim == null)
                {
                    return new ResponseModel<string>
                    {
                        Data = null,
                        Message = "Invalid token",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Success = false
                    };
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(emailClaim.Value));
                if (user == null)
                {
                    return new ResponseModel<string>
                    {
                        Data = null,
                        Message = "User not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                // Hash the new password and update the user record
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "Password reset successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (SecurityTokenException)
            {
                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "Invalid or expired token",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Success = false
                };
            }
            catch (Exception ex)
            {
                return new ResponseModel<string>
                {
                    Data = null,
                    Message = "An error occurred while resetting the password",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        public async Task<ResponseModel<User>> GetUserDetails()
        {
            try
            {
                // Retrieve UserId from claims
                var userId = GetUserIdFromClaims();
                if (userId == null)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "User is not authenticated",
                        StatusCode = (int)HttpStatusCode.Unauthorized,
                        Success = false
                    };
                }

                // Fetch the user by UserId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId.Value);

                // Check if user exists
                if (user == null)
                {
                    return new ResponseModel<User>
                    {
                        Data = null,
                        Message = "User not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Success = false
                    };
                }

                return new ResponseModel<User>
                {
                    Data = user,
                    Message = "User retrieved successfully",
                    StatusCode = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception)
            {
                return new ResponseModel<User>
                {
                    Data = null,
                    Message = "An error occurred while retrieving the user",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }


    }
}
