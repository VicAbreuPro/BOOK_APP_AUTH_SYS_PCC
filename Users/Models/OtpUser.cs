using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Users.Models
{
    public class OtpUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int RequestId { get; set; }
        public required string Email { get; set; }
        public required string Otp { get; set; }
    }
}
