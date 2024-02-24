using System.ComponentModel.DataAnnotations;

namespace Users.Dtos
{
    public class MailObject
    {
        [Required(ErrorMessage = "Email Destination is required!", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string To { get; set; }

        [Required(ErrorMessage = "Subject is required!", AllowEmptyStrings = false)]
        public required string Subject { get; set; }

        [Required(ErrorMessage = "Message is required!", AllowEmptyStrings = false)]
        public required string Message { get; set; }

        
    }
}
