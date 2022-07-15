using DatabaseLayer.UserModels;
using RepositoryLayer.Service.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayer.Interface
{
    public interface IUserBL
    {
        public void AddUser(UserPostModel userPostModel);
        public List<User> GetAllUsers();
    }
}
