using BussinessLayer.Interface;
using DatabaseLayer.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer;
using DatabaseLayer;
using RepositoryLayer.Service;
using System;
using RepositoryLayer.Service.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using NLogger.Interface;
using Microsoft.AspNetCore.Authorization;

namespace FundoNote_EFCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly FundoContext fundoContext;
        private readonly IUserBL userBl;
        private readonly ILoggerManager logger;


        public UserController(FundoContext fundoContext, IUserBL userBL, ILoggerManager logger)
        {
            this.fundoContext = fundoContext;
            this.userBl = userBL;
            this.logger = logger;
        }

        [HttpPost("AddUser")]
        public IActionResult AddUser(UserPostModel userModel)
        {
            try
            {
                this.userBl.AddUser(userModel);
                return this.Ok(new { success = true, message = "User Created Successfully" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                List<User> getusers = new List<User>();
                getusers = this.userBl.GetAllUsers();
                return Ok(new { success = false, message = "GetAll users Fetch Successfully", data = getusers });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("LoginUser")]
        public IActionResult LoginUser(UserLoginModel userModel)
        {
            try
            {
                this.logger.LogInfo($"User cred Email : {userModel.Email}");
                string token = this.userBl.LoginUser(userModel);
                if (token == null)
                {
                    return this.BadRequest(new { success = false, message = "Enter Valid Email and Password" });
                }

                return this.Ok(new { success = true, message = "User Login Successfully", data = token });
            }
            catch (Exception ex)
            {
                this.logger.LogError($"User cred Failed : {userModel.Email}");
                throw ex;
            }
        }

        [HttpPost("ForgetUser/{email}")]
        public IActionResult ForgetUser(string email)
        {
            try
            {
                bool isExist = this.userBl.ForgetPasswordUser(email);
                if (isExist) return Ok(new { success = true, message = $"Reset Link sent to Eamil : {email}" });
                else return BadRequest(new { success = false, message = $"No user Exist with Email : {email}" });
            }
            catch (Exception ex)
            {
                this.logger.LogError($"User cred Failed : {email}");
                throw ex;
            }
        }

        [Authorize]
        [HttpPut("ResetUser")]
        public IActionResult ResetUser(PasswordModel passwordModel)
        {
            try
            {
                if (passwordModel.Password != passwordModel.CPassword)
                {
                    return this.BadRequest(new { success = false, message = "New Password and Confirm Password are not equal." });
                }

                var identity = User.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    IEnumerable<Claim> claims = identity.Claims;
                    var email = claims.Where(p => p.Type == @"Email").FirstOrDefault()?.Value;
                    this.userBl.ResetPassoword(email, passwordModel);
                    return this.Ok(new { success = true, message = "Password Changed Sucessfully", email = $"{email}" });
                }

                return this.BadRequest(new { success = false, message = "Password Changed Unsuccessful" });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
