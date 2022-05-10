using System.ComponentModel.DataAnnotations;

namespace Shared.Dto
{
    public class RegisterRequestDto
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
