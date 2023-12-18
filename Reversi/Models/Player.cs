using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace Reversi.Models
{
    public class Player
    {
        public int Id { get; set; }
        [Required]
        [StringLength(25)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        [JsonIgnore]
        public string Password { get; set; }
        [JsonIgnore]
        public string PasswordSalt { get; set; }
    }
}
