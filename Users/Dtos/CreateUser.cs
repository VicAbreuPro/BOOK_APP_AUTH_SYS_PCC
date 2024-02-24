using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
    public class CreateUser
    {
        [Required(ErrorMessage = "Username is required!", AllowEmptyStrings = false)]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required!", AllowEmptyStrings = false)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Email is required!", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "FullName is required!", AllowEmptyStrings = false)]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Country is required!", AllowEmptyStrings = false)]
        public required string Country { get; set; }

        [Required(ErrorMessage = "Role is required!", AllowEmptyStrings = false)]
        public required string Role { get; set; }
    }
}
