using System;
using System.Collections.Generic;
using System.Text;
using DatabaseLayer.UserModels;
using RepositoryLayer.Service.Entities;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        public void AddUser(UserPostModel userPostModel);
        public List<User> GetAllUsers();
    }
}
