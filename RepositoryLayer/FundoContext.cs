using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Service.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer
{
    public class FundoContext : DbContext
    {
        public FundoContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
    }
}
