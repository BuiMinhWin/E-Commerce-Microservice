using AuthenticationAPI.Application.DTOs;
using AuthenticationAPI.Application.Interfaces;
using AuthenticationAPI.Domain.Entities;
using AuthenticationAPI.Infrastructure.Data;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationAPI.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
    {
        private async Task<User> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.Email == email);
            return user is null? null: user;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
            var user = await context.Users.FindAsync(userId);
            return user is not null ? new GetUserDTO
            (
                user.Id,
                user.Name,
                user.TelephoneNumber,
                user.Address,
                user.Email,
                user.Role,
                user.Password
            ) : null;
        }


        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.email);
            if (getUser is null) return new Response(false,"Invalid credentials");
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password,getUser.Password);
            Console.Write(verifyPassword);
            if (!verifyPassword) return new Response(false, "Invalid credentials");

            string token = GenerateToken(getUser);
            Console.Write(token);
                return new Response(true, token);

        }

        private string GenerateToken (User user) 
        {
            var key= Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);

            var securityKey = new SymmetricSecurityKey(key);

            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name!),
                new(ClaimTypes.Email, user.Email!),
              
            };
            if(string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role)) claims.Add(new(ClaimTypes.Role, user.Role));

            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<Response> Register(UserDTO userDTO)
        {
            var getUser = await GetUserByEmail(userDTO.Email);
            if (getUser is not null) 
                return new Response(false, $"your email cannot register ");
            var result = context.Users.Add(new User()
            {
        
                Name = userDTO.Name,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                TelephoneNumber = userDTO.TelephoneNumber,
                Address = userDTO.Address,
                Role = userDTO.Role,
            });
            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "Register successfully") : new Response(false, "Invalid data");
        }
    }
}
