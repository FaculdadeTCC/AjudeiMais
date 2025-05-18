using System.Text.Json.Serialization;

namespace AjudeiMais.API.Models
{
    public class NominatimResultDto
    {
        [JsonPropertyName("lat")]
        public string Latitude { get; set; }
        [JsonPropertyName("lon")]
        public string Longitude { get; set; }
    }
}
