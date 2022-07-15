using System;
using System.Collections.Generic;
using System.Text;
using DatabaseLayer.UserModels;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        public void AddUser(UserPostModel userPostModel);
    }
}
