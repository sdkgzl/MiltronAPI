using Business.Helper.Interfaces;
using DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Business.Helper.Abstract
{
    public class UserOperationHelper : IUserOperationHelper
    {
        private readonly IConfiguration _configuration; 
        public UserOperationHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public PasswordHelper CreatePasswordHash(string password)
        {
            PasswordHelper passwordHelper = new();
            using (var hmac = new HMACSHA512())
            {
                passwordHelper.PasswordSalt= hmac.Key;
                passwordHelper.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            return passwordHelper;
        }

        public string CreateToken(User model)
        {
            List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(ClaimTypes.Name,model.UserName),
                new System.Security.Claims.Claim(ClaimTypes.Role,"noob")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public bool VerifyPasswordHash(string password, PasswordHelper passwordHelper)
        {
            using (var hmac = new HMACSHA512(passwordHelper.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHelper.PasswordHash);
            }
        }


    }
}
