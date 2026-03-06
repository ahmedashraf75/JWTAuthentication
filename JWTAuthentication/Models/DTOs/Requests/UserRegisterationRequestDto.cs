using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.Models.DTOs.Requests
{
    public class UserRegisterationRequestDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
