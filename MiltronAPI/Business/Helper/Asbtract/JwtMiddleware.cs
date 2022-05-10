using Business.BusinessRules.Interfaces;
using Business.Helper.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Business.Helper.Abstract
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthBusinessRules userBusinessRules, IJwtUtils jwtUtils)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();                                
            var userId = jwtUtils.ValidateToken(token);
            if (userId != null)
            {                
                context.Items["User"] = userBusinessRules.GetById(userId.Value);
            }

            await _next(context);
        }
    }
}
