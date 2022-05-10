using Shared.Dto;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace Business.Extensions
{
    public static class ClaimExtensions
    {
        public static void AddEmail(this ICollection<Claim> claims, string email)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));
        }
        public static void AddPrn(this ICollection<Claim> claims, string Prn)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Prn, Prn));
        }

        public static void AddName(this ICollection<Claim> claims, string name)
        {
            claims.Add(new Claim(ClaimTypes.Name, name));
        }

        public static void AddNameIdentifier(this ICollection<Claim> claims, string nameIdentifier)
        {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, nameIdentifier));
        }

    }
}
