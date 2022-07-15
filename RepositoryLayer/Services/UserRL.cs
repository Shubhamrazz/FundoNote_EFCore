using DatabaseLayer.UserModels;
using Microsoft.Extensions.Configuration;
using Experimental.System.Messaging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using RepositoryLayer.Interface;
using RepositoryLayer.Service.Entities;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepositoryLayer.Service
{
    public class UserRL : IUserRL
    {
        FundoContext fundoContext;
        IConfiguration iconfiguration;
        public UserRL(FundoContext fundoContext, IConfiguration iconfiguration)
        {
            this.fundoContext = fundoContext;
            this.iconfiguration = iconfiguration;
        }
        public void AddUser(UserPostModel userPostModel)
        {
            try
            {
                User user = new User();
                user.Firstname = userPostModel.Firstname;
                user.Lastname = userPostModel.Lastname;
                user.Email = userPostModel.Email;
                user.Password = userPostModel.Password;
                user.CreateDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                fundoContext.Users.Add(user);
                fundoContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return this.fundoContext.Users.ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string LoginUser(UserLoginModel loginUser)
        {
            try
            {
                var user = fundoContext.Users.Where(x => x.Email == loginUser.Email && x.Password == loginUser.Password).FirstOrDefault();

                if (user == null)
                {
                    return null;
                }
                return GenerateJWTToken(loginUser.Email, user.UserId);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GenerateJWTToken(string email, int UserId)
        {
            try
            {
                // generate token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("Email", email),
                    new Claim("UserId",UserId.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),

                    SigningCredentials =
                new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature),
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
