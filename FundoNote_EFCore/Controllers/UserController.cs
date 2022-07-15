using BussinessLayer.Interface;
using DatabaseLayer.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer;
using DatabaseLayer;
using RepositoryLayer.Service;
using System;

namespace FundoNote_EFCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly FundoContext fundoContext;
        private readonly IUserBL userBl;

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
    }
}
