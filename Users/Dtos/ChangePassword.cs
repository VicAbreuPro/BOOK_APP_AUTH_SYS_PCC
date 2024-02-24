using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
    public class ChangePassword
    {
        [Required(ErrorMessage = "OldPassword is required!", AllowEmptyStrings = false)]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "NewPassword is required!", AllowEmptyStrings = false)]
        public required string NewPassword { get; set; }
    }
}
