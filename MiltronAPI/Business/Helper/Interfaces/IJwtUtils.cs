using DataAccess.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Dto;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helper.Interfaces
{
    public interface IJwtUtils
    {
        /// <summary>
        /// This method is helped to generate token.
        /// </summary>
        public string GenerateToken(User user);

        /// <summary>
        /// This method is controlled about generated token.
        /// </summary>
        public int? ValidateToken(string token);
    }
}
