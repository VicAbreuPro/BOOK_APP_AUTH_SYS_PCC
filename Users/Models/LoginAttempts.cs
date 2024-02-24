using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Users.Models
{
    public class LoginAttempts
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key()]
        public int AttemptId { get; set; }
        public required string email { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
