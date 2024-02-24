using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
    public class Login
    {
        [Required(ErrorMessage = "Password is required!", AllowEmptyStrings = false)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email is required!", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }
    }
}
