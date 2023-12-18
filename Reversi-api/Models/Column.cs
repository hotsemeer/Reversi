using System.Text.Json.Serialization;

namespace Reversi_api.Models
{
    public class Column
    {
        [JsonIgnore]
        public int Id { get; set; }
        public List<Row> Rows { get; set; }
        [JsonIgnore]
        public int GameId { get; set; }
    }
}
