using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Reversi_api.Models
{
    public class Game
    {
        [Required]
        public int Id { get; set; }
        public Player? PlayerWhite { get; set; }
        public Player? PlayerBlack { get; set; }
        [JsonIgnore]
        public List<Column>? Columns { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
    }
}
