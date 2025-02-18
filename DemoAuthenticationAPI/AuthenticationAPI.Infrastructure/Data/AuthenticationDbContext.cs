﻿using AuthenticationAPI.Application.DTOs;
using AuthenticationAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationAPI.Infrastructure.Data
{
        public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext>options) : DbContext(options)
        {
            public DbSet<User> Users { get; set; }
        }
    
}
