using DatabaseLayer.UserModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinessLayer.Interface
{
    public interface IUserBL
    {
        public void AddUser(UserPostModel userPostModel);
    }
}
