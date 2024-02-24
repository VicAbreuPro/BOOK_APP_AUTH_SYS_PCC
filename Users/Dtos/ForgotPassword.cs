using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
    public class ForgotPassword
    {
        [Required(ErrorMessage = "Email is required!", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; set; }
    }
}
