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


        public UserController(FundoContext fundoContext, IUserBL userBL)
        {
            this.fundoContext = fundoContext;
            this.userBl = userBL;
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
    }
}
