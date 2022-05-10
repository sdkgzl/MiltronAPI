using System.ComponentModel.DataAnnotations;
namespace Shared.Dto
{
    public class AuthenticateRequestDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
