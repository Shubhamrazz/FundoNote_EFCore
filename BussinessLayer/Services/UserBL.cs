using BussinessLayer.Interface;
using DatabaseLayer.UserModels;
using RepositoryLayer.Interface;
using RepositoryLayer.Service.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayer.Service
{
    public class UserBL : IUserBL
    {
        IUserRL userRL;
        public UserBL(IUserRL userRL)
        {
            this.userRL = userRL;
        }
        public void AddUser(UserPostModel userPostModel)
        {
            try
            {
                this.userRL.AddUser(userPostModel);
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
                return this.userRL.GetAllUsers();
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
                return this.userRL.LoginUser(loginUser);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
