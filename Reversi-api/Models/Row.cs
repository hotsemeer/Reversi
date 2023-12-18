using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Reversi_api.Models
{
    public class Row
    {
        [Required]
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        [JsonIgnore]
        public int GameId{ get; set; }
        [Required]
        [JsonIgnore]
        public int ColumnId { get; set; }
        [Range(0, 2)]
        public int Value { get; set; } = 0;
    }
}
